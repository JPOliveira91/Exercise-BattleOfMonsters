using Lib.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lib.Repository.Repository;

public class BattleRepository : IBattleRepository
{
    private readonly BattleOfMonstersContext _context;

    public BattleRepository(BattleOfMonstersContext context)
    {
        _context = context;
    }

    public ValueTask<EntityEntry<Battle>> AddAsync(Battle battle)
    {
        return _context.Set<Battle>().AddAsync(battle);
    }

    public Task<Battle?> FindAsync(int id)
    {
        return _context.Set<Battle>().FindAsync(id).AsTask();
    }

    public async Task<IEnumerable<Battle>> GetAllAsync()
    {
        return await _context.Set<Battle>()
            .Include(x => x.MonsterARelation)
            .Include(x => x.MonsterBRelation)
            .Include(x => x.WinnerRelation)
            .ToArrayAsync();
    }

    public int? GetId()
    {
        var maxId = GetAllAsync().Result.MaxBy(battle => battle.Id);
        return maxId == null ? 1 : maxId.Id;
    }

    public void RemoveAsync(Battle battle)
    {
        _context.Set<Battle>().Remove(battle);
    }
}