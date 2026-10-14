using UnityEngine;
using TMPro;


public class RoomInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomCodeText;

    private void Start()
    {
        roomCodeText.text = "Room Code: " + BasicSpawner.Instance.CurrentRoomCode;
    }
}
