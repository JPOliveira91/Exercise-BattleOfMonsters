using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace API.Test;

[ExcludeFromCodeCoverage]
public class MonsterControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public MonsterControllerTests()
    {
        _repository = new Mock<IBattleOfMonstersRepository>();
    }
    
    [Fact]
    public async Task Get_OnSuccess_ReturnsListOfMonsters()
    {
        Monster[] monsters = MonsterFixture.GetMonstersMock().ToArray();

        _repository
            .Setup(x => x.Monsters.GetAllAsync())
            .ReturnsAsync(monsters);

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult) result;
        objectResults?.Value.Should().BeOfType<Monster[]>();
    }

    [Fact]
    public async Task Get_OnSuccess_ReturnsOneMonsterById()
    {
        const int id = 1;
        Monster[] monsters = MonsterFixture.GetMonstersMock().ToArray();

        Monster monster = monsters[0];
        _repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(monster);

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Find(id);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
    }

    [Fact]
    public async Task Get_OnNoMonsterFound_Returns404()
    {
        const int id = 123;

        _repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(() => null);

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Find(id);
        NotFoundObjectResult objectResults = (NotFoundObjectResult)result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The monster with ID = {id} not found.", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_CreateMonster()
    {
        Monster m = new Monster()
        {
            Name = "Monster Test",
            Attack = 50,
            Defense = 40,
            Hp = 80,
            Speed = 60,
            ImageUrl = ""
        };

        _repository
            .Setup(x => x.Monsters.AddAsync(m));

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Add(m);
        OkObjectResult objectResults = (OkObjectResult)result;
        objectResults?.Value.Should().BeOfType<Monster>();
    }

    [Fact]
    public async Task Put_OnSuccess_UpdateMonster()
    {
        const int id = 1;
        Monster[] monsters = MonsterFixture.GetMonstersMock().ToArray();

        Monster m = new Monster()
        {
            Name = "Monster Update"
        };

        var updatedSavedMonster = new Monster();

        _repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(monsters[0]);

        _repository
           .Setup(x => x.Monsters.Update(monsters[0], m))
           .Callback<Monster, Monster>((existMon, newMon) => updatedSavedMonster = UpdateMonster(existMon, newMon));

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Update(id, m);
        OkResult objectResults = (OkResult)result;
        updatedSavedMonster.Id.Should().Be(id);
        updatedSavedMonster.Name.Should().Be(m.Name);
        objectResults.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Put_OnNoMonsterFound_Returns404()
    {
        const int id = 123;

        Monster m = new Monster()
        {
            Name = "Monster Update"
        };

        _repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(() => null);

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Update(id, m);
        NotFoundObjectResult objectResults = (NotFoundObjectResult)result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The monster with ID = {id} not found.", objectResults.Value);
    }


    [Fact]
    public async Task Delete_OnSuccess_RemoveMonster()
    {
        const int id = 1;
        Monster[] monsters = MonsterFixture.GetMonstersMock().ToArray();
        List<Monster> newMonsters = new List<Monster>();

        var deletedMonster = monsters[0];

        this._repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(deletedMonster);

        this._repository
            .Setup(x => x.Monsters.RemoveAsync(deletedMonster))
            .Callback<Monster>(mon => newMonsters = monsters.Where(mn => mn.Id != mon.Id).ToList());

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Remove(id);
        OkResult objectResults = (OkResult)result;
        newMonsters.Should().NotContain(deletedMonster);
        objectResults.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Delete_OnNoMonsterFound_Returns404()
    {
        const int id = 123;

        _repository
            .Setup(x => x.Monsters.FindAsync(id))
            .ReturnsAsync(() => null);

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.Remove(id);
        NotFoundObjectResult objectResults = (NotFoundObjectResult)result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal($"The monster with ID = {id} not found.", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_ImportCsvToMonster()
    {
        var file = GetFile("monsters-correct.csv");

        var monsters = new List<Monster>();

        _repository
            .Setup(x => x.Monsters.AddAsync(It.IsAny<IEnumerable<Monster>>()))
            .Callback<IEnumerable<Monster>>(monstersSaved => monsters.AddRange(monstersSaved));

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.ImportCsv(file);
        OkResult objectResults = (OkResult)result;
        objectResults.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Post_BadRequest_ImportCsv_With_Nonexistent_Monster()
    {
        var file = GetFile("monsters-empty-monster.csv");

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.ImportCsv(file);
        BadRequestObjectResult objectResults = (BadRequestObjectResult)result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Wrong data mapping.", objectResults.Value);
    }
    
    [Fact]
    public async Task Post_BadRequest_ImportCsv_With_Nonexistent_Column()
    {
        var file = GetFile("monsters-wrong-column.csv");

        MonsterController sut = new MonsterController(_repository.Object);

        ActionResult result = await sut.ImportCsv(file);
        BadRequestObjectResult objectResults = (BadRequestObjectResult)result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Wrong data mapping.", objectResults.Value);
    }

    #region Helpers
    private Monster UpdateMonster(Monster existMon, Monster newMon)
    {
        var returnedMonster = existMon;

        if (newMon.Defense != 0) returnedMonster.Defense = newMon.Defense;
        if (newMon.Hp != 0) returnedMonster.Hp = newMon.Hp;
        if (newMon.Speed != 0) returnedMonster.Speed = newMon.Speed;
        if (newMon.Attack != 0) returnedMonster.Attack = newMon.Attack;

        if (!string.IsNullOrEmpty(newMon.ImageUrl)) returnedMonster.ImageUrl = newMon.ImageUrl;
        if (!string.IsNullOrEmpty(newMon.Name)) returnedMonster.Name = newMon.Name;

        return returnedMonster;
    }

    private FormFile GetFile(string fileName)
    {
        var stream = File.OpenRead($"{System.AppContext.BaseDirectory}\\Files\\{fileName}");

        var file = new FormFile(stream, 0, stream.Length, $"stream_{stream.Name}", Path.GetFileName(stream.Name))
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };

        return file;
    }
    #endregion
}