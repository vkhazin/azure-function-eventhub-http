using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes
{
    public interface IFareTypesCosmosDb
    {
        Task<FareType> Get(int id);

        Task<FareType[]> GetAll(int skip, int limit);

        Task<bool> Upsert(FareType data);

        Task<bool> Delete(string id);
    }
}
