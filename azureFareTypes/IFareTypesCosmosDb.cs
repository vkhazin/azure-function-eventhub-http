using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes
{
    public interface IFareTypesCosmosDb
    {
        Task<FareType> Get(int id);

        Task<FareType[]> GetAll();

        Task<bool> Insert(FareType data);

        Task<bool> Update(string id, FareType data);

        Task<bool> Delete(string id);
    }
}
