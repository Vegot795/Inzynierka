using UnityEngine;
using TMPro;
using Unity.AppUI.UI;
using System.Collections.Generic;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject MainMenuTab;
    public GameObject PlayBut;
    public GameObject ScoreboardBut;
    public GameObject SettingsBut;
    public GameObject QuitBut;

    [Header("Play")]
    public GameObject PlayGameTab;
    public GameObject EnterNicknameText;
    public TMP_InputField NicknameInputField;
    public GameObject StartGameBut;
    public GameObject BackBut;

    [Header("Scoreboard")]
    public GameObject ScoreboardTab;
    public GameObject Scoretable;
    public GameObject BackButScoreboard;

    [Header("Data")]
    public List<GameObject> AllTabs = new List<GameObject>();
    public PlayerData playerData;

    void Start()
    {
        playerData = PlayerData.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        CloseAllTabs();
        PlayGameTab.SetActive(true);
    }

    public void StartGame()
    {
        playerData.playerNickname = NicknameInputField.text;
    }

    public void OpenScoreboard()
    {
        CloseAllTabs();
        ScoreboardTab.SetActive(true);
    }
    public void OpenSettings()
    {
        CloseAllTabs();
        SettingsBut.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        CloseAllTabs();
        MainMenuTab.SetActive(true);
    }
    private void CloseAllTabs()
    {
        foreach (GameObject tab in AllTabs)
        {
            tab.SetActive(false);
        }
    }
}
