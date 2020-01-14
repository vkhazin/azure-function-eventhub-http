using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes
{
    public class FareTypesDbContext : DbContext
    {
        public FareTypesDbContext(DbContextOptions<FareTypesDbContext> options)
            : base(options)
        { }

        public DbSet<FareType> FareTypes { get; set; }

        public async Task<bool> Delete(int id)
        {
            var fareType = FareTypes.Find(id);
            if (fareType != null)
            {
                FareTypes.Remove(fareType);
                await SaveChangesAsync();
            }
            return fareType != null;
        }

        public async Task<bool> Insert(FareType data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var fareType = FareTypes.Find(data.FareTypeId);
            if (fareType == null)
            {
                FareTypes.Add(data);
                await SaveChangesAsync();
            }
            return fareType == null;
        }

        public async Task<bool> Upsert(int id, FareType data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var fareType = FareTypes.Find(data.FareTypeId);
            if (fareType == null)
            {
                FareTypes.Add(data);
            }
            else
            {
                foreach (var property in typeof(FareType).GetProperties())
                {
                    property.SetValue(fareType, property.GetValue(data, null), null);
                }
            }
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(int id, FareType data)
        {
            var fareType = FareTypes.Find(id);
            if (fareType != null)
            {
                foreach (var property in typeof(FareType).GetProperties())
                {
                    property.SetValue(fareType, property.GetValue(data, null), null);
                }
                await SaveChangesAsync();
            }
            return fareType != null;
        }
    }
}
