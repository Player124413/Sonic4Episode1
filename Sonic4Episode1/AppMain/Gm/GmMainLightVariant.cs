internal partial class AppMain
{
    // these two must stay above gm_main_light_variant: static field initialisers run in textual
    // order within a file, so a consumer declared first binds to null
    internal static readonly GMS_MAIN_LIGHT_VARIANT gm_main_light_variant_mobile = new GMS_MAIN_LIGHT_VARIANT
    {
        default_light_vec = new NNS_VECTOR(0.0f, 0.0f, -1f),

        player_light_follows_default = false,
        player_light_vec = new NNS_VECTOR(-0.5f, -0.4f, -0.25f),
        player_light_vec_zone_4 = new NNS_VECTOR(0.05f, 0.15f, -0.05f),

        player_intensity = 1.5f,
    };

    internal static readonly GMS_MAIN_LIGHT_VARIANT gm_main_light_variant_console = new GMS_MAIN_LIGHT_VARIANT
    {
        default_light_vec = new NNS_VECTOR(-1f, -1f, -1f),

        player_light_follows_default = true,

        player_intensity = 1f,
    };

    internal static GMS_MAIN_LIGHT_VARIANT gm_main_light_variant = gm_main_light_variant_mobile;

    // resolved once per stage beside gmMapSelectVariant, never per frame. Keyed on the map because
    // the blocks are the bulk of the lit geometry and the only lit thing that faces the camera head
    // on, which is what the mobile direction blows out.
    internal static void gmMainSelectLightVariant(int stage_id)
    {
        gm_main_light_variant = gmMapIsConsoleDataComplete(stage_id)
            ? gm_main_light_variant_console
            : gm_main_light_variant_mobile;
    }
}
