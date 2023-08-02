using Lib.Repository.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lib.Repository.Repository;

public interface IBattleRepository
{
    public ValueTask<EntityEntry<Battle>> AddAsync(Battle battle);
    public Task<Battle?> FindAsync(int id);
    public Task<IEnumerable<Battle>> GetAllAsync();
    public int? GetId();
    public void RemoveAsync(Battle battle);
}