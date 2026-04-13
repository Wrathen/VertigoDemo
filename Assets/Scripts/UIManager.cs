using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject bombScreen;
    public GameObject endScreen;
    public GameObject safeZoneFlashEffect;
    public GameObject superZoneFlashEffect;

    [Header("Buttons")]
    [SerializeField]
    private Button leaveButton;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button giveupButton;
    [SerializeField]
    private Button reviveButtonGold;
    [SerializeField]
    private Button reviveButtonAds;

    [Header("Stats Screen")]
    [SerializeField]
    private TextMeshProUGUI zoneText;
    [SerializeField]
    private TextMeshProUGUI goldText;
    [SerializeField]
    private TextMeshProUGUI cashText;
    [SerializeField]
    private TextMeshProUGUI inventoryText;

    [Header("End Screen")]
    [SerializeField]
    private TextMeshProUGUI endscreenZoneText;
    [SerializeField]
    private TextMeshProUGUI endscreenGoldText;
    [SerializeField]
    private TextMeshProUGUI endscreenCashText;
    [SerializeField]
    private TextMeshProUGUI endscreenInventoryText;

    private void Start()
    {
        if (bombScreen == null)
        {
            Debug.LogError("End Game Screen reference is missing in UIManager!");
            return;
        }

        GameManager.Instance.OnGameStart.AddListener(HideBombScreen);
        GameManager.Instance.OnPlayerRevive.AddListener(HideBombScreen);
        GameManager.Instance.OnBomb.AddListener(ShowBombScreen);

        GameManager.Instance.OnGameStart.AddListener(UpdateUI);
        GameManager.Instance.OnPlayerRevive.AddListener(UpdateUI);
        GameManager.Instance.OnRewardClaimed.AddListener(UpdateUI);

        GameManager.Instance.OnGameStart.AddListener(HideEndScreen);
        GameManager.Instance.OnGameEnd.AddListener(ShowEndScreen);
        GameManager.Instance.OnGameEnd.AddListener(UpdateEndScreen);

        GameManager.Instance.OnZoneChanged.AddListener(UpdateZoneEffects);
        GameManager.Instance.OnZoneChanged.AddListener(UpdateLeaveButtonState);

        giveupButton.onClick.AddListener(OnGiveupButtonClicked);
        reviveButtonGold.onClick.AddListener(OnReviveButtonGoldClicked);
        reviveButtonAds.onClick.AddListener(OnReviveButtonAdsClicked);

        restartButton.onClick.AddListener(() => {
            GameManager.Instance.StartNewGame();
        });
        leaveButton.onClick.AddListener(() => {
            GameManager.Instance.EndGame();
        });
    }

    void LogErrorIfNull(object elementName)
    {
        if (elementName == null)
        {
            Debug.LogError(elementName + " reference is missing in UIManager!");
        }
    }

    // In the demo docs it is stated to find the buttons in OnValidate
    // I will manually assign them in the inspector, honestly this is a design choice here.
    void OnValidate()
    {
        LogErrorIfNull(endScreen);
        LogErrorIfNull(bombScreen);
        LogErrorIfNull(leaveButton);
        LogErrorIfNull(giveupButton);
        LogErrorIfNull(reviveButtonGold);
        LogErrorIfNull(reviveButtonAds);
        LogErrorIfNull(zoneText);
        LogErrorIfNull(goldText);
        LogErrorIfNull(cashText);
        LogErrorIfNull(inventoryText);
        LogErrorIfNull(endscreenZoneText);
        LogErrorIfNull(endscreenGoldText);
        LogErrorIfNull(endscreenCashText);
        LogErrorIfNull(endscreenInventoryText);
        LogErrorIfNull(restartButton);
        LogErrorIfNull(safeZoneFlashEffect);
        LogErrorIfNull(superZoneFlashEffect);
    }

    private void ShowBombScreen()
    {
        bombScreen.SetActive(true);
    }
    private void HideBombScreen()
    {
        bombScreen.SetActive(false);
    }

    private void ShowEndScreen()
    {
        endScreen.SetActive(true);
    }

    private void HideEndScreen()
    {
        endScreen.SetActive(false);
    }

    public void OnGiveupButtonClicked()
    {
        GameManager.Instance.EndGame();
    }

    public void OnReviveButtonGoldClicked()
    {
        GameManager.Instance.RevivePlayerWithGold();
    }

    public void OnReviveButtonAdsClicked()
    {
        GameManager.Instance.RevivePlayerWithAds();
    }

    public void UpdateUI()
    {
        PlayerData data = GameState.GetPlayerData();
        zoneText.text = "Zone: " + data.zone;
        goldText.text = "Gold: " + data.gold;
        cashText.text = "Cash: " + data.cash;

        string inventoryStr = "Inventory:\n";
        foreach (var item in data.inventory)
        {
            inventoryStr += $"{item.Key}: {item.Value}\n";
        }
        inventoryText.text = inventoryStr;
    }

    public void UpdateEndScreen()
    {
        PlayerData data = GameState.GetPlayerData();
        endscreenZoneText.text = "Zone: " + data.zone;
        endscreenGoldText.text = "Gold: " + data.gold;
        endscreenCashText.text = "Cash: " + data.cash;

        string inventoryStr = "Inventory\n";
        foreach (var item in data.inventory)
        {
            inventoryStr += $"{item.Key}: {item.Value}\n";
        }
        endscreenInventoryText.text = inventoryStr;
    }

    public void UpdateZoneEffects()
    {
        ZoneType zoneType = GameManager.Instance.GetZoneType(GameState.GetZone());
        safeZoneFlashEffect.SetActive(false);
        superZoneFlashEffect.SetActive(false);

        if (zoneType == ZoneType.Safe)
        {
            safeZoneFlashEffect.SetActive(true);
        }
        else if (zoneType == ZoneType.Super)
        {
            superZoneFlashEffect.SetActive(true);
        }
    }

    public void UpdateLeaveButtonState()
    {
        ZoneType zoneType = GameManager.Instance.GetZoneType(GameState.GetZone());
        if (zoneType == ZoneType.Safe || zoneType == ZoneType.Super)
        {
            leaveButton.gameObject.SetActive(true);
        }
        else
        {
            leaveButton.gameObject.SetActive(false);
        }
    }
}