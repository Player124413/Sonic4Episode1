namespace Sonic4Episode1.Tests;

// dmStgSlctGetRevisedZoneNo is what L/R, the shoulder buttons, the touch arrows and a horizontal
// flick all go through, and its four open-state cases wrap differently. Pure, so worth pinning.
public class StgSlctZoneWrapTests
{
    private const int Zone1 = 0;
    private const int Zone4 = 3;
    private const int Final = 4;
    private const int Spe = 5;

    private static int Wrap(int from, int diff, bool final_open, bool spe_open)
    {
        return StgSlct.RevisedZoneNoForTest(from, diff, final_open ? 1 : 0, spe_open ? 1 : 0);
    }

    [Theory]
    [InlineData(Zone1, +1, 1)]
    [InlineData(Zone4, -1, 2)]
    [InlineData(Zone1, -1, Zone4)]
    [InlineData(Zone4, +1, Zone1)]
    public void NothingOpenWrapsWithinTheFourZones(int from, int diff, int expected)
    {
        Assert.Equal(expected, Wrap(from, diff, final_open: false, spe_open: false));
    }

    [Theory]
    [InlineData(Zone4, +1, Final)]
    [InlineData(Final, +1, Zone1)]
    [InlineData(Zone1, -1, Final)]
    [InlineData(Final, -1, Zone4)]
    public void FinalOpenAddsAFifthZone(int from, int diff, int expected)
    {
        Assert.Equal(expected, Wrap(from, diff, final_open: true, spe_open: false));
    }

    // final is closed here, so its slot is stepped over rather than landed on
    [Theory]
    [InlineData(Zone4, +1, Spe)]
    [InlineData(Spe, -1, Zone4)]
    [InlineData(Spe, +1, Zone1)]
    [InlineData(Zone1, -1, Spe)]
    public void SpecialOpenSkipsTheFinalSlot(int from, int diff, int expected)
    {
        Assert.Equal(expected, Wrap(from, diff, final_open: false, spe_open: true));
        Assert.NotEqual(Final, Wrap(from, diff, final_open: false, spe_open: true));
    }

    [Theory]
    [InlineData(Zone4, +1, Final)]
    [InlineData(Final, +1, Spe)]
    [InlineData(Spe, +1, Zone1)]
    [InlineData(Zone1, -1, Spe)]
    [InlineData(Final, -1, Zone4)]
    public void AllOpenWalksEveryZone(int from, int diff, int expected)
    {
        Assert.Equal(expected, Wrap(from, diff, final_open: true, spe_open: true));
    }

    [Theory]
    [InlineData(false, false, 4)]
    [InlineData(true, false, 5)]
    [InlineData(false, true, 5)]
    [InlineData(true, true, 6)]
    public void SteppingForwardVisitsEveryReachableZoneExactlyOnce(
        bool final_open, bool spe_open, int expected_cycle)
    {
        var seen = new System.Collections.Generic.HashSet<int>();
        var zone = Zone1;

        for (int i = 0; i < expected_cycle; i++)
        {
            Assert.True(seen.Add(zone), $"zone {zone} visited twice");
            zone = Wrap(zone, +1, final_open, spe_open);
        }

        Assert.Equal(Zone1, zone);
        Assert.Equal(expected_cycle, seen.Count);
    }
}
