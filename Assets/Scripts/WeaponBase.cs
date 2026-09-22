using UnityEngine;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour
{
    public int maxAmmo = 8;
    public float fireInterval = 0.5f;
    public float reloadTime = 1f;
    public Sprite weaponIcon;
    public int currentAmmo { get; protected set; }
    protected bool canShoot = true;
    protected bool isReloading = false;


    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }
    public virtual void TryShoot()
    {
        if (canShoot && !isReloading && currentAmmo > 0)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    private IEnumerator ShootRoutine()
    {
        canShoot = false;
        currentAmmo--;
        if (WeaponUI.Instance != null)
        {
            WeaponUI.Instance.UpdateBoth(currentAmmo, maxAmmo);
        }
        ExecuteShoot(); 
        yield return new WaitForSeconds(fireInterval);
        canShoot = true;
    }
    public virtual void StartReload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            Debug.Log("Recharging");
            StartCoroutine(ReloadRoutine());
        }
    }
    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        if (WeaponUI.Instance != null)
        {
            WeaponUI.Instance.UpdateBoth(currentAmmo, maxAmmo);
        }
        isReloading = false;
    }
    protected abstract void ExecuteShoot();

    protected void Update()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo");
        }
    }
}