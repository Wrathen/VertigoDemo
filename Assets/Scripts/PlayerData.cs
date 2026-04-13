using System.Collections.Generic;

public struct PlayerData
{
    public int zone;
    public int gold;
    public int cash;
    public Dictionary<string, int> inventory; // This is fine for the demo, but on a real game we would want something better.
}