using Lib.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lib.Repository.Repository;

public class MonsterRepository : IMonsterRepository
{
    private readonly BattleOfMonstersContext _context;

    public MonsterRepository(BattleOfMonstersContext context)
    {
        _context = context;
    }
    
    public ValueTask<EntityEntry<Monster>> AddAsync(Monster monster)
    {
        return _context.Set<Monster>().AddAsync(monster);
    }

    public Task AddAsync(IEnumerable<Monster> monsters)
    {
        return _context.Set<Monster>().AddRangeAsync(monsters);
    }

    public ValueTask<Monster?> FindAsync(int? id)
    {
        return _context.Set<Monster>().FindAsync(id);
    }

    public async Task<Monster[]> GetAllAsync()
    {
        return await _context.Set<Monster>().ToArrayAsync();
    }

    public void RemoveAsync(Monster monster)
    {        
        _context.Set<Monster>().Remove(monster);
    }

    public void Update(Monster existingMonster, Monster updatedMonster)
    {
        _context.Entry<Monster>(existingMonster).CurrentValues.SetValues(updatedMonster);
    }
}