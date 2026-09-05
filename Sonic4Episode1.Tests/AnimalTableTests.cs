using System.Linq;

namespace Sonic4Episode1.Tests;

public class AnimalTableTests
{
    private const int Pig = 0;
    private const int Bird = 1;
    private const int Penguin = 2;
    private const int Rabbit = 3;
    private const int Chicken = 4;

    [Theory]
    [InlineData(Pig, 0)]
    [InlineData(Bird, 1)]
    [InlineData(Penguin, 4)]
    [InlineData(Rabbit, 3)]
    [InlineData(Chicken, 2)]
    public void TheModelIndexTableMatchesTheConsoleMemberOrder(int type, int member)
    {
        Assert.Equal(member, AppMain.gm_gmk_animal_console_obj_id[type]);
    }

    [Theory]
    [InlineData(Pig, 0, 1, 2)]
    [InlineData(Bird, 3, 4, 5)]
    [InlineData(Penguin, 12, 13, 14)]
    [InlineData(Rabbit, 9, 10, 11)]
    [InlineData(Chicken, 6, 7, 8)]
    public void TheMotionIdTableMatchesTheConsoleMemberOrder(int type, int st, int up, int down)
    {
        var ids = AppMain.gm_gmk_animal_console_mtn_id[type];

        Assert.Equal(st, ids[0]);
        Assert.Equal(up, ids[1]);
        Assert.Equal(down, ids[2]);
    }

    [Fact]
    public void EveryModelAndMotionIsNamedExactlyOnce()
    {
        Assert.Equal(5, AppMain.gm_gmk_animal_console_obj_id.Length);
        Assert.Equal(
            new[] { 0, 1, 2, 3, 4 },
            AppMain.gm_gmk_animal_console_obj_id.OrderBy(id => id).ToArray());

        var motions = AppMain.gm_gmk_animal_console_mtn_id.SelectMany(ids => ids).OrderBy(id => id);
        Assert.Equal(Enumerable.Range(0, 15).ToArray(), motions.ToArray());
    }
}
