using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public MeshRenderer playerExhibitionMesh; 
    public Material[] availableSkins; 

    private void Start()
    {
        int savedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);
        ApplySkinToMannequin(savedSkin);
    }
    public void SelectSkin(int skinIndex)
    {
        if (skinIndex >= 0 && skinIndex < availableSkins.Length)
        {
            ApplySkinToMannequin(skinIndex);
            PlayerPrefs.SetInt("SelectedSkin", skinIndex);
            PlayerPrefs.Save();
        }
    }
    private void ApplySkinToMannequin(int index)
    {
        if (playerExhibitionMesh != null && availableSkins.Length > 0)
        {
            playerExhibitionMesh.material = availableSkins[index];
        }
    }
}