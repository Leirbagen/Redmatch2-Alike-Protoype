using Rewired; 
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public List<WeaponBase> startingWeapons = new List<WeaponBase>();
    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform aimingPosition;
    public float aimSpeed = 9f;
    private WeaponBase[] weaponSlots = new WeaponBase[2];
    public int activeWeaponIndex { get; private set; }
    private WeaponBase currentWeapon; 


    private void Start()
    {
        activeWeaponIndex = -1;
        foreach (WeaponBase startingWeapon in startingWeapons)
        {
            AddWeapon(startingWeapon);
        }
        SwitchWeapon(0);
    }
    private void Update()
    {
        
        if (currentWeapon != null)
        {
            if (InputController.Instance.GetButtonDown(InputController.Input.FIRE_1))
            {
                currentWeapon.TryShoot();
            }
            if (InputController.Instance.GetButtonDown(InputController.Input.RELOAD_WEAPON))
            {
                currentWeapon.StartReload();
            }
            if (InputController.Instance.GetButton(InputController.Input.AIM_WEAPON))
            {
                weaponParentSocket.position = Vector3.Lerp(weaponParentSocket.position, aimingPosition.position, Time.deltaTime * aimSpeed);
            }
            else
            {
                weaponParentSocket.position = Vector3.Lerp(weaponParentSocket.position, defaultWeaponPosition.position, Time.deltaTime * aimSpeed);
            }
        }

        float scrollValue = InputController.Instance.GetAxis(InputController.Input.SCROLL_WHEEL);
        if (scrollValue > 0f) 
        {
            if (activeWeaponIndex >= weaponSlots.Length - 1)
            {
                SwitchWeapon(0); 
            }
            else
            {
                SwitchWeapon(activeWeaponIndex + 1); 
            }
        }
        else if (scrollValue < 0f) 
        {
            if (activeWeaponIndex <= 0)
            {
                SwitchWeapon(weaponSlots.Length - 1); 
            }
            else
            {
                SwitchWeapon(activeWeaponIndex - 1); 
            }
        }
    }
    private void AddWeapon(WeaponBase p_weaponPrefab)
    {
        weaponParentSocket.position = defaultWeaponPosition.position;

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                WeaponBase weaponClone = Instantiate(p_weaponPrefab, weaponParentSocket);
                weaponClone.gameObject.SetActive(false);
                weaponSlots[i] = weaponClone;
                return;
            }
        }
    }
    private void SwitchWeapon(int newIndex)
    {
        if (weaponSlots[newIndex] == null)
        {
            return;
        }
        foreach (WeaponBase weapon in weaponSlots)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(false);
            }
        }
        weaponSlots[newIndex].gameObject.SetActive(true);
        activeWeaponIndex = newIndex;
        currentWeapon = weaponSlots[newIndex];
    }
}