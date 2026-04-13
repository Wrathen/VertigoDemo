using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private WheelData wheelData;

    [SerializeField]
    private List<GameObject> wheelSlots;

    void OnValidate()
    {
        if (wheelData == null)
        {
            Debug.LogError("WheelData reference is missing!");
            return;
        }

        UpdateSlots();
    }

    public void SetWheelData(WheelData newData)
    {
        wheelData = newData;
        UpdateSlots();
    }

    void UpdateSlots()
    {
        for (int i = 0; i < wheelSlots.Count; ++i)
        {
            GameObject slot = wheelSlots[i];
            WheelSlot wheelSlot = slot.GetComponent<WheelSlot>();
            if (wheelSlot == null)
            {
                Debug.LogError("WheelSlot component is missing on slot: " + slot.name);
                continue;
            }

            if (i < wheelData.slots.Count())
            {
                SlotConfig slotConfig = wheelData.slots[i];
                wheelSlot.SetWheelImage(slotConfig.icon);
                wheelSlot.SetWheelDescription(slotConfig.descText);
            }
            else
            {
                Debug.LogError("Not enough slot data for slot index: " + i);
                wheelSlot.SetWheelImage(null);
                wheelSlot.SetWheelDescription("N/A");
            }
        }
    }

    public SlotConfig GetSlotConfigByIndex(int index)
    {
        if (wheelData == null || index < 0 || index >= wheelData.slots.Length)
        {
            Debug.LogError("Invalid slot index: " + index);
            return null;
        }

        return wheelData.slots[index];
    }
    public int GetIndexBySlotConfig(SlotConfig config)
    {
        if (wheelData == null || config == null)
        {
            Debug.LogError("WheelData or SlotConfig is null.");
            return -1;
        }

        for (int i = 0; i < wheelData.slots.Length; ++i)
        {
            if (wheelData.slots[i] == config)
            {
                return i;
            }
        }

        Debug.LogError("SlotConfig not found in WheelData.");
        return -1;
    }

    public SlotConfig GetRandomSlotConfig()
    {
        if (wheelData == null || wheelData.slots == null || wheelData.slots.Length == 0)
        {
            Debug.LogError("WheelData or slots are not properly set up.");
            return null;
        }

        return wheelData.slots[Random.Range(0, wheelData.slots.Length)];
    }

    // There are better algorithms for weighted random, but this is totally fine for our case.
    public SlotConfig GetRandomSlotConfigWeighted()
    {
        int totalWeight = 0;
        foreach (var slot in wheelData.slots)
        {
            totalWeight += slot.weight;
        }

        int randomValue = Random.Range(0, totalWeight);

        int cursor = 0;
        for (int i = 0; i < wheelData.slots.Length; ++i)
        {
            cursor += wheelData.slots[i].weight;
            if (randomValue < cursor)
            {
                return wheelData.slots[i];
            }
        }

        // should never happen
        return null;
    }
}