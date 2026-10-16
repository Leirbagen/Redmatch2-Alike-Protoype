using Fusion;
using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour
{
    [Serializable]
    public struct WeaponLoad
    {
        public string categoryName;
        public WeaponBase[] weapons;
    }
    public List<WeaponBase> startingWeapons = new List<WeaponBase>();
    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform aimingPosition;
    public Camera playerCamera;
    public float aimSpeed = 9f;
    private WeaponBase[] weaponSlots = new WeaponBase[2];
    public WeaponLoad[] availableCategories;
    private WeaponBase currentWeapon;
    private bool isSwitchingAxis = false;

    public static event Action<int> OnWeaponSwitched;
    public static event Action OnWeaponsCleared;
    public static event Action<Sprite, int> OnWeaponIconReady;

    [Networked, OnChangedRender(nameof(OnCategoryChanged))]
    public int SyncedCategoryIndex { get; set; } = -1;

    [Networked, OnChangedRender(nameof(OnActiveWeaponChanged))]
    public int activeWeaponIndex { get; set; } = -1;
    [Networked] public NetworkBool IsAiming { get; set; }
    [Networked] private NetworkButtons PreviousButtons { get; set; }

    private void OnEnable()
    {
        WeaponUI.OnUIWakesUp += ForceUIUpdate;
        if (weaponSlots != null && weaponSlots[0] != null)
        {
            ForceUIUpdate();
        }
    }
    private void OnDisable()
    {
        WeaponUI.OnUIWakesUp -= ForceUIUpdate;
    }
    public override void Spawned()
    {
        base.Spawned();
        if (HasInputAuthority)
        {
            OnWeaponsCleared?.Invoke();
            int mySelectedCategory = PlayerPrefs.GetInt("SelectedWeaponCategory", 0);
            RPC_SetWeaponCategory(mySelectedCategory);
        }
        if (SyncedCategoryIndex != -1)
        {
            BuildWeapons(SyncedCategoryIndex);
        }
        if (activeWeaponIndex != -1)
        {
            ApplyWeaponSwitchVisuals(activeWeaponIndex);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetWeaponCategory(int categoryIndex)
    {
        SyncedCategoryIndex = categoryIndex;
        BuildWeapons(categoryIndex);
        activeWeaponIndex = 0;
        ApplyWeaponSwitchVisuals(0);
    }

    private void OnCategoryChanged()
    {
        BuildWeapons(SyncedCategoryIndex);
        if (activeWeaponIndex != -1)
        {
            ApplyWeaponSwitchVisuals(activeWeaponIndex);
        }
    }

    private void OnActiveWeaponChanged()
    {
        ApplyWeaponSwitchVisuals(activeWeaponIndex);
    }

    private void Update()
    {
        if (Object == null || !Object.IsValid) return;
        if (Time.timeScale == 0f) return;
        if (currentWeapon != null)
        {
            Transform targetPos = IsAiming ? aimingPosition : defaultWeaponPosition;
            weaponParentSocket.position = Vector3.Lerp(weaponParentSocket.position, targetPos.position, Time.deltaTime * aimSpeed);
            if (HasInputAuthority)
            {
                currentWeapon.SetAiming(IsAiming);
            }
        }
        if (HasInputAuthority && currentWeapon != null && InputController.Instance != null)
        {
            float scrollValue = InputController.Instance.GetAxis(InputController.Input.SCROLL_WHEEL);
            if (scrollValue > 0.1f)
            {
                if (!isSwitchingAxis)
                {
                    int nextIndex = activeWeaponIndex >= weaponSlots.Length - 1 ? 0 : activeWeaponIndex + 1;
                    RPC_RequestWeaponSwitch(nextIndex);
                    isSwitchingAxis = true;
                }
            }
            else if (scrollValue < -0.1f)
            {
                if (!isSwitchingAxis)
                {
                    int prevIndex = activeWeaponIndex <= 0 ? weaponSlots.Length - 1 : activeWeaponIndex - 1;
                    RPC_RequestWeaponSwitch(prevIndex);
                    isSwitchingAxis = true;
                }
            }
            else
            {
                isSwitchingAxis = false;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestWeaponSwitch(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weaponSlots.Length && weaponSlots[newIndex] != null)
        {
            activeWeaponIndex = newIndex;
            ApplyWeaponSwitchVisuals(newIndex);
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            bool fireJustPressed = data.buttons.WasPressed(PreviousButtons, InputButton.fire);
            bool reloadJustPressed = data.buttons.WasPressed(PreviousButtons, InputButton.reload);
            PreviousButtons = data.buttons;

            if (currentWeapon != null)
            {
                IsAiming = data.buttons.IsSet(InputButton.aim);

                if (Runner.IsForward)
                {
                    bool shotFired = false;
                    if (currentWeapon.isAutomatic)
                    {
                        if (data.buttons.IsSet(InputButton.fire))
                        {
                            shotFired = currentWeapon.TryShoot();
                        }
                    }
                    else
                    {
                        if (fireJustPressed)
                        {
                            shotFired = currentWeapon.TryShoot();
                        }
                    }
                    if (shotFired && HasStateAuthority)
                    {
                        RPC_PlayShootEffects();
                    }

                    if (reloadJustPressed)
                    {
                        currentWeapon.StartReload();
                    }
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayShootEffects()
    {
        if (currentWeapon != null)
        {
            currentWeapon.PlayNetworkShootEffects();
        }
    }

    public void ForceUIUpdate()
    {
        if (Object == null || !HasInputAuthority) return;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            OnWeaponIconReady?.Invoke(weaponSlots[i] != null ? weaponSlots[i].weaponIcon : null, i);
        }
        OnWeaponSwitched?.Invoke(activeWeaponIndex);
        if (currentWeapon != null) currentWeapon.RefreshAmmoUI();
    }

    private void BuildWeapons(int categoryIndex)
    {
        if (weaponSlots[0] != null) return;
        if (categoryIndex < 0 || categoryIndex >= availableCategories.Length) return;

        foreach (WeaponBase startingWeapon in availableCategories[categoryIndex].weapons)
        {
            AddWeapon(startingWeapon);
        }

        if (HasInputAuthority) ForceUIUpdate();
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
                weaponClone.InjectCamera(playerCamera);
                weaponClone.SetupNetworkRole(HasInputAuthority, HasStateAuthority, Object.InputAuthority);
                weaponSlots[i] = weaponClone;
                if (!HasInputAuthority)
                {
                    SetLayerRecursively(weaponClone.gameObject, LayerMask.NameToLayer("Default"));
                }

                if (HasInputAuthority)
                {
                    OnWeaponIconReady?.Invoke(weaponClone.weaponIcon, i);
                }
                return;
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void ApplyWeaponSwitchVisuals(int newIndex)
    {
        if (weaponSlots[newIndex] == null) return;

        foreach (WeaponBase weapon in weaponSlots)
        {
            if (weapon != null)
            {
                weapon.SetAiming(false);
                weapon.gameObject.SetActive(false);
            }
        }
        weaponSlots[newIndex].gameObject.SetActive(true);
        currentWeapon = weaponSlots[newIndex];
        if (HasInputAuthority)
        {
            OnWeaponIconReady?.Invoke(currentWeapon.weaponIcon, newIndex);
            OnWeaponSwitched?.Invoke(newIndex);
            currentWeapon.RefreshAmmoUI();
        }
    }
}