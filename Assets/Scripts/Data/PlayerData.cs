using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public MeshRenderer myPlayerMesh;
    public Material[] availableSkins;
    private void Start()
    {
        LoadCosmetics();
    }
    private void LoadCosmetics()
    {
        int savedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
        if (myPlayerMesh != null && availableSkins.Length > savedSkinIndex)
        {
            myPlayerMesh.material = availableSkins[savedSkinIndex];
        }
    }
}