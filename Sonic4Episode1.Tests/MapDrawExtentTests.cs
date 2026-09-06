using System;
using Xunit;

namespace Sonic4Episode1.Tests;

// The map draws a fixed count of 64 unit blocks centred on the camera's own block, so what it
// guarantees either side is (span >> 1) * 64 rather than half the span. SEGA sized the table
// against a 383.6 unit 16:9 view and hand-minimised the boss rows to 192 against a 191.8 half
// view, so every one of them runs out the moment the window gets wider.
// See docs/camera-view-extent.md.
public class MapDrawExtentTests
{
    private const float DispHeight = 720f;
    private const int BlockSize = AppMain.GMD_MAP_BLOCK_SIZE;

    private const float SixteenNine = 1280f / 720f;
    private const float TwentyOneNine = 2560f / 1080f;
    private const float Ultrawide = 3440f / 1440f;
    private const float ZFlip4 = 1920f / 785f;

    // GmMain.cs:16, evaluated without needing the globals GSD_DISP_HEIGHT reads through
    private static int Lcd(float aspect) => (short)(AppMain.GMD_CAMERA_SCALE * DispHeight * aspect);

    private static float HalfView(float aspect) => AppMain.GMD_CAMERA_SCALE * DispHeight * aspect * 0.5f;

    private static int Rows => AppMain.gm_map_set_draw_size.GetLength(0);

    private static int Covered(int span) => (span >> 1) * BlockSize;

    [Theory]
    [InlineData(SixteenNine)]
    [InlineData(TwentyOneNine)]
    [InlineData(Ultrawide)]
    [InlineData(ZFlip4)]
    public void EveryDrawSizeRowCoversTheViewHorizontally(float aspect)
    {
        for (int row = 0; row < Rows; row++)
        {
            int width = (int)AppMain.gm_map_set_draw_size[row, 0];
            int span = AppMain.GmMapGetDrawBlockSpan(width, 0, Lcd(aspect));

            Assert.True(Covered(span) >= HalfView(aspect),
                $"row {row} width {width} covers {Covered(span)} against a half view of {HalfView(aspect)}");
        }
    }

    // OBD_LCD_Y has no aspect term, so one window covers every window
    [Fact]
    public void EveryDrawSizeRowCoversTheViewVertically()
    {
        for (int row = 0; row < Rows; row++)
        {
            int height = (int)AppMain.gm_map_set_draw_size[row, 1];
            int span = AppMain.GmMapGetDrawBlockSpan(height, 0, Lcd(1f));

            Assert.True(Covered(span) >= HalfView(1f),
                $"row {row} height {height} covers {Covered(span)} against a half view of {HalfView(1f)}");
        }
    }

    // the no-op proof. widening must not move 16:9 by a single block, or it is not a fix, it is a
    // reframe of the whole game
    [Fact]
    public void SixteenNineIsUnchangedFromTheAuthoredTable()
    {
        for (int row = 0; row < Rows; row++)
        {
            int width = (int)AppMain.gm_map_set_draw_size[row, 0];
            int height = (int)AppMain.gm_map_set_draw_size[row, 1];

            Assert.Equal(width + AppMain.GMD_MAP_DRAW_MARGIN * 2,
                AppMain.GmMapGetDrawBlockSpan(width, 0, Lcd(SixteenNine)));
            Assert.Equal(height + AppMain.GMD_MAP_DRAW_MARGIN * 2,
                AppMain.GmMapGetDrawBlockSpan(height, 0, Lcd(1f)));
        }
    }

    [Fact]
    public void TheMarginAdjustStillWidensWhenTheAspectTermIsNotBinding()
    {
        int width = AppMain.GMD_MAP_DRAW_WIDTH_DEF;
        int lcd = Lcd(SixteenNine);

        Assert.Equal(width + 2, AppMain.GmMapGetDrawBlockSpan(width, 0, lcd));
        Assert.Equal(width + 4, AppMain.GmMapGetDrawBlockSpan(width, 1, lcd));
    }

    // the boss rows are the three live cases: 4 wide covers 192 against 191.8 and nothing more
    [Fact]
    public void TheBossRowsWereAtTheLimitAndNowAreNot()
    {
        int lcd_wide = Lcd(TwentyOneNine);
        int authored = 4 + AppMain.GMD_MAP_DRAW_MARGIN * 2;

        Assert.True(Covered(authored) < HalfView(TwentyOneNine));
        Assert.True(Covered(AppMain.GmMapGetDrawBlockSpan(4, 0, lcd_wide)) >= HalfView(TwentyOneNine));
    }

    // gm_map_block_check is indexed DF_SIZE + span + ofst, and the aspect term can push span past
    // the table, so what keeps the buffer big enough is mppAspectCap rather than the table
    [Fact]
    public void TheBlockCheckBufferCoversTheWidestWindowTheAspectCapAllows()
    {
        const int maxOfst = 7;

        int widest = 0;
        for (int row = 0; row < Rows; row++)
            widest = Math.Max(widest, (int)AppMain.gm_map_set_draw_size[row, 0]);

        int lcd = Lcd(AppMain.mppAspectCap);
        int span = AppMain.GmMapGetDrawBlockSpan(widest, AppMain.GMD_MAP_DRAW_MARGIN_ADJUST_MAX, lcd);

        Assert.True(AppMain.GMD_MAP_DRAW_DF_SIZE + span + maxOfst < AppMain.GMD_MAP_DRAW_CHECK_WIDTH,
            $"span {span} at the {AppMain.mppAspectCap} cap indexes "
            + $"{AppMain.GMD_MAP_DRAW_DF_SIZE + span + maxOfst} into {AppMain.GMD_MAP_DRAW_CHECK_WIDTH}");
    }

    // scrl_scale is the ratio of scrollable travel, so both layers reach their far edge together.
    // OBD_LCD_X tracks the window now, and a layer narrower than the view used to come out
    // negative. it must not do that, and it must not come out zero either: the layer camera is
    // half + (main - half) * scrl_scale, so zero pins the layer and the player walks off it
    [Fact]
    public void ANarrowBackgroundLayerStillTracksTheCamera()
    {
        int lcd = Lcd(TwentyOneNine);
        int narrow = 7 * BlockSize;
        int main = 200 * BlockSize;

        Assert.True(narrow < lcd, "the fixture needs a layer narrower than the view");
        Assert.Equal(narrow / (float)main, AppMain.GmMapCalcScrlScale(narrow, main, lcd), 5);
    }

    [Fact]
    public void AWideBackgroundLayerKeepsTheAuthoredRatio()
    {
        int lcd = Lcd(SixteenNine);
        int layer = 40 * BlockSize;
        int main = 200 * BlockSize;

        Assert.Equal((layer - lcd) / (float)(main - lcd), AppMain.GmMapCalcScrlScale(layer, main, lcd), 5);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(64, 0)]
    [InlineData(64, 64)]
    [InlineData(12800, 64)]
    public void ADegenerateMapNeverProducesANegativeOrInfiniteRate(int layer, int main)
    {
        float scale = AppMain.GmMapCalcScrlScale(layer, main, Lcd(TwentyOneNine));

        Assert.True(scale >= 0f, $"{scale} is negative");
        Assert.False(float.IsNaN(scale) || float.IsInfinity(scale), $"{scale} is not finite");
    }
}
