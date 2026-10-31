using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public PC_Controller player = null;
    public static GameSettings GS;
    public Toggle fpsToggle;
    public TextMeshProUGUI fpsText;
    public Image LoadingScreen;
    public bool showFPS = false;
    public int frames;
    public float time;
    public int FPS;
    public bool isPlayerSet = false;
    public bool isLoadingScreenOn = false;
    public DeviceKind activeDevice;
    [Header("Scripts")]
    public UI_MainMenu UIM;
    public float LoadingScreenFadeTime = 5;
    public bool exitToScoreboard = false;

    [Header("Audio")]
    public AudioSource MainAS;
    public bool canPlayMusic = true;
    public AudioClip MenuMusic;
    public List<AudioClip> GameMusic = new List<AudioClip>();
    public Slider AudioVolume;
    public Slider MusicVolume;
    public int currentMusicNum = -1;

    public enum DeviceKind
    {
        KeyboardAndMouse,
        XboxPad,
        DualShock,
        OtherController
    }

    public void Awake()
    {
        GS = this;
        UIM = UI_MainMenu.UIM;
        DontDestroyOnLoad(this.gameObject);


        SetupGS();
    }

    private void SetupGS()
    {
        fpsText.gameObject.SetActive(false);
        if (GS == null)
        {
            GS = GameObject.Find("GameSettings").GetComponent<GameSettings>();
        }
        GS.MainAS.clip = MenuMusic;
        GameMusic.OrderBy(_ => UnityEngine.Random.value);

    }

    public void Update()
    {
        if (showFPS)
        {
            CalculateFPS();
            fpsText.text = $"FPS: {FPS}";
        }

        if (isPlayerSet)
        {
            
            var deviceInputed = player.GetComponent<PlayerInput>().devices;
            if (deviceInputed.Count > 0)
            {
                var newDevice = RecognizeDevice(deviceInputed[0]);
                if (newDevice != activeDevice)
                {
                    activeDevice = newDevice;
                    ChangeDeviceUI(activeDevice);

                }
            }
        }

        if (canPlayMusic && MainAS.isPlaying == false)
        {
            MainAS.Play();
        }

        if (!canPlayMusic && MainAS.isPlaying == true)
        {
            MainAS.Stop();
        }

        ChangeMusicVolume();
        GameMusicHandler();
    }
    public void CalculateFPS()
    {
        frames += 1;
        time += Time.unscaledDeltaTime;
        if (time >= 0.5)
        {
            FPS = (int)(frames / time);
            frames = 0 ;
            time = 0;
        }
    }


    public void SetPlayerObject(PC_Controller playerObj)
    {
        player = playerObj;
        if (player != null)
        {
            isPlayerSet = true;
        }
    }

    private DeviceKind RecognizeDevice(InputDevice playerInput)
    {

        if (playerInput.device is Keyboard || playerInput.device is Mouse)
        {
            return DeviceKind.KeyboardAndMouse;
        }
        else if (playerInput.device is XInputController)
        {
            return DeviceKind.XboxPad;
        }
        else if (playerInput.device is DualShockGamepad)
        {
            return DeviceKind.DualShock;
        }
        else
        {
            return DeviceKind.OtherController;
        }
        
    }

    private void ChangeDeviceUI(DeviceKind deviceKind)
    {

    }

    public void SwitchToKnM()
    {
        activeDevice = DeviceKind.KeyboardAndMouse;
        ChangeDeviceUI(activeDevice);
    }

    public IEnumerator TurnOnLoadingScreen(Action function)
    {
        float timer = 0f;
        var duration = LoadingScreenFadeTime;
        LoadingScreen.gameObject.SetActive(true);

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            LoadingScreen.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / duration));
            yield return null;
        }
        isLoadingScreenOn = true;
        Debug.Log("BlackScreen coroutine ended");
        function?.Invoke();
    }

    public IEnumerator TurnOffLoadingScreen(Action function)
    {
        float timer = 0f;
        var duration = LoadingScreenFadeTime;
        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            LoadingScreen.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / duration));
            yield return null;
        }
        LoadingScreen.gameObject.SetActive(false);
        isLoadingScreenOn = false;
        Debug.Log("BlackScreen coroutine ended");
        function?.Invoke();
    }

    public void ExitToScoreboard()
    {
        SceneManager.LoadScene("MainMenu");
        UIM.OpenScoreboard();
        StartCoroutine(TurnOffLoadingScreen(null));
    }

    #region ----- Toggles -----
    public void ToggleFpsScript()
    {
        showFPS = !showFPS;
        fpsText.gameObject.SetActive(showFPS);

    }

    public void ToggleMusic()
    {
        canPlayMusic = !canPlayMusic;
    }

    public void ChangeMusicVolume()
    {
        MainAS.volume = AudioVolume.value * MusicVolume.value;
    }

    public void GameMusicHandler()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" && MainAS.clip != MenuMusic)
        {
            MainAS.clip = MenuMusic;
            if (!MainAS.isPlaying)
            {
                MainAS.Play();
            }
        }
        else if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!MainAS.isPlaying || !GameMusic.Contains(MainAS.clip))
            {
                if (currentMusicNum == GameMusic.Count - 1)
                {
                    currentMusicNum = 0;
                }
                else
                {
                    currentMusicNum++;
                }
                MainAS.clip = GameMusic[currentMusicNum];
                MainAS.Play();

            }
            
        }
    }
    #endregion
}
