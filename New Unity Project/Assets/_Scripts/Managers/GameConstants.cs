public static class GameConstants
{
    //public static float MELEE_DIST = .5f; //will we need/want this?
    //public static float RANGE_DIST = .9f;

    public static float ATTACK_READY_PERCENTAGE = .9f; //when is the attack set in stone? Once it is, it will happen even if the target is not within range, but within vision and in front of unit
    public static float ATTACK_CHARGE_LIMITER = .5f;   //To what point should the attack charge to when a unit is within vision, but not range, or not in front of the unit
    public static float MAXIMUM_ATTACK_ANGLE = 70.0f;  //to what extend can the unit be not rotated on target before the attack is delayed? Think airship
    public static float ROTATION_SPEED = 120.0f;       //How fast does a unit rotate when tracking a target, unit is in degrees/second

    public static float RESOURCE_SPEED = 0.75f;         //How fast the resource generates
    public static int RESOURCE_MAX = 9;                //The cap on the resource (9 means 10)
    public static int MAX_HAND_SIZE = 4;               //size of the player hand
    public static string HUD_CANVAS = "HUD - Canvas";      //The name of the gameObject that holds the UI

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

    public enum BUILDING_TYPE {
        ATTACK,
        SPAWN
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
