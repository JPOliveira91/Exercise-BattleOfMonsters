namespace Lib.Repository.Repository;

public class BattleOfMonstersRepository : IBattleOfMonstersRepository
{
    private readonly BattleOfMonstersContext _context;
    private BattleRepository? _battles;
    private MonsterRepository? _monsters;

    public BattleOfMonstersRepository(BattleOfMonstersContext context)
    {
        _context = context;
    }

    public IBattleRepository Battles
    {
        get { return _battles ??= new BattleRepository(_context); }
    }

    public IMonsterRepository Monsters
    {
        get { return _monsters ??= new MonsterRepository(_context); }
    }

    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }
}