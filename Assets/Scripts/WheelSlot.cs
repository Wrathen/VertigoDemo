using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSlot : MonoBehaviour
{
    public Image wheelIcon;
    public TextMeshProUGUI wheelDesc;

    public void SetWheelImage(Sprite icon)
    {
        if (wheelIcon == null)
        {
            Debug.LogError("Wheel icon reference is missing!");
            return;
        }

        wheelIcon.sprite = icon;
    }

    public void SetWheelDescription(string description)
    {
        if (wheelDesc == null)
        {
            Debug.LogError("Wheel description reference is missing!");
            return;
        }
        
        wheelDesc.text = description;
    }
}