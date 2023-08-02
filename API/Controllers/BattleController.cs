using Lib.Repository.Entities;
using Lib.Repository.Mappings;
using Lib.Repository.Repository;
using Lib.Repository.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class BattleController : BaseApiController
{
    private readonly IBattleOfMonstersRepository _repository;

    public BattleController(IBattleOfMonstersRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<Battle> battles = await _repository.Battles.GetAllAsync();
        return Ok(battles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Add([FromBody] Battle battle)
    {
        Monster winner = await CreateBattle(battle.MonsterA, battle.MonsterB);

        if (winner == null) return new BadRequestObjectResult("Missing ID");

        return Ok(winner);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Remove(int id)
    {
        var entity = await _repository.Battles.FindAsync(id);

        if (entity == null) return new NotFoundObjectResult(string.Format($"The battle with ID = {id} not found."));

        _repository.Battles.RemoveAsync(entity);
        await _repository.Save();
        return Ok();
    }

    #region Battle
    private async Task<Monster> CreateBattle(int? monsterAId, int? monsterBId)
    {
        if (monsterAId == null || monsterBId == null)
        {
            return null;
        }

        var monsterA = _repository.Monsters.FindAsync(monsterAId);
        var monsterB = _repository.Monsters.FindAsync(monsterBId);

        if (monsterA.Result == null || monsterB.Result == null)
        {
            return null;
        }

        var battleResult = Fight(monsterA.Result, monsterB.Result);

        await _repository.Battles.AddAsync(battleResult);
        await _repository.Save();

        return (monsterA.Result.Id == battleResult.Winner) ? monsterA.Result : monsterB.Result;
    }

    private Battle Fight(Monster monsterA, Monster monsterB)
    {
        // Who attacks first
        bool monsterAFirst = true;

        if (monsterB.Speed > monsterA.Speed)
        {
            monsterAFirst = false;
        }
        else if (monsterB.Speed == monsterA.Speed)
        {
            if (monsterB.Attack > monsterA.Attack)
            {
                monsterAFirst = false;
            }
        }

        while (monsterA.Hp > 0 && monsterB.Hp > 0)
        {
            // Turn
            if (monsterAFirst)
            {
                Turn(ref monsterA, ref monsterB);
            }
            else
            {
                Turn(ref monsterB, ref monsterA);
            }
        }

        var winner = (monsterA.Hp > 0) ? monsterA.Id : monsterB.Id;

        return new Battle() {
            Id = _repository.Battles.GetId(),
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id,
            Winner = winner
        };
    }

    private void Turn(ref Monster firstMonster, ref Monster secondMonster)
    {
        var firstDamage = (firstMonster.Attack - secondMonster.Defense) > 0 ? (firstMonster.Attack - secondMonster.Defense) : 1;

        secondMonster.Hp -= firstDamage;

        if(secondMonster.Hp > 0)
        {
            var secondDamage = (secondMonster.Attack - firstMonster.Defense) > 0 ? (secondMonster.Attack - firstMonster.Defense) : 1;

            firstMonster.Hp -= secondDamage;
        }
    }
    #endregion
}
