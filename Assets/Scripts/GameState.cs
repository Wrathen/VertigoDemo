using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    private PlayerData playerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static PlayerData GetPlayerData()
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return new PlayerData();
        }

        // This creates a copy, so we are fine.
        return Instance.playerData;
    }
    public static void SetPlayerData(PlayerData newData)
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return;
        }

        Instance.playerData = newData;
    }
    public static void AddToInventory(string item, int quantity)
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return;
        }

        if (Instance.playerData.inventory.ContainsKey(item))
        {
            Instance.playerData.inventory[item] += quantity;
        }
        else
        {
            Instance.playerData.inventory[item] = quantity;
        }
    }
    public static void AddGold(int amount)
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return;
        }

        Instance.playerData.gold += amount;
    }
    public static void AddCash(int amount)
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return;
        }

        Instance.playerData.cash += amount;
    }
    public static void IncrementZone()
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return;
        }

        ++Instance.playerData.zone;
    }

    public static int GetZone()
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return 0;
        }

        return Instance.playerData.zone;
    }
    public static int GetGold()
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return 0;
        }

        return Instance.playerData.gold;
    }
    public static int GetCash()
    {
        if (Instance == null)
        {
            Debug.LogError("GameState instance is null.");
            return 0;
        }

        return Instance.playerData.cash;
    }
}
