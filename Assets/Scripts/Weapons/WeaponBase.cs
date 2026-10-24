using Fusion;
using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public int maxAmmo = 8;
    public float fireInterval = 0.5f;
    public float reloadTime = 1f;
    public Sprite weaponIcon;
    public Sprite ammoIcon;
    public int currentAmmo { get; protected set; }
    protected bool canShoot = true;
    protected bool isReloading = false;
    public bool isAutomatic = false;
    public static event Action<int, int> OnAmmoChanged;
    protected bool isLocalPlayer = false;
    protected bool isServer = false;
    protected float nextFireTime = 0f;
    [SerializeField] protected Animator weaponAnimator;
    protected PlayerRef weaponOwner;

    public void SetupNetworkRole(bool localPlayer, bool server, PlayerRef owner)
    {
        isLocalPlayer = localPlayer;
        isServer = server;
        weaponOwner = owner;
    }
    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
        if(weaponAnimator == null)
        {
            weaponAnimator = GetComponent<Animator>();
        }
    }
    protected virtual void OnEnable()
    {
        if (Time.time >= nextFireTime)
        {
            canShoot = true;
        }
        isReloading = false;
    }
    public virtual bool TryShoot()
    {
        if (canShoot && Time.time >= nextFireTime && !isReloading && currentAmmo > 0 && gameObject.activeInHierarchy)
        {
            nextFireTime = Time.time + fireInterval; 
            StartCoroutine(ShootRoutine());
            return true;
        }
        return false;
    }
    public virtual void SetAiming(bool aiming) { }
    public void RefreshAmmoUI()
    {
        if (isLocalPlayer)
        {
            OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        }
    }
    private IEnumerator ShootRoutine()
    {
        canShoot = false;
        currentAmmo--;
        RefreshAmmoUI();
        ExecuteShoot();
        yield return new WaitForSeconds(fireInterval);
        canShoot = true;
    }
    public virtual void StartReload()
    {
        if (!isReloading && currentAmmo < maxAmmo && gameObject.activeInHierarchy)
        {
            if(weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("Reload");
            }
            StartCoroutine(ReloadRoutine());
        }
    }
    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;

        RefreshAmmoUI();

        isReloading = false;
    }

    public void SetSprinting(bool sprinting)
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("Sprint", sprinting);
        }
    }
    protected abstract void ExecuteShoot();
    public abstract void PlayNetworkShootEffects();
    public virtual void InjectCamera(Camera cam) { }
}