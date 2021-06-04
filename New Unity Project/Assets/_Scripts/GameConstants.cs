public static class GameConstants
{
    public static float MELEE_DIST = .5f; //will we need/want this?
    public static float RANGE_DIST = .9f;

    public enum OBJECT_TYPE {
        GROUND,
        FLYING
    }

    public enum OBJECT_ATTACKABLE {
        GROUND,
        FLYING,
        BOTH
    }

    public enum UNIT_RANGE {
        MELEE,
        RANGE
    }
}
