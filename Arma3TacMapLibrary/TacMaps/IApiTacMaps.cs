using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arma3TacMapLibrary.TacMaps
{
    public interface IApiTacMaps
    {
        Task<List<ApiTacMap>> List();

        Task<ApiTacMap> Get(int id);

        Task<ApiTacMap> Create(ApiTacMapCreate tacMap);

        Task<ApiTacMap> Update(int id, ApiTacMapPatch tacMap);

        Task Delete(int id);
    }
}