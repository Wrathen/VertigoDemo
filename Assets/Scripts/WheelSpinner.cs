using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WheelSpinner : MonoBehaviour
{
    [Header("Settings")]
    public Transform wheelTransform;
    public Ease spinEase = Ease.OutQuart;
    public float spinDuration = 5.0f;
    public bool antiClockwise = false;

    private Button spinButton;
    private bool isSpinning = false;
    private bool isSpinButtonEnabled = true;

    void OnValidate()
    {
        FindSpinButton();
    }

    void Awake()
    {
        // In builds, the OnValidate doesn't work, so we find the button here as well.
        // To be honest, I'd just assign it through the editor, but this was required in the docs.
        // In the UIManager, I did it the old-fashion way.
        FindSpinButton();
        if (spinButton == null)
        {
            Debug.LogError("Spin Button is not assigned.");
            return;
        }

        spinButton.onClick.AddListener(StartSpin);

        GameManager.Instance.OnBomb.AddListener(DisableSpinButton);
        GameManager.Instance.OnGameEnd.AddListener(DisableSpinButton);
        GameManager.Instance.OnPlayerRevive.AddListener(EnableSpinButton);
        GameManager.Instance.OnGameStart.AddListener(EnableSpinButton);
    }

    void FindSpinButton()
    {
        if (spinButton == null)
        {
            spinButton = GetComponentInChildren<Button>();
            if (spinButton == null)
            {
                Debug.LogError("Spin Button not found in children.");
            }
        }
    }

    void DisableSpinButton()
    {
        isSpinButtonEnabled = false;
        spinButton.interactable = false;
    }

    void EnableSpinButton()
    {
        isSpinButtonEnabled = true;
        spinButton.interactable = true;
    }

    void StartSpin()
    {
        if (!isSpinButtonEnabled)
        {
            Debug.LogWarning("Spin Button is currently disabled.");
            return;
        }
        if (isSpinning)
        {
            Debug.LogWarning("Start Spin failed. Already spinning.");
            return;
        }

        if (wheelTransform == null)
        {
            Debug.LogError("Wheel Transform is not assigned.");
            return;
        }
        
        // Lets test the distribution of weighted random -- DEBUG
        /*List<SpinResult> testResults = new List<SpinResult>();
        for (int i = 0; i < 1000; ++i)
        {
            result = GetRandomSpinResult();
            testResults.Add(result);
        }

        Dictionary<string, int> distribution = new Dictionary<string, int>();
        foreach (var r in testResults)
        {
            if (!distribution.ContainsKey(r.slotConfig.prizeName))
            {
                distribution[r.slotConfig.prizeName] = 0;
            }
            distribution[r.slotConfig.prizeName]++;
        }
        foreach (var kvp in distribution)
        {
            Debug.Log($"Prize: {kvp.Key}, Count: {kvp.Value}");
        }*/

        SpinResult result = GetRandomSpinResult();
        float finalAngle = (result.fullSpins * 8 + result.roll) * 45f;
        finalAngle = antiClockwise ? -finalAngle : finalAngle;

        Debug.Log("Spin Result: " + result.slotConfig.prizeName + " (Value: " + result.slotConfig.value + ")");

        isSpinning = true;
        wheelTransform.DORotate(new Vector3(0, 0, finalAngle), spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase)
            .OnComplete(() => {
                isSpinning = false;
                spinButton.interactable = true;

                GameManager.Instance.OnSpinResult(result);
            });

        spinButton.interactable = false;
    }

    SpinResult GetRandomSpinResult()
    {
        SpinResult result = new SpinResult();
        Wheel wheel = GetComponent<Wheel>();
        if (wheel == null)
        {
            Debug.LogError("Wheel component is missing on WheelSpinner.");
            return result;
        }

        // The house always wins :p
        var slotConfig = wheel.GetRandomSlotConfigWeighted();
        result.slotConfig = slotConfig;

        result.fullSpins = Random.Range(4, 8);
        result.roll = wheel.GetIndexBySlotConfig(slotConfig);

        return result;
    }
}