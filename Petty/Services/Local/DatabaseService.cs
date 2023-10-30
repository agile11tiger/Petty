using Microsoft.EntityFrameworkCore;
namespace Petty.Services.Local;

public class DatabaseService(ApplicationDbContext applicationDbContext) : Service
{
    public ApplicationDbContext ApplicationDbContext => applicationDbContext;

    public async Task CreateOrUpdateAsync<T>(T item) where T : class, IDatabaseItem
    {
        if (Contains<T>(item.Id))
        {
            await UpdateAsync<T>(item);
        }
        else
        {
            await ApplicationDbContext.AddAsync(item);
            await ApplicationDbContext.SaveChangesAsync();
            ApplicationDbContext.ChangeTracker.Clear();
        }
    }

    public async ValueTask<T> ReadAsync<T>(int id) where T : class
    {
        return await ApplicationDbContext.FindAsync<T>(id);
    }

    public async Task UpdateAsync<T>(T item) where T : class
    {
        ApplicationDbContext.Update(item);
        await ApplicationDbContext.SaveChangesAsync();
        ApplicationDbContext.ChangeTracker.Clear();
    }

    public async Task DeleteAsync<T>(T item) where T : class
    {
        ApplicationDbContext.Remove<T>(item);
        await ApplicationDbContext.SaveChangesAsync();
        ApplicationDbContext.ChangeTracker.Clear();
    }

    public bool Contains<T>(int id) where T : class
    {
        var result = ApplicationDbContext.Find<T>(id);

        if (result != default)
            ApplicationDbContext.Entry(result).State = EntityState.Detached;

        return result != default;
    }
}
