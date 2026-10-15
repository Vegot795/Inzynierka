using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController UIC;
    public GameObject PC;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI wavesText;
    public GameObject DeathScreen;
    public GameObject BoofList;
    public WeaponClass riffle;
    public CharacterBase character;
    public PlayerInventory inventory;
    public GameControllerScript GCS;

    [Header("Endgame Stats")]
    public Image BlackScreen;
    public TextMeshProUGUI DeathScreenText;
    public float blackScreenAlpha = 0;
    public TextMeshProUGUI TotalScore;
    public TextMeshProUGUI TotalEnemiesKilled;
    public TextMeshProUGUI TotalZombiesKilled;
    public TextMeshProUGUI TotalSkeletsKilled;
    public float timeToDisplayStatistics;

    private void Awake()
    {
        if (UIC == null)
        {
            UIC = this;
        }
    }

	public void SetupUIC()
    {

        PC = GameObject.FindGameObjectWithTag("Player");
        ammoText = GameObject.FindGameObjectWithTag("AmmoDisplay").GetComponent<TextMeshProUGUI>();
        hpText = GameObject.FindGameObjectWithTag("hpDisplay").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreDisplay").GetComponent<TextMeshProUGUI>();
        wavesText = GameObject.FindGameObjectWithTag("WavesDisplay").GetComponent<TextMeshProUGUI>();
        BoofList = transform.Find("BoofList").gameObject;
        DeathScreen = transform.Find("DeathScreen").gameObject;

        //BlackScreen = DeathScreen.transform.Find("BlackScreen").GetComponent<Image>();
        //DeathScreenText = transform.Find("DeathScreenText").GetComponent<TextMeshProUGUI>();

        DeathScreen.SetActive(false);


        riffle = PC.GetComponentInChildren<WeaponClass>();
		character = PC.GetComponent<CharacterBase>();
		inventory = PC.GetComponent<PlayerInventory>();
        GCS = GameControllerScript.GCS;
        ShowDeathScreen();

	}

	private void Update()
   {
        UpdateAmmoText();
        UpdateHealthText();
        UpdateScore();
        UpdateWaves();
    }

    private void UpdateAmmoText()
   {
        if (PC != null)
        {
            if (riffle != null)
            {
                ammoText.text = "Ammo: " + riffle.currentAmmo + "/" + riffle.currentMaxAmmo;
            }
            else
            {
                Debug.Log("riffle is missing, trying to reassign");
                riffle = PC.GetComponentInChildren<WeaponClass>();

            }
        }
        else
        {
            Debug.Log("PC is null");
        }
   }

    private void UpdateHealthText()
    {
        if (PC != null)
        {
            if (character != null)
            {
                hpText.text = "HP: " + character.CurrentHp + "/" + character.currentMaxHp;
            }
        }
    }

    private void UpdateScore()
    {
        if (PC != null)
        {
            if (inventory != null)
            {                
                if (scoreText != null)
                {
                    scoreText.text = "Score: " + inventory.currentScorePoints;
                }
            }
            else
            {
                //Debug.LogWarning("ScoreSystem component not found on the player.");
            }
        }
    }

    private void UpdateWaves()
    {
        if (PC != null)
        {
            if (wavesText != null)
            {
                wavesText.text = $"Wave: {GCS.WaveNumber}";
            }
        }
    }

    public void ShowDeathScreen()
    {
        BlackScreen.color = new Color(0,0,0,blackScreenAlpha);
        BlackScreen.gameObject.SetActive(true);
        DeathScreenText.gameObject.SetActive(true);
        StartCoroutine(StartBlackScreenFade(3));

    }

    public void ShowEndgameStatistics()
    {
        TotalScore.text = "Total score: " + GCS.TotalScore;
        TotalEnemiesKilled.text = "Total enemies killed: " + GCS.AllEnemiesKilled;
        TotalZombiesKilled.text = "Total zombies killed: " + GCS.ZombiesKilled;
        TotalSkeletsKilled.text = "Total skelets killed: " + GCS.SkeletsKilled;
    }
    private IEnumerator StartBlackScreenFade(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            BlackScreen.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer/duration));
            yield return null;
        }
        Debug.Log("BlackScreen coroutine ended");
        StartCoroutine(ShowPlayerStats(timeToDisplayStatistics));
    }

    private IEnumerator ShowPlayerStats(float time)
    {
        DeathScreenText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        ShowEndgameStatistics();
        yield return new WaitForSeconds(time);
    }
}
