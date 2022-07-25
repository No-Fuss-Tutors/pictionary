using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    //Total rounds in the game
    public static int rounds = 3;
    //Seconds to draw
    public static int seconds = 60;

    public void ApplySettings(int newRounds, int newSeconds)
    {
        rounds = newRounds;
        seconds = newSeconds;
    }
}
