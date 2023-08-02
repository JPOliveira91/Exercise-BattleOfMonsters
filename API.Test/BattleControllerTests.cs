using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace API.Test;

[ExcludeFromCodeCoverage]
public class BattleControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public BattleControllerTests()
    {
        _repository = new Mock<IBattleOfMonstersRepository>();
    }

    [Fact]
    public async void Get_OnSuccess_ReturnsListOfBattles()
    {
        _repository
            .Setup(x => x.Battles.GetAllAsync())
            .ReturnsAsync(BattlesFixture.GetBattlesMock());

        BattleController sut = new BattleController(_repository.Object);
        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult) result;
        objectResults?.Value.Should().BeOfType<Battle[]>();
    }
    
    [Fact]
    public async Task Post_BadRequest_When_StartBattle_With_nullMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        
        Battle b = new Battle()
        {
            MonsterA = null,
            MonsterB = monstersMock[1].Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult) result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Missing ID", objectResults.Value);
    }
    
    [Fact]
    public async Task Post_OnNoMonsterFound_When_StartBattle_With_NonexistentMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle b = new Battle()
        {
            MonsterB = monstersMock[1].Id
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult)result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal($"Missing ID", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var choosenMonsterA = monstersMock[1];
        int? idMonsterA = choosenMonsterA.Id;

        var choosenMonsterB = monstersMock[0];
        int? idMonsterB = choosenMonsterB.Id;

        Battle b = new Battle()
        {
            MonsterA = idMonsterA,
            MonsterB = idMonsterB
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(choosenMonsterA);

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(choosenMonsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
        Monster? winner = objectResults?.Value as Monster;
        winner?.Id.Should().Be(idMonsterA);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var choosenMonsterA = monstersMock[0];
        int? idMonsterA = choosenMonsterA.Id;

        var choosenMonsterB = monstersMock[1];
        int? idMonsterB = choosenMonsterB.Id;

        Battle b = new Battle()
        {
            MonsterA = idMonsterA,
            MonsterB = idMonsterB
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(choosenMonsterA);

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(choosenMonsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
        Monster? winner = objectResults?.Value as Monster;
        winner?.Id.Should().Be(idMonsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirSpeedsSame_And_MonsterA_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var choosenMonsterA = monstersMock[3];
        int? idMonsterA = choosenMonsterA.Id;

        var choosenMonsterB = monstersMock[4];
        int? idMonsterB = choosenMonsterB.Id;

        Battle b = new Battle()
        {
            MonsterA = idMonsterA,
            MonsterB = idMonsterB
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(choosenMonsterA);

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(choosenMonsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
        Monster? winner = objectResults?.Value as Monster;
        winner?.Id.Should().Be(idMonsterA);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning_When_TheirSpeedsSame_And_MonsterB_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var choosenMonsterA = monstersMock[4];
        int? idMonsterA = choosenMonsterA.Id;

        var choosenMonsterB = monstersMock[3];
        int? idMonsterB = choosenMonsterB.Id;

        Battle b = new Battle()
        {
            MonsterA = idMonsterA,
            MonsterB = idMonsterB
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(choosenMonsterA);

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(choosenMonsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
        Monster? winner = objectResults?.Value as Monster;
        winner?.Id.Should().Be(idMonsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirDefensesSame_And_MonsterA_Has_Higher_Speed()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        var choosenMonsterA = monstersMock[0];
        int? idMonsterA = choosenMonsterA.Id;

        var choosenMonsterB = monstersMock[2];
        int? idMonsterB = choosenMonsterB.Id;

        Battle b = new Battle()
        {
            MonsterA = idMonsterA,
            MonsterB = idMonsterB
        };

        _repository.Setup(x => x.Battles.AddAsync(b));

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(choosenMonsterA);

        _repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(choosenMonsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
        Monster? winner = objectResults?.Value as Monster;
        winner?.Id.Should().Be(idMonsterA);
    }

    [Fact]
    public async Task Delete_OnSuccess_RemoveBattle()
    {
        const int id = 1;
        Battle[] battles = BattlesFixture.GetBattlesMock().ToArray();
        List<Battle> newBattles = new List<Battle>();

        var deletedBattle = battles[0];

        this._repository
            .Setup(x => x.Battles.FindAsync(id))
            .ReturnsAsync(deletedBattle);

        this._repository
            .Setup(x => x.Battles.RemoveAsync(deletedBattle))
            .Callback<Battle>(bat => newBattles = battles.Where(bt => bt.Id != bat.Id).ToList());

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Remove(id);
        OkResult objectResults = (OkResult)result;
        newBattles.Should().NotContain(deletedBattle);
        objectResults.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Delete_OnNoBattleFound_Returns404()
    {
        const int id = 123;

        _repository
            .Setup(x => x.Battles.FindAsync(id))
            .ReturnsAsync(() => null);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Remove(id);
        NotFoundObjectResult objectResults = (NotFoundObjectResult)result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The battle with ID = {id} not found.", objectResults.Value);
    }
}
