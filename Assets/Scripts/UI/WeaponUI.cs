using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public static WeaponUI Instance { get; private set; }
    public TMP_Text currentBullets;
    public TMP_Text totalBullets;
    public Image weaponIconDisplay;
    public Image ammoIconDisplay;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void UpdateCurrent(int newCurrentBullets)
    {
        currentBullets.text = newCurrentBullets.ToString();
    }
    public void UpdateTotal(int newTotalBullets)
    {
        totalBullets.text = newTotalBullets.ToString();
    }
    public void UpdateBoth(int current, int total)
    {
        UpdateCurrent(current);
        UpdateTotal(total);
    }
    public void UpdateWeaponIcon(Sprite newIcon, Sprite newAmmoIcon)
    {
        if (weaponIconDisplay != null && newIcon != null && newAmmoIcon != null)
        {
            weaponIconDisplay.sprite = newIcon;
            ammoIconDisplay.sprite = newAmmoIcon;
        }
    }
}