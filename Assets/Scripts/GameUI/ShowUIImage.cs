using UnityEngine;

public class ShowUIImage : MonoBehaviour
{

    public WeaponStats[] weaponStats;
    public GameObject[] pictureOfWeapon; 




    // Start is called once before the first execution of Update after the MonoBehaviour is 

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < weaponStats.Length; i++)
        {
            if (weaponStats[i].isActive)
            {
                pictureOfWeapon[i].SetActive(true);
            }
            else
            {
                pictureOfWeapon[i].SetActive(false);

            }
        }
    }
}
