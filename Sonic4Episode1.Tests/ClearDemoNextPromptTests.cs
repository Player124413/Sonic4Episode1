using System.Collections.Generic;
using System.IO;

namespace Sonic4Episode1.Tests;

[Collection(AmaReaderCollection.Name)]
public class ClearDemoNextPromptTests
{
    private static AppMain.A2S_AMA_HEADER Read(string fixture)
    {
        var path = Path.Combine(
            Path.GetDirectoryName(typeof(ClearDemoNextPromptTests).Assembly.Location), "Fixtures", fixture);

        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        return AppMain.readAMAFile(reader);
    }

    [Fact]
    public void ThePcArchiveCarriesThePromptAtTheGatedIds()
    {
        var rslt = Read("G_RSLT_PC.AMA");
        var lang = Read("G_RSLT_US_PC.AMA");

        Assert.Equal(105u, rslt.act_num);
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_TBL_ACT_ID, rslt.FindAct("tbl01_l"));
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_TBL_ACT_ID + 1, rslt.FindAct("tbl01_c"));
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_TBL_ACT_ID + 2, rslt.FindAct("tbl01_r"));
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_BTN_KEY_ACT_ID, rslt.FindAct("btn_next_key"));
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_BTN_PAD_ACT_ID, rslt.FindAct("btn_next"));

        Assert.Equal(22u, lang.act_num);
        Assert.Equal(GmClearDemo.GMD_CLRDM_NEXT_TEX_ACT_ID, lang.FindAct("tex_next"));
    }

    // 100-105 exist on mobile too, as btn01_l/c/r and btn02_l/c/r, and its language archive stops
    // at 20. An id-only gate would create the touch button art as the prompt
    [Fact]
    public void TheMobileArchiveReachesTheSameIdsWithDifferentArt()
    {
        var rslt = Read("G_RSLT_MOBILE.AMA");
        var lang = Read("G_RSLT_US_MOBILE.AMA");

        Assert.True(rslt.act_num > GmClearDemo.GMD_CLRDM_NEXT_BTN_PAD_ACT_ID);
        Assert.Equal("btn01_l", rslt.act_names[GmClearDemo.GMD_CLRDM_NEXT_TBL_ACT_ID]);
        Assert.Equal(-1, rslt.FindAct("tbl01_l"));
        Assert.Equal(-1, rslt.FindAct("btn_next"));
        Assert.Equal(-1, rslt.FindAct("btn_next_key"));

        Assert.Equal(21u, lang.act_num);
        Assert.Equal(-1, lang.FindAct("tex_next"));
    }

    [Fact]
    public void TheSharedPrefixAgreesBetweenBuilds()
    {
        var pc = Read("G_RSLT_PC.AMA");
        var mobile = Read("G_RSLT_MOBILE.AMA");

        for (int id = 0; id < GmClearDemo.GMD_CLRDM_NEXT_TBL_ACT_ID; id++)
            Assert.Equal(mobile.act_names[id], pc.act_names[id]);
    }

    private static System.Func<int, bool> Cleared(params int[] stages)
    {
        var set = new HashSet<int>(stages);
        return stage => set.Contains(stage);
    }

    [Fact]
    public void NextStageIsTheFirstUnclearedActInTheZone()
    {
        Assert.Equal(1, AppMain.gmMainFirstUnclearedStage(0, Cleared(0)));
        Assert.Equal(3, AppMain.gmMainFirstUnclearedStage(1, Cleared(0, 1, 2)));
        Assert.Equal(6, AppMain.gmMainFirstUnclearedStage(4, Cleared(0, 1, 2, 3, 4, 5, 7)));
    }

    [Fact]
    public void NextStageFallsBackToTheFirstUnclearedActAnywhere()
    {
        Assert.Equal(8, AppMain.gmMainFirstUnclearedStage(3, Cleared(0, 1, 2, 3, 4, 5, 6, 7)));
        Assert.Equal(0, AppMain.gmMainFirstUnclearedStage(15, Cleared(12, 13, 14, 15)));
    }

    [Fact]
    public void NextStageIsEggStationOnceEveryActIsCleared()
    {
        var all = new int[AppMain.GSD_MAIN_STAGE_ID_FINAL_1];
        for (int i = 0; i < all.Length; i++)
            all[i] = i;

        Assert.Equal(AppMain.GSD_MAIN_STAGE_ID_FINAL_1, AppMain.gmMainFirstUnclearedStage(15, Cleared(all)));
        Assert.Equal(AppMain.GSD_MAIN_STAGE_ID_FINAL_1, AppMain.gmMainFirstUnclearedStage(AppMain.GSD_MAIN_STAGE_ID_FINAL_1, Cleared(all)));
    }
}
