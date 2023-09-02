using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Petty.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local
{
    public class DatabaseService
    {
        public DatabaseService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        private readonly ApplicationDbContext _applicationDbContext;

        public async Task CreateOrUpdateAsync<T>(T item) where T: class, IDatabaseItem
        {
            if (Contains<T>(item.Id, out T result))
            {
                await UpdateAsync<T>(result);
            }
            else
            {
                await _applicationDbContext.AddAsync(item);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async ValueTask<T> ReadAsync<T>(int id) where T : class
        {
            return await _applicationDbContext.FindAsync<T>(id);
        }

        public async Task UpdateAsync<T>(T item) where T : class
        {
            _applicationDbContext.Update(item);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(T item) where T : class
        {
            _applicationDbContext.Remove<T>(item);
            await _applicationDbContext.SaveChangesAsync();
        }

        public bool Contains<T>(int id, out T result) where T : class
        {
            result = _applicationDbContext.Find<T>(id);
            return result != default;
        }
    }
}
