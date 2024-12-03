using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arma3TacMapLibrary.Arma3
{
    [Obsolete]
    public interface IMapInfosService
    {
        Task<List<MapInfos>> GetMapsInfos();
        Task<MapInfos> GetMapsInfos(string worldName);
    }
}