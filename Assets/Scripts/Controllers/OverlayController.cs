using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayController : MonoBehaviour
{
    public static OverlayController OC;
    public GameControllerScript GCS;
    public GameSettings GS;
    public GameSettings FakeGS;
    public BoofManager BM;
    public UIController UIC;
    public GameObject tipPrefab;
    public Camera cam;
    public GameObject player;
    public List<GameObject> tipList = new List<GameObject>();
    [Header("Esc Menu")]
    public GameObject EscMenu;
    public GameObject EscMenuBackground;
    public GameObject SettingsTab;
    public GameObject MenuButtons;
    public bool isEscMenuOpen = false;

    [Header("Settings")]
    public Slider audioSlider;
    public Slider musicSlider;
    public Slider enemySlider;
    public Toggle musicToggle;
    public Toggle fpsToggle;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public float audioVolume;
    public float musicVolume;
    public bool isAudioSet = false;
    public GameSettingsSave setts;

    [Header("Else")]
    public GameObject hpSliderBox;
    public GameObject hpSliderPrefab;
    public GameObject tipBox;


    void Awake()
    {
        OC = this;
    }

    public void SetupOC()
    {
        GCS = GameControllerScript.GCS;
        BM = BoofManager.boofManager;
        UIC = UIController.UIC;
        GS = GameSettings.GS;
        if (GS == null)
        {
            GS = GCS.GS;
        }
        LoadSettings();
        isAudioSet = true;
    }

    public void ToggleFPS() => GS.ToggleFpsScript();
    public void ToggleMusic() => GS.ToggleMusic();
    public void HandleAudioVolume()
    {
        if (isAudioSet)
        {
            GS.audioFloat = audioSlider.value;
            GS.musicFloat = musicSlider.value;
        }        
    }
    

    void Update()
    {
        if (tipList.Count > 0)
        {
            foreach (var tip in tipList)
            {
                tip.transform.position = cam.WorldToScreenPoint(GetPositionForTip(tip));
            }
        }
        HandleAudioVolume();
        HandleHpSlider();

        if (UIC.gameEnded)
        {
            tipBox.SetActive(false);
            hpSliderBox.SetActive(false);
        }
        else
        {
            tipBox.SetActive(true);
            hpSliderBox.SetActive(true);
        }
    }

    public Vector3 GetPositionForTip(GameObject tip)
    {
        TipController TC = tip.GetComponent<TipController>();
        Vector3 displayPosition = new Vector3();

        float xDistance = 1f;
        float yDistance = 1f;

        if (TC.owner is DoorScript)
        {
            xDistance = 1f;
            yDistance = 1f;
        }
        else if (TC.owner is BoofStationScript)
        {
            xDistance = 2f;
            yDistance = 1f;
        }

        if (TC.assignedObject.GetComponent<InteractableBase>().orientation == MapGenerator.Corridor.Orientation.Vertical)
        {
            if(GCS.playerObj.transform.position.y > TC.assignedObject.transform.position.y)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y - yDistance, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y + 1f, TC.assignedObject.transform.position.z);
            }
        }
        else if (TC.assignedObject.GetComponent<InteractableBase>().orientation == MapGenerator.Corridor.Orientation.Horizontal)
        {
            if (GCS.playerObj.transform.position.x > TC.assignedObject.transform.position.x)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x - xDistance, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x + xDistance, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
            }
        }

        return displayPosition;
    }
    public void MakeDoorTip(DoorScript door)
    {
        if (GCS == null)
        {
            GCS = GameControllerScript.GCS;
        }
        GameObject tip = Instantiate(tipPrefab);
        tip.transform.SetParent(tipBox.transform);
        TipController TC = tip.GetComponent<TipController>();
        TC.owner = door;
        TC.title.text = "Open";
        TC.cost.text = GCS.doorPrice.ToString();
        door.Tip = tip;
        TC.assignedObject = door.gameObject;
        tipList.Add(tip);
    }

    public void DestroyDoorTip(DoorScript door)
    {
        Debug.Log($"[OC] - {door.Tip.name} has to be deleted");
        tipList.Remove(door.Tip);
        Destroy(door.Tip);
    }

    public void MakeBoofStationTip(BoofStationScript BSS)
    {
        if (GCS == null)
        {
            GCS = GameControllerScript.GCS;
        }
        GameObject tip = Instantiate(tipPrefab);
        tip.transform.SetParent(tipBox.transform);
        TipController TC = tip.GetComponent<TipController>();
        TC.owner = BSS;
        TC.title.text = BSS.BBD.boofName.ToString();
        
        if(!BM.BoofBox.ContainsKey(BSS.BBD))
        {
            TC.cost.text = BSS.BBD.boofCost.ToString();
        }
        else
        {
            TC.cost.text = "Already obtained";
        }

        BSS.Tip = tip;
        TC.assignedObject = BSS.gameObject;
        tipList.Add(tip);
        Debug.Log("Boof Tip Created");
    }

    public void DestroyBoofStationTip(BoofStationScript BSS)
    {
        Debug.Log($"[OC] - {BSS.Tip.name} has to be deleted");
        tipList.Remove(BSS.Tip);
        Destroy(BSS.Tip);
    }

    public void MakeBuyItemTip(BuyableBase buyable)
    {
        if (GCS == null)
        {
            GCS = GameControllerScript.GCS;
        }
        GameObject tip = Instantiate(tipPrefab);
        tip.transform.SetParent(tipBox.transform);
        TipController TC = tip.GetComponent<TipController>();
        TC.owner = buyable;
        TC.title.text = buyable.objectName;
        if (buyable is WeaponBuyScript WBS && player.GetComponent<PlayerInventory>().weaponClass.weaponData == WBS.weaponData)
        {
            TC.cost.text = "Already obtained";
        }
        else
        {
            TC.cost.text = buyable.price.ToString();
        }
        buyable.Tip = tip;
        TC.assignedObject = buyable.gameObject;
        tipList.Add(tip);
    }

    public void DestroyBuyItemTip(BuyableBase buyable)
    {
        tipList.Remove(buyable.Tip);
        Destroy(buyable.Tip);
    }

    private void HandleHpSlider()
    {
        foreach (var enemy in GCS.enemiesSpawned)
        {
            if (enemy.sliderPreview == null)
            {
                enemy.sliderPreview = Instantiate(hpSliderPrefab);
                enemy.sliderPreview.transform.SetParent(hpSliderBox.transform);
                enemy.sliderPreview.gameObject.SetActive(false);
            }

            if (enemy.CurrentHp < enemy.currentMaxHp)
            {
                enemy.sliderPreview.gameObject.SetActive(true);
            }
            else
            {
                enemy.sliderPreview.gameObject.SetActive(false);
            }

            var sliderPosition = cam.WorldToScreenPoint(GetPositionForSlider(enemy));
            enemy.sliderPreview.GetComponent<Slider>().value = enemy.CurrentHp / enemy.currentMaxHp;
            enemy.sliderPreview.transform.position = sliderPosition;
            Debug.Log("Code reached hpSlider transform position");
        }
        
    }

    private Vector3 GetPositionForSlider(EnemyClass enemy)
    {
        Vector3 displayPosition = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.7f, enemy.transform.position.y);
        return displayPosition;
    }

    #region ----- Escape Menu -----



    public void ToggleEscMenu()
    {
        isEscMenuOpen = !isEscMenuOpen;

        if (isEscMenuOpen)
        {
            UIC.gameObject.SetActive(false);
            EscMenu.SetActive(true);
            EscMenuBackground.SetActive(true);
            MenuButtons.SetActive(true);
            Time.timeScale = 0;
            GS.isGamePaused = true;
        }
        else
        {
            UIC.gameObject.SetActive(true);
            EscMenu.SetActive(false);
            EscMenuBackground.SetActive(false);
            MenuButtons.SetActive(false);
            SettingsTab.SetActive(false);
            Time.timeScale = 1;
            GS.isGamePaused = false;
        }
    }

    public void OpenSettings()
    {
        MenuButtons.SetActive(false);
        SettingsTab.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsTab.SetActive(false);
        EscMenu.SetActive(true);
    }

    public void QuitToMM()
    {

    }

    public void LoadSettings()
    {
        setts = GameSettingsSaveSystem.Load();
        audioSlider.SetValueWithoutNotify(setts.AudioVolume);
        musicSlider.SetValueWithoutNotify(setts.MusicVolume);
        enemySlider.SetValueWithoutNotify(setts.EnemyVolume);
        fpsToggle.SetIsOnWithoutNotify(setts.showFPS);
        musicToggle.SetIsOnWithoutNotify(setts.playMusic);
        GS.LoadFromSaveFile();
    }

    public void SaveSettings()
    {
        GS.audioFloat = audioSlider.value;
        GS.musicFloat = musicSlider.value;
        GS.enemyFloat = enemySlider.value;
        GS.showFPS = fpsToggle.isOn;
        GS.canPlayMusic = musicToggle.isOn;
        GameSettingsSaveSystem.Save(setts);
        GS.LoadFromSaveFile();
        BackToMenu();
    }

    public void BackToMenu()
    {
        SettingsTab.SetActive(false);
        MenuButtons.SetActive(true);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void QuitToMainMenu()
    {
        GS.ExitToScoreboard();
    }
    #endregion
}
