using UnityEngine;

public class ReloadingPictureApear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public WeaponStats weaponStats;

    public GameObject reloadPicture;

    private void Update()
    {
        if (weaponStats.isReloading == true)
        {
            reloadPicture.SetActive(true);
        }else { 
            reloadPicture.SetActive(false);
        }
    }
}
