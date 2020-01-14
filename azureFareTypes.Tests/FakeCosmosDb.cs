using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes.Tests
{
    public class FakeCosmosDb : IFareTypesCosmosDb
    {
        private Dictionary<string, CosmosFareType> fakeData = new Dictionary<string, CosmosFareType>();

        public async Task<FareType> Get(int id) => await Get(id.ToString());

        private async Task<FareType> Get(string id)
        {
            return fakeData.ContainsKey(id) ? fakeData[id] : null;
        }

        public async Task<FareType[]> GetAll(int skip, int limit)
        {
            return fakeData.Values.Skip(skip).Take(limit).Select(f => f.ToFareType()).ToArray();
        }

        public async Task<bool> Upsert(FareType data)
        {
            if (data == null)
                return false;

            fakeData[data.FareTypeId.ToString()] = new CosmosFareType(data, 1);
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var existingItem = await Get(id);
            if (existingItem != null)
            {
                fakeData.Remove(id);
            }
            return existingItem != null;
        }
    }
}
