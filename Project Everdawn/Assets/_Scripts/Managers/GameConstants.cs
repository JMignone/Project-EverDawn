public static class GameConstants
{
    public static float ATTACK_READY_PERCENTAGE = .75f; //when is the attack set in stone? Once it is, it will happen even if the target is not within range, but within vision and in front of unit
    public static float ATTACK_CHARGE_LIMITER = .5f;   //To what point should the attack charge to when a unit is within vision, but not range, or not in front of the unit
    public static float MAXIMUM_ATTACK_ANGLE = 70.0f;  //to what extend can the unit be not rotated on target before the attack is delayed? Think airship

    public static float RESOURCE_SPEED = 0.33f;             //How fast the resource generates
    public static int RESOURCE_MAX = 9;                     //The cap on the resource (9 means 10)
    public static int MAX_HAND_SIZE = 4;                    //size of the player hand
    public static string HUD_CANVAS = "HUD - Canvas";       //The name of the gameObject that holds the UI
    public static string PLAYER_TAG = "Player";

    public static int FLY_ZONE_HEIGHT = 20; //The distance of the fly zone above the ground

    public enum MOVEMENT_TYPE {
        GROUND,
        FLYING
    }

    public enum HEIGHT_ATTACKABLE {
        BOTH,
        GROUND,
        FLYING
    }

    public enum TYPE_ATTACKABLE {
        BOTH,
        UNIT,
        STRUCTURE
    }

    public enum UNIT_TYPE {
        UNIT,
        STRUCTURE
    }

    public enum UNIT_GROUPING {
        SOLO,
        GROUP
    }

    public enum BUILDING_TYPE {
        ATTACK,
        SPAWN
    }

    public enum BUILDING_SIZE {
        SMALL,
        BIG
    }

    public enum SUMMON_SIZE {
        NONE,
        SMALL,
        BIG
    }

    public enum ATTACK_PRIORITY {
        EVERYTHING,
        STRUCTURE
    }

    public enum UNIT_RANGE {
        MELEE,
        RANGE
    }

    //The start location an airstrike should take
    public enum AIR_STRIKE_LOCATION {
        BOTTOM,
        SIDE
    }

    //The new location an ability will fire to via LocationStats
    public enum NEW_ABILITY_LOCATION {
        ON_ABILITY,
        HALFWAY,
        ON_UNIT
    }

    //What a unit should do in the event its target dies while mid firing a volley of projectiles
    public enum CONTINUE_FIRING_TYPE {
        NONE,
        SAMELOCATION,
        RETARGET
    }

    //How should a projectile behave when fired from a unit
    public enum FIRING_TYPE {
        TARGET,
        ATTACKSLOCATION,
        ATTACKSPAST
    }

    //How much of the red spawn zone should be shown when placing cards
    public enum SPAWN_ZONE_RESTRICTION {
        FULL,
        HALF,
        NONE
    }
}
