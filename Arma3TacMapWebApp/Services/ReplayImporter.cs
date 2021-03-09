using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Http;

namespace Arma3TacMapWebApp.Services
{
    public class ReplayImporter
    {
        private readonly Regex LTLine = new Regex("^(([0-9]{4})/([ 0-9]{1,2})/([ 0-9]{1,2}), )?([ 0-9]{2}:[0-9]{2}:[0-9]{2}) LT::([A-Z])( (.*))?$", RegexOptions.Compiled);

        private readonly Arma3TacMapContext _db;

        private const string aarJsonPrelude = "aarFileData = ";

        public ReplayImporter(Arma3TacMapContext db)
        {
            _db = db;
        }

        internal async Task<ReplayMap> ImportAarTxt(User owner, IFormFile file, string label)
        {
            var map = Init(owner, label);

            var data = await ReadAarJson(file);

            map.WorldName = data.metadata.island;
            if(string.IsNullOrEmpty(map.Label))
            {
                map.Label = data.metadata.name;
            }
            map.Groups.Add(new ReplayGroup() { GroupNumber = 0, Name = "BLUFOR", Side = Arma3Side.BLUFOR, ReplayMap = map });
            map.Groups.Add(new ReplayGroup() { GroupNumber = 1, Name = "OPFOR", Side = Arma3Side.OPFOR, ReplayMap = map });
            map.Groups.Add(new ReplayGroup() { GroupNumber = 2, Name = "INDEP", Side = Arma3Side.Independant, ReplayMap = map });
            map.Groups.Add(new ReplayGroup() { GroupNumber = 3, Name = "CIV", Side = Arma3Side.Civilian, ReplayMap = map });
            map.Players.AddRange(data.metadata.players.Select((p,i) => new ReplayPlayer() { PlayerNumber = i, Name = p[0], ReplayMap = map }));
            map.Units.AddRange(data.metadata.objects.units.Select((p, i) => new ReplayUnit() { UnitNumber = p[0].GetInt32(), Name = p[1].GetString(), ReplayMap = map, Side = ToArma3Side(p[2].GetString()) }));
            map.Vehicles.AddRange(data.metadata.objects.vehs.Select((p, i) => new ReplayVehicle() { VehicleNumber = p[0].GetInt32(), ClassName = p[1].GetString(), ReplayMap = map }));
            
            foreach (var frameData in data.timeline)
            {
                var prevFrame = map.Frames.LastOrDefault();
                var frame = new ReplayFrame() { FrameNumber = map.Frames.Count, ReplayMap = map, Data = new ReplayFrameData() { Positions = new List<ReplayPosition>() } };
                map.Frames.Add(frame);

                var units = frameData[0];
                foreach(var unit in units)
                {
                    var num = unit[0];
                    var unitMetadata = map.Units.FirstOrDefault(u => u.UnitNumber == num);
                    if (unit.Count == 1)
                    {
                        if (prevFrame != null)
                        {
                            var lastPosition = frame.Data.Positions.FirstOrDefault(p => p.UnitNumber == num);
                            if (lastPosition != null)
                            {
                                frame.Data.Positions.Add(new ReplayPosition()
                                {
                                    Direction = lastPosition.Direction,
                                    X = lastPosition.X,
                                    Y = lastPosition.Y,
                                    IsAlive = lastPosition.IsAlive,
                                    UnitNumber = lastPosition.UnitNumber,
                                    GroupNumber = lastPosition.GroupNumber,
                                    VehicleNumber = lastPosition.VehicleNumber
                                });
                            }
                        }
                    }
                    else
                    {
                        var x = unit[1];
                        var y = unit[2];
                        var dir = unit[3];
                        var alive = unit[4];
                        var inCargo = unit[5];
                        frame.Data.Positions.Add(new ReplayPosition()
                        {
                            Direction = unit[3],
                            X = unit[1],
                            Y = unit[2],
                            IsAlive = unit[4] != 0,
                            UnitNumber = num,
                            GroupNumber = map.Groups.FirstOrDefault(g => g.Side == unitMetadata?.Side)?.GroupNumber
                        });
                    }
                }

                var vehs = frameData[1];
                foreach (var veh in vehs)
                {
                    var num = veh[0];
                    if (veh.Count == 1)
                    {
                        if (prevFrame != null)
                        {
                            var lastPosition = frame.Data.Positions.FirstOrDefault(p => p.UnitNumber == null && p.VehicleNumber == num);
                            if (lastPosition != null)
                            {
                                frame.Data.Positions.Add(new ReplayPosition()
                                {
                                    Direction = lastPosition.Direction,
                                    X = lastPosition.X,
                                    Y = lastPosition.Y,
                                    IsAlive = lastPosition.IsAlive,
                                    UnitNumber = lastPosition.UnitNumber,
                                    GroupNumber = lastPosition.GroupNumber,
                                    VehicleNumber = lastPosition.VehicleNumber
                                });
                            }
                        }
                    }
                    else
                    {
                        var ownerNum = veh[5];
                        var cargoCount = veh[6];
                        ReplayPosition ownerPosition = null;
                        if (ownerNum != -1 )
                        {
                            ownerPosition = frame.Data.Positions.FirstOrDefault(p => p.UnitNumber == ownerNum);
                            if (ownerPosition != null)
                            {
                                ownerPosition.VehicleNumber = num;
                            }
                        }
                        frame.Data.Positions.Add(new ReplayPosition()
                        {
                            Direction = veh[3],
                            X = veh[1],
                            Y = veh[2],
                            IsAlive = veh[4] != 0,
                            VehicleNumber = num,
                            GroupNumber = ownerPosition?.GroupNumber,
                            PlayerNumber = ownerPosition?.PlayerNumber,
                        });
                    }
                }
                var attacks = frameData[2];

                frame.Data.Positions = frame.Data.Positions.OrderBy(p => p.UnitNumber).ThenBy(p => p.VehicleNumber).ToList();
            }


            return await SaveAll(map);
        }

        private static Arma3Side? ToArma3Side(string v)
        {
            switch(v.ToLowerInvariant())
            {
                case "blufor": 
                    return Arma3Side.BLUFOR;
                case "opfor": 
                    return Arma3Side.OPFOR;
                case "indep": 
                    return Arma3Side.Independant;
                case "civ": 
                    return Arma3Side.Civilian;
            }
            return null;
        }

        private static async Task<AarJson> ReadAarJson(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var buffer = new byte[aarJsonPrelude.Length];
                await stream.ReadAsync(buffer);
                if (Encoding.UTF8.GetString(buffer) != aarJsonPrelude)
                {
                    throw new ApplicationException("Invalid AAR text file");
                }
                return await JsonSerializer.DeserializeAsync<AarJson>(stream);
            }
        }

        internal async Task<ReplayMap> ImportAarLog(User owner, IFormFile file, string label)
        {
            //var map = Init(owner, label);

            throw new ApplicationException("Sorry, not yet supported !");

            //return await SaveAll(map);
        }

        internal async Task<ReplayMap> ImportGtdLog(User owner, IFormFile file, string label, TimeZoneInfo tz)
        {
            var map = Init(owner, label);

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    var match = LTLine.Match(line);
                    if (match.Success)
                    {
                        var time = match.Groups[3].Value;
                        if (match.Groups[1].Success)
                        {
                            time = match.Groups[2].Value + " " + time;
                        }
                        ProcessGtdLine(tz, map, time, match.Groups[4].Value, match.Groups[6].Value);
                    }
                }
            }
            return await SaveAll(map);
        }

        private void ProcessGtdLine(TimeZoneInfo tz, ReplayMap map, string time, string cmd, string data)
        {
            var dataArray = data.Split("|");

            switch (cmd)
            {
                case "D":
                    Date(map, dataArray[0], dataArray[1]);
                    break;
                case "B":
                    Start(map, tz, time, dataArray[0], dataArray[1]);
                    break;
                case "C":
                    Clock(map, tz, time);
                    break;
                case "G":
                    RegisterGroup(map, int.Parse(dataArray[0], CultureInfo.InvariantCulture), dataArray[1], dataArray[2]);
                    break;
                case "R":
                    if (dataArray[1] == "U")
                    {
                        RegisterUnit(map, int.Parse(dataArray[0], CultureInfo.InvariantCulture), dataArray[2], int.Parse(dataArray[3], CultureInfo.InvariantCulture), dataArray[4], dataArray[5]);
                    }
                    else
                    {
                        RegisterVehicle(map, int.Parse(dataArray[0], CultureInfo.InvariantCulture), dataArray[2]);
                    }
                    break;
                case "S":
                    Update(map, int.Parse(dataArray[0]), int.Parse(dataArray[1], CultureInfo.InvariantCulture), int.Parse(dataArray[2], CultureInfo.InvariantCulture), bool.Parse(dataArray[3]), double.Parse(dataArray[4], CultureInfo.InvariantCulture), double.Parse(dataArray[5], CultureInfo.InvariantCulture), double.Parse(dataArray[6], CultureInfo.InvariantCulture), dataArray[7]);
                    break;
            }
        }

        private void Date(ReplayMap map, string gameTime, string serverUtcTime)
        {
            if (map.Frames.Count == 0)
            {
                return;
            }
            var currentFrame = map.Frames[map.Frames.Count - 1];
            currentFrame.GameDateTime = DateTime.ParseExact(gameTime, "yyyy:M:d:H:m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            currentFrame.ServerDateTimeUtc = DateTime.ParseExact(serverUtcTime, "yyyy:M:d:H:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        private void Start(ReplayMap map, TimeZoneInfo tz, string time, string world, string mission)
        {
            map.WorldName = world;
            if (string.IsNullOrEmpty(map.Label))
            {
                map.Label = $"{mission} {time}";
            }
            Clock(map, tz, time);
        }

        private void Update(ReplayMap map, int index, int groupIndex, int vehicleIndex, bool alive, double x, double y, double dir, string playerUid)
        {
            if (map.Frames.Count == 0)
            {
                return;
            }
            var currentFrame = map.Frames[map.Frames.Count - 1];
            var unit = map.Units.FirstOrDefault(u => u.UnitNumber == index);
            ReplayPlayer player = null;
            if (!string.IsNullOrEmpty(playerUid))
            {
                player = map.Players.FirstOrDefault(g => g.Uid == playerUid);
            }
            currentFrame.Data.Positions.Add(new ReplayPosition()
            {
                X = Math.Truncate(x),
                Y = Math.Truncate(y),
                Direction = Math.Truncate(dir),
                IsAlive = alive,
                PlayerNumber = player?.PlayerNumber,
                UnitNumber = unit?.UnitNumber,
                GroupNumber = groupIndex == -1 ? null : groupIndex,
                VehicleNumber = vehicleIndex == -1 || vehicleIndex == unit?.UnitNumber ? null : vehicleIndex
            });
            
        }

        private void RegisterVehicle(ReplayMap map, int index, string className)
        {
            map.Vehicles.Add(new ReplayVehicle()
            {
                VehicleNumber = index,
                ClassName = className,
                ReplayMap = map
            });
        }

        private void RegisterUnit(ReplayMap map, int index, string className, int groupIndex, string playerUid, string name)
        {
            map.Units.Add(new ReplayUnit()
            {
                UnitNumber = index,
                ClassName = className,
                Name = name
            });
            if (!string.IsNullOrEmpty(playerUid))
            {
                if (!map.Players.Any(p => p.Uid == playerUid))
                {
                    map.Players.Add(new ReplayPlayer()
                    {
                        Uid = playerUid,
                        PlayerNumber = map.Players.Count,
                        Name = name,
                        ReplayMap = map
                    });
                }
            }
        }

        private void RegisterGroup(ReplayMap map, int groupIndex, string side, string name)
        {
            map.Groups.Add(new ReplayGroup()
            {
                GroupNumber = groupIndex,
                Side = ToArma3Side(side),
                Name = name
            });
        }

        private void Clock(ReplayMap map, TimeZoneInfo tz, string time)
        {
            ReplayFrame currentFrame = null;
            if (map.Frames.Count > 0)
            {
                currentFrame = map.Frames[map.Frames.Count - 1];
                currentFrame.Data.Positions = currentFrame.Data.Positions.OrderBy(u => u.UnitNumber).ToList();
                if (currentFrame.ServerDateTimeUtc == null)
                {
                    var dt = DateTime.Parse(time);
                    if (dt.Kind == DateTimeKind.Unspecified || dt.Kind == DateTimeKind.Local)
                    {
                        dt = TimeZoneInfo.ConvertTimeToUtc(dt, tz);
                    }
                    currentFrame.ServerDateTimeUtc = dt;
                }
            }
            map.Frames.Add(new ReplayFrame()
            {
                ReplayMap = map,
                FrameNumber = map.Frames.Count,
                Data = new ReplayFrameData()
                {
                    Positions = GetDeads(currentFrame?.Data?.Positions)
                }
            });
        }

        private List<ReplayPosition> GetDeads(List<ReplayPosition> unitPositions)
        {
            if (unitPositions == null)
            {
                return new List<ReplayPosition>();
            }
            return unitPositions.Where(u => !u.IsAlive).ToList();
        }

        private ReplayMap Init(User owner, string label)
        {
            return new ReplayMap() 
            { 
                Owner = owner, 
                Label = label, 
                Created = DateTime.UtcNow,
                Frames = new List<ReplayFrame>(),
                Groups = new List<ReplayGroup>(),
                Players = new List<ReplayPlayer>(),
                Units = new List<ReplayUnit>(),
                Vehicles = new List<ReplayVehicle>()
            };
        }

        private async Task<ReplayMap> SaveAll(ReplayMap map)
        {
            if (string.IsNullOrEmpty(map.Label))
            {
                map.Label = "Imported";
            }
            _db.Add(map);
            await _db.SaveChangesAsync();
            return map;
        }
    }
}
