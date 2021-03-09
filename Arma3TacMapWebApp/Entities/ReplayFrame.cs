using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayFrame
    {
        private ReplayFrameData _data;
        private string _jsonData;

        public int FrameNumber { get; set; }

        public int ReplayMapID { get; set; }

        public ReplayMap ReplayMap { get; set; }

        public DateTime? GameDateTime { get; set; }

        public DateTime? ServerDateTimeUtc { get; set; }

        public ReplayFrameData Data { get; set; }
    }
}
