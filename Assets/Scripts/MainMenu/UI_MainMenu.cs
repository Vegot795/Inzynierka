using UnityEngine;
using TMPro;
using Unity.AppUI.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    public static UI_MainMenu UIM;
    [Header("Main Menu")]
    public GameObject MainMenuTab;

    [Header("Play")]
    public GameObject PlayGameTab;
    public GameObject EnterNicknameText;
    public TMP_InputField NicknameInputField;
    public Image PlayerSpritePreview;

    [Header("Scoreboard")]
    public GameObject ScoreboardTab;
    public GameObject Scoretable;
    public GameObject BackButScoreboard;
    public int scorePositionsShown = 10;
    public GameObject scoreRecordHolder;

    [Header("Settings")]
    public GameObject SettingsTab;

    [Header("Data")]
    public List<GameObject> AllTabs = new List<GameObject>();
    public PlayerData playerDataPrefab;
    public PlayerData PD;
    public GameSettings GS;
    public GameObject GSprefab;
    public StatListClass scoreListClass;
    public GameObject RecordPrefab;
    public List<Sprite> PlayerSprites;
    public int currentPCPrevNum = 0;

    void Start()
    {
        UIM = this;
        GS = GameSettings.GS;

        if (GS == null)
        {
            GameObject GameSettingsObj = Instantiate(GSprefab);
            DontDestroyOnLoad(GameSettingsObj);
            GS = GameSettings.GS;
        }
        if (GS.UIM == null)
        {
            GS.UIM = UIM;
        }

        if (GS.exitToScoreboard == true)
        {
            OpenScoreboard();
            GS.exitToScoreboard = false;
        }

        StartCoroutine(GS.TurnOffLoadingScreen(null));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        CloseAllTabs();
        PlayGameTab.SetActive(true);
        PlayerSpritePreview.sprite = PlayerSprites[0];
    }

    public void StartGame()
    {
        if (PlayerData.Instance == null)
        {
            PD = Instantiate(playerDataPrefab).GetComponent<PlayerData>();
            DontDestroyOnLoad(PD);
        }
        PD.playerNickname = NicknameInputField.text;
        PD.playerSprite = PlayerSprites[currentPCPrevNum];

        StartCoroutine(GS.TurnOnLoadingScreen(LoadGameScene));
    }

    public void OpenScoreboard()
    {
        CloseAllTabs();
        ScoreboardTab.SetActive(true);
        scoreListClass = EndgameStatsController.Load();
        var scoreList = scoreListClass.StatList;
        scoreList.OrderBy(x => x.totalScore).ToList();
        for (var i = 0; i < scorePositionsShown; i++)
        {
            try
            {
                var ScoreRecord = Instantiate(RecordPrefab, scoreRecordHolder.transform);
                ScoreRecord.gameObject.name = $"Score {i}";
                //ScoreRecord.transform.parent = scoreRecordHolder.transform;
                ScoreRecord.GetComponent<RecordScript>().Initialize(scoreList[i], i);
            }
            catch
            {
                Debug.LogWarning($"Score {i.ToString()} could not be loaded");
            }
            
        }
    }
    public void OpenSettings()
    {
        CloseAllTabs();
        SettingsTab.SetActive(true);
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

    private void LoadGameScene()
    {
        SceneManager.LoadScene("ProceduralGeneratedScene");
    }

    public void NextPcSprite()
    {
        if (currentPCPrevNum == 2)
        {
            currentPCPrevNum = 0;
        }
        else
        {
            currentPCPrevNum++;
        }
        ChangePcSpritePreview();
    }

    public void PrevPcSprite()
    {
        if (currentPCPrevNum == 0)
        {
            currentPCPrevNum = 2;
        }
        else
        {
            currentPCPrevNum--;
        }
        ChangePcSpritePreview();
    }

    private void ChangePcSpritePreview()
    {
        PlayerSpritePreview.sprite = PlayerSprites[currentPCPrevNum];
    }
}
