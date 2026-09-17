using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using Rewired;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    private Transform cameraPlayerTransform;
    public Transform weaponNozzle;
    public LayerMask hitLayer;
    public float fireRange = 200;
    public float backForce = 4f;
    public float fireInterval = 0.5f;
    public float reloadTime = 1;
    private bool canShoot = true;
    private bool canReload;
    public int maxAmmo = 8;
    public int currentAmmo { get; private set;}
    public GameObject bulletHole;
    public GameObject flashEffect;

    private void Start()
    {
        currentAmmo = maxAmmo;
        cameraPlayerTransform = GameObject.FindWithTag("PlayerCamera").transform;
    }
    private void Update()
    {
        if (InputController.Instance.GetButtonDown(InputController.Input.FIRE_1)) 
        {
            TryShoot();
        }

        if (InputController.Instance.GetButtonDown(InputController.Input.RELOAD_WEAPON)) 
        {
            if (currentAmmo <= maxAmmo) 
            {
                canReload = true;
                StartCoroutine(Reload());
            }
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
    }
    private void TryShoot() 
    {
        if (canShoot == true) 
        {
            if (currentAmmo >= 1) 
            {
                StartCoroutine(ShootRoutine());
                currentAmmo--;
            }
        }
    }
    private void HandleShoot() 
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPlayerTransform.position, cameraPlayerTransform.forward, out hit, fireRange, hitLayer)) 
        {
            Debug.Log("Disparo");
            Instantiate(flashEffect, weaponNozzle.position, Quaternion.Euler(weaponNozzle.forward), transform);
            AddRecoil();
            GameObject bulletHoleClone = Instantiate(bulletHole, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
            Destroy(bulletHoleClone, 4f);
        } 
    }
    private void AddRecoil() 
    {
        transform.Rotate(-backForce, 0, 0);
        transform.position = transform.position - transform.forward * (backForce/50f);
    }

    IEnumerator ShootRoutine() 
    {
        HandleShoot();
        canReload = false;
        canShoot = false;
        yield return new WaitForSeconds(fireInterval);
        canShoot= true;
        canReload = true;
    }
    IEnumerator Reload() 
    {
        Debug.Log("recargando");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }
}
