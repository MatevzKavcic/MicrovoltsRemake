using UnityEngine;

public abstract class WeaponStats : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Default Weapon";
    public float damage = 10f;
    public float fireRate = 0.25f;

    protected float nextFireTime;
    protected AimDirection aimDirection;

    protected virtual void Awake()
    {
        aimDirection = FindObjectOfType<AimDirection>();
    }

    public virtual void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    protected abstract void Shoot();
}