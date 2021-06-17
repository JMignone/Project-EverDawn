public static class GameConstants
{
    //public static float MELEE_DIST = .5f; //will we need/want this?
    //public static float RANGE_DIST = .9f;

    public static float ATTACK_READY_PERCENTAGE = .9f; //when is the attack set in stone? 90% through delay? Set smaller to be able to test easier
    public static float MAXIMUM_ATTACK_ANGLE = 70.0f;  //to what extend can the unit be not rotated on target before the attack is delayed? Think airship
    public static float ROTATION_SPEED = 120.0f;       //How fast does a unit rotate when tracking a target, unit is in degrees/second

    public enum MOVEMENT_TYPE {
        GROUND,
        FLYING
    }

    public enum OBJECT_ATTACKABLE {
        GROUND,
        FLYING,
        BOTH
    }

    public enum UNIT_TYPE {
        UNIT,
        STRUCTURE
    }

    public enum ATTACK_PRIORITY {
        EVERYTHING,
        STRUCTURE
    }

    public enum UNIT_RANGE {
        MELEE,
        RANGE
    }
}
