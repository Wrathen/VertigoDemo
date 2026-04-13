using UnityEngine;

[CreateAssetMenu(fileName = "NewWheelData", menuName = "Wheel/Wheel Data")]
public class WheelData : ScriptableObject
{
    public string wheelName;
    public SlotConfig[] slots; // Array of 8 slots
}