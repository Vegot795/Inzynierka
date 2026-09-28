using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject PC;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI wavesText;
    public GameObject BuffsList;
    public WeaponClass riffle;
    public CharacterBase character;
    public PlayerInventory inventory;
    public GameControllerScript GCS;

	private void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player");
        ammoText = GameObject.FindGameObjectWithTag("AmmoDisplay").GetComponent<TextMeshProUGUI>();
        hpText = GameObject.FindGameObjectWithTag("hpDisplay").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.FindGameObjectWithTag("ScoreDisplay").GetComponent<TextMeshProUGUI>();
        wavesText = GameObject.FindGameObjectWithTag("WavesDisplay").GetComponent<TextMeshProUGUI>();
        BuffsList = GameObject.FindGameObjectWithTag("BuffsList");


        riffle = PC.GetComponentInChildren<WeaponClass>();
		character = PC.GetComponent<CharacterBase>();
		inventory = PC.GetComponent<PlayerInventory>();
        GCS = GameControllerScript.GCS;


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
               ammoText.text = "Ammo: " + riffle.currentAmmo + "/" + riffle.maxAmmo;
           }
       }
   }

    private void UpdateHealthText()
    {
        if (PC != null)
        {
            if (character != null)
            {
                hpText.text = "HP: " + character.CurrentHp + "/" + character.MaxHp;
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
}
