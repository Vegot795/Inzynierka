using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject PC;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI hpText;

    private void Start()
    {
               PC = GameObject.FindGameObjectWithTag("Player");
               ammoText = GameObject.FindGameObjectWithTag("AmmoDisplay").GetComponent<TextMeshProUGUI>();
               hpText = GameObject.FindGameObjectWithTag("hpDisplay").GetComponent<TextMeshProUGUI>();
    }

   private void Update()
   {
       UpdateAmmoText();
       UpdateHealthText();
    }

   private void UpdateAmmoText()
   {
       if (PC != null)
       {
           RiffleScript riffle = PC.GetComponentInChildren<RiffleScript>();
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
}
