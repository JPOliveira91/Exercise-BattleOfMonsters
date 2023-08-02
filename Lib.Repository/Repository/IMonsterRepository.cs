using Lib.Repository.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lib.Repository.Repository;

public interface IMonsterRepository
{
    public ValueTask<EntityEntry<Monster>> AddAsync(Monster monster);
    public Task AddAsync(IEnumerable<Monster> monsters);
    public ValueTask<Monster?> FindAsync(int? id);
    public Task<Monster[]> GetAllAsync();
    public void RemoveAsync(Monster monster);
    public void Update(Monster existingMonster, Monster updatedMonster);
}