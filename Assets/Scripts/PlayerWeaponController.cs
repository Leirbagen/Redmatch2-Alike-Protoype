using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public List<WeaponController> startingWeapons = new List<WeaponController>();
    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform aimingPosition;
    private WeaponController[] weaponSlots = new WeaponController[2];
    public int activeWeaponIndex { get; private set; }
    private void Start()
    {
        activeWeaponIndex = -1;

        foreach (WeaponController startingWeapon in startingWeapons)
        {
            AddWeapon(startingWeapon);
        }

        SwitchWeapon();
    }

    private void AddWeapon(WeaponController p_weaponPrefab) 
    {
        weaponParentSocket.position = defaultWeaponPosition.position;

        for (int i = 0; i < weaponSlots.Length; i++) 
        {
            if (weaponSlots[i] == null) 
            {
                WeaponController weaponClone = Instantiate(p_weaponPrefab, weaponParentSocket);
                weaponClone.gameObject.SetActive(false);
                weaponSlots[i] = weaponClone;
                return;
            }
        }
    }
    private void SwitchWeapon() 
    {
        int tempIndex = (activeWeaponIndex + 1) % weaponSlots.Length;
        if (weaponSlots[tempIndex] == null) 
        {
            return;
        }

        foreach (WeaponController weapon in weaponSlots) 
        {
            if (weapon != null) 
            {
                weapon.gameObject.SetActive(false);
            }
        }
        weaponSlots[tempIndex].gameObject.SetActive(true);
        activeWeaponIndex = tempIndex;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SwitchWeapon();
        }
    }
}
