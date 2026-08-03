using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject PC;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
               PC = GameObject.FindGameObjectWithTag("Player");
               ammoText = GameObject.FindGameObjectWithTag("AmmoDisplay").GetComponent<TextMeshProUGUI>();
               hpText = GameObject.FindGameObjectWithTag("hpDisplay").GetComponent<TextMeshProUGUI>();
               scoreText = GameObject.FindGameObjectWithTag("ScoreDisplay").GetComponent<TextMeshProUGUI>();
    }

   private void Update()
   {
       UpdateAmmoText();
       UpdateHealthText();
       UpdateScore();
    }

   private void UpdateAmmoText()
   {
       if (PC != null)
       {
           WeaponClass riffle = PC.GetComponentInChildren<WeaponClass>();
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
            CharacterBase character = PC.GetComponent<CharacterBase>();
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
            ScoreSystem scoreSystem = PC.GetComponent<ScoreSystem>();
            if (scoreSystem != null)
            {
                
                if (scoreText != null)
                {
                    scoreText.text = "Score: " + scoreSystem.currentScorePoints;
                }
            }
            else
            {
                Debug.LogWarning("ScoreSystem component not found on the player.");
            }
        }
    }
}
