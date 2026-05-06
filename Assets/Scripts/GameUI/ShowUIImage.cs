using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowUIImage : MonoBehaviour
{

    public WeaponStats[] weaponStats;
    public GameObject[] pictureOfWeapon;

    public TMP_Text currentAmmo;
    public TMP_Text totalAmmo;





    // Start is called once before the first execution of Update after the MonoBehaviour is 

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < weaponStats.Length; i++)
        {
            if (weaponStats[i].isActive)
            {
                pictureOfWeapon[i].SetActive(true);

                if (weaponStats[i].weaponName != "pencil") {

                    currentAmmo.text = weaponStats[i].ammo.ToString();

                    totalAmmo.text = weaponStats[i].totalAmo.ToString();
                }
                else
                {
                    currentAmmo.text = "0";

                    totalAmmo.text = "0";

                }

            }
            else
            {
                pictureOfWeapon[i].SetActive(false);

            }




        }
    }
}
