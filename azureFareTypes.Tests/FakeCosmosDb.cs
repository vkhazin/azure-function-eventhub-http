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

        public async Task<FareType[]> GetAll()
        {
            return fakeData.Values.Select(f => f.ToFareType()).ToArray();
        }

        public async Task<bool> Insert(FareType data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var existingItem = await Get(data.FareTypeId);
            if (existingItem == null)
            {
                fakeData[data.FareTypeId.ToString()] = new CosmosFareType(data, 1);
            }
            return existingItem == null;
        }

        public async Task<bool> Update(string id, FareType data)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var existingItem = await Get(id);
            if (existingItem != null)
            {
                fakeData[data.FareTypeId.ToString()] = new CosmosFareType(data, 1);
            }
            return existingItem != null;
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
