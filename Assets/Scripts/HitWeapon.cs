using UnityEngine;

public class HitWeapon : WeaponBase
{
    public float fireRange = 200f;
    public LayerMask wallsHitLayer;
    public LayerMask hitLayer;
    public float backForce = 4f;
    [SerializeField] private int weaponDamage;
    public Transform weaponNozzle;
    public GameObject bulletHole;
    public GameObject flashEffect;
    private Transform cameraPlayerTransform;
    protected override void Start()
    {
        base.Start(); 
        cameraPlayerTransform = GameObject.FindWithTag("PlayerCamera").transform;
    }
    private void Update()
    {
        base.Update();
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
    }

    protected override void ExecuteShoot()
    {
        if (flashEffect != null && weaponNozzle != null)
        {
            GameObject flashClone = Instantiate(flashEffect, weaponNozzle.position, Quaternion.Euler(weaponNozzle.forward), transform);
            Destroy(flashClone, 0.1f);
        }
        AddRecoil();
        if (Physics.Raycast(cameraPlayerTransform.position, cameraPlayerTransform.forward, out RaycastHit hit, fireRange, hitLayer))
        {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(weaponDamage);
            }
            else 
            {
                GameObject bulletHoleClone = Instantiate(bulletHole, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHoleClone, 4f);
            }
        }
    }
    private void AddRecoil()
    {
        transform.Rotate(-backForce, 0, 0);
        transform.position -= transform.forward * (backForce / 50f);
    }
}