using Unity.VisualScripting;
using UnityEngine;
using Fusion;

public class PlayerDataManager : NetworkBehaviour
{
    public MeshRenderer myPlayerMesh;
    public Material[] availableSkins;
    public int currentWeaponCategory;

    [Networked, OnChangedRender(nameof(OnSkinChanged))] 
    public int SyncedSkinIndex { get; set; } = -1;

    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority)
        {
            int savedSkinIndex = PlayerPrefs.GetInt("SelectedSkin", 0);
            currentWeaponCategory = PlayerPrefs.GetInt("SelectedWeaponCategory", 0);
            RPC_SetSkin(savedSkinIndex);
        }
        if (SyncedSkinIndex != -1)
        {
            ApplySkinVisuals(SyncedSkinIndex);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetSkin(int skinIndex)
    {
        SyncedSkinIndex = skinIndex; 
        ApplySkinVisuals(skinIndex); 
    }

    private void OnSkinChanged()
    {
        ApplySkinVisuals(SyncedSkinIndex);
    }

    private void ApplySkinVisuals(int index)
    {
        if (myPlayerMesh != null && availableSkins != null && index >= 0 && index < availableSkins.Length)
        {
            myPlayerMesh.material = availableSkins[index];
        }
    }
}