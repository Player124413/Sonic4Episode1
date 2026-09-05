internal partial class AppMain
{
    internal sealed class GMS_MAIN_LIGHT_VARIANT
    {
        public NNS_VECTOR default_light_vec;

        // console has no _IPHONE block to overwrite light_vec before the player call, so light 6
        // inherits light 0's direction and the two vectors below go unread
        public bool player_light_follows_default;
        public NNS_VECTOR player_light_vec;
        public NNS_VECTOR player_light_vec_zone_4;

        public float player_intensity;
    }
}
