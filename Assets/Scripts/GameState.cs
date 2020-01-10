using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static int coinScore = 0;

    //magnet power
    static bool magnetPower = false;
    static float magnetPowerDuration = 10.0f;
    public static float magnetRadius = 10.0f;
    public static float totalMagnetTime = 0.0f;

    //2x power
    static bool twoXPower = false;
    static float twoXPowerDuration = 10.0f;
    public static float total2XPowerTime = 0.0f;

    //reverse keys disadvantage
    static bool reverseKeys = false;
    public static float totalReverseKeysTime = 0.0f;
    static float reverseKeysDuration = 10.0f;

    //speed of platform (meaning speed of player)
    public static float moveSpeedPlatform = 13.0f;
    public static float begginingMoveSpeedPlatform = 13.0f;
    //speed of moving player left and right, it increases at the same rate as moving upward
    public static float leftRightSpeed = 7.0f;
    public static float beginningLeftRightSpeed = 7.0f;

    //Raketa
    static bool rocketPower = false;
    public static float totalRocketPowerTime = 0.0f;
    static float rocketPowerDuration = 10.0f;
    public static float hasJumpedWithRocketStartTime = 0.0f;
    public static bool hasJumpedWithRocket = false;

    public static bool deadByCollision = false;
    public static bool deadByExplosion = false;


    public static void collectCoin()
    {
        if (twoXPower)
        {
            coinScore += 2;
        }
        else
        {
            coinScore += 1;
        }

    }

    public static void enableMagnet()
    {
        magnetPower = true;
        totalMagnetTime = magnetPowerDuration;
    }

    public static void disableMagnet()
    {
        magnetPower = false;
        totalMagnetTime = 0.0f;
    }

    public static void enable2XPower()
    {
        twoXPower = true;
        total2XPowerTime = twoXPowerDuration;
    }

    public static void disable2XPower()
    {
        twoXPower = false;
        total2XPowerTime = 0.0f;
    }

    public static void enableReverseKeys()
    {
        reverseKeys = true;
        totalReverseKeysTime = reverseKeysDuration;
    }

    public static void disableReverseKeys()
    {
        reverseKeys = false;
        totalReverseKeysTime = 0.0f;
    }

    public static void enableRocketPower()
    {
        rocketPower = true;
        totalRocketPowerTime = rocketPowerDuration;
    }

    public static void disableRocketPower()
    {
        rocketPower = false;
        totalRocketPowerTime = 0.0f;
    }

    public static bool has2XPower()
    {
        return (twoXPower && total2XPowerTime > 0.0f);
    }

    public static bool hasMagnetPower()
    {
        return (magnetPower && totalMagnetTime > 0.0f);
    }

    public static bool hasReverseKeys()
    {
        return (reverseKeys && totalReverseKeysTime > 0.0f);
    }

    public static bool hasRocketPower()
    {
        return (rocketPower && totalRocketPowerTime > 0.0f);
    }

    public static void restartGame()
    {
        //Ponastavimo score
        coinScore = 0;
        //Izničimo vse power upe in njihove čase
        totalMagnetTime = 0.0f;
        magnetPower = false;
        twoXPower = false;
        total2XPowerTime = 0.0f;
        reverseKeys = false;
        totalReverseKeysTime = 0.0f;
        rocketPower = false;
        totalRocketPowerTime = 0.0f;
        //Ponastavimo hitrost na začetno
        moveSpeedPlatform = begginingMoveSpeedPlatform;
        leftRightSpeed = beginningLeftRightSpeed;
        deadByCollision = false;
        deadByExplosion = false;
        //ponastavimo gravitacijo
        Physics.gravity = new Vector3(Physics.gravity.x, -40.0f, Physics.gravity.z);
    }
}
