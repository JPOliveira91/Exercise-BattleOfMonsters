using Lib.Repository.Entities;
using System.Diagnostics.CodeAnalysis;

namespace API.Test.Fixtures;

[ExcludeFromCodeCoverage]
public static class BattlesFixture
{
    public static IEnumerable<Battle> GetBattlesMock()
    {
        return new[]
        {
            new Battle()
            {
                Id = 1,
                MonsterA = 1,
                MonsterB = 2,
                Winner = 1
            }
        };
    }
}