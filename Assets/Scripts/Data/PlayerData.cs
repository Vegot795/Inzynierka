using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    public string playerNickname;
    public Sprite playerSprite;
    public int TotalScore;
    public int TotalEnemiesKilled;
    public string Date;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
