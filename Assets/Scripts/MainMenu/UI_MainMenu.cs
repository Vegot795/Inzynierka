using UnityEngine;
using TMPro;
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
    public Slider AudioVolume;
    public Slider MusicVolume;
    public Slider EnemyVolume;
    public Toggle musicToggle;
    public Toggle fpsToggle;

    [Header("Tutorial")]
    public GameObject TutorialTab;

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
    public bool isAudioSet = false;
    public GameSettingsSave setts;

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

        LoadSettings();
        isAudioSet = true;
        StartCoroutine(GS.TurnOffLoadingScreen(null));
    }

    // Update is called once per frame
    void Update()
    {
        HandleSettingsInRealTime();
    }

    public void PlayGame()
    {
        CloseAllTabs();
        PlayGameTab.SetActive(true);
        PlayerSpritePreview.sprite = PlayerSprites[0];
    }

    public void StartGame()
    {
        if (PlayerData.Instance == null || PD == null)
        {
            PD = Instantiate(playerDataPrefab).GetComponent<PlayerData>();
            DontDestroyOnLoad(PD);
        }
        else
        {
            PD.Clear();
        }

        PD.playerNickname = NicknameInputField.text;
        PD.playerSprite = PlayerSprites[currentPCPrevNum];

        GS.ChangeCursorSprite(GS.InGameCursor);
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

    public void HandleSettingsInRealTime()
    {
        if (isAudioSet)
        {
            GS.audioFloat = AudioVolume.value;
            GS.musicFloat = MusicVolume.value;
            GS.enemyFloat = EnemyVolume.value;
            GS.canPlayMusic = musicToggle.isOn;
            GS.showFPS = fpsToggle.isOn;
        }
    }

    public void LoadSettings()
    {
        setts = GameSettingsSaveSystem.Load();
        AudioVolume.SetValueWithoutNotify(setts.AudioVolume);
        MusicVolume.SetValueWithoutNotify(setts.MusicVolume);
        fpsToggle.SetIsOnWithoutNotify(setts.showFPS);
        musicToggle.SetIsOnWithoutNotify(setts.playMusic);
        GS.LoadFromSaveFile();
        GS.ChangeCursorSprite(GS.DefaultCursor);
    }

    public void SaveSettings()
    {
        GS.audioFloat = AudioVolume.value;
        GS.musicFloat = MusicVolume.value;
        GS.showFPS = fpsToggle.isOn;
        GS.canPlayMusic = musicToggle.isOn;
        GameSettingsSaveSystem.Save(setts);
        GS.LoadFromSaveFile();
        BackToMainMenu();
    }

    public void BackFromSettings()
    {
        LoadSettings();
        BackToMainMenu();
    }

    public void OpenTutorial()
    {
        CloseAllTabs();
        TutorialTab.SetActive(true);
    }
}
