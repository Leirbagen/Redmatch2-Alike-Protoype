using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public static WeaponUI Instance { get; private set; }
    public TMP_Text currentBullets;
    public TMP_Text totalBullets;
    public Image[] weaponIcons;
    private Color activeColor = new Color(1f, 1f, 1f, 1f);
    private Color inactiveColor = new Color(0.6f, 0.6f, 0.6f, 0.4f);
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
    public void UpdateActiveWeaponIndex(int activeIndex)
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i] != null)
            {
                if (i == activeIndex)
                {
                    weaponIcons[i].color = activeColor;
                }
                else
                {
                    weaponIcons[i].color = inactiveColor;
                }
            }
        }
    }
    public void ClearWeaponUI()
    {
        if (currentBullets != null) 
        {
            currentBullets.text = "--";
        }
        if (totalBullets != null) 
        {
            totalBullets.text = "--";
        } 
        UpdateActiveWeaponIndex(-1); 
    }
}