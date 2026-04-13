using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Settings")]
    public int GoldCostToRevive = 25;
    
    private Wheel wheel;
    [SerializeField]
    private WheelData normalZoneWheelData;
    [SerializeField]
    private WheelData safeZoneWheelData;
    [SerializeField]
    private WheelData superZoneWheelData;

    [HideInInspector]
    public UnityEvent OnGameStart;
    [HideInInspector]
    public UnityEvent OnGameEnd; // not used atm, but might be used in the future.
    [HideInInspector]
    public UnityEvent OnPlayerRevive;
    [HideInInspector]
    public UnityEvent OnBomb;
    [HideInInspector]
    public UnityEvent OnRewardClaimed;
    [HideInInspector]
    public UnityEvent OnZoneChanged;
    [HideInInspector]
    public UnityEvent OnSpinStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Init();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (normalZoneWheelData == null || 
            safeZoneWheelData == null   || 
            superZoneWheelData == null)
        {
            Debug.LogError("One or more WheelData references are missing in GameManager!");
        }

        FindWheel();
    }

    private void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        FindWheel();
    }

    void FindWheel()
    {
        if (wheel == null)
        {
            wheel = FindObjectOfType<Wheel>();
            if (wheel == null)
            {
                Debug.LogError("Wheel component not found in the scene!");
            }
        }
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        Debug.Log("Starting a new game...");
        
        GameState.SetPlayerData(new PlayerData
        {
            zone = 0,
            gold = 0,
            cash = 0,
            inventory = new Dictionary<string, int>()
        });

        AdvanceZone();
        OnGameStart?.Invoke();
    }

    private void OnBombDrawn()
    {
        OnBomb?.Invoke();
    }
    public void EndGame()
    {
        OnGameEnd?.Invoke();
    }

    public ZoneType GetZoneType(int zone)
    {
        if (zone % 30 == 0)
        {
            return ZoneType.Super;
        }
        if (zone % 5 == 0)
        {
            return ZoneType.Safe;
        }

        return ZoneType.Normal;
    }
    public void AdvanceZone()
    {
        if (wheel == null)
        {
            Debug.LogError("Wheel component not found in the scene!");
            return;
        }

        GameState.IncrementZone();
        int zone = GameState.GetZone();
        ZoneType zoneType = GetZoneType(zone);

        switch (zoneType)
        {
            case ZoneType.Normal:
                wheel.SetWheelData(normalZoneWheelData);
                break;
            case ZoneType.Safe:
                wheel.SetWheelData(safeZoneWheelData);
                break;
            case ZoneType.Super:
                wheel.SetWheelData(superZoneWheelData);
                break;
            default:
                Debug.LogError($"Unknown zone type: {zoneType}");
                return;
        }

        OnZoneChanged?.Invoke();
    }

    public void RevivePlayerWithGold()
    {
        if (GameState.GetGold() >= GoldCostToRevive)
        {
            GameState.AddGold(-GoldCostToRevive);
            OnPlayerRevive?.Invoke();
            Debug.Log("Player revived using gold!");
        }
        else
        {
            Debug.LogWarning("Not enough gold to revive!");
            Debug.LogWarning("Show purchase gold prompt. Won't implement in this demo.");
        }
    }

    public void RevivePlayerWithAds()
    {
        OnPlayerRevive?.Invoke();
        Debug.Log("Player revived by watching an ad!");
        Debug.Log("Ads not implemented for this demo!");
    }

    public void OnSpinResult(SpinResult result)
    {
        switch (result.slotConfig.prizeType)
        {
            case PrizeType.Gold:
                GameState.AddGold(result.slotConfig.value);
                break;
            case PrizeType.Cash:
                GameState.AddCash(result.slotConfig.value);
                break;
            case PrizeType.Item:
                GameState.AddToInventory(result.slotConfig.prizeName, result.slotConfig.value);
                break;
            case PrizeType.Bomb:
                OnBombDrawn();
                return; // When a bomb is hit, we return. Otherwise we advance the zone.
            default:
                Debug.LogError("Unknown prize type: " + result.slotConfig.prizeType);
                return;
        }

        AdvanceZone();
        OnRewardClaimed?.Invoke();
    }
}