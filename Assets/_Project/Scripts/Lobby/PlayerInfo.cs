using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    public string steamName;
    public ulong steamId;
    public GameObject readyImage;
    public bool isReady;

    private void Start()
    {
        readyImage.SetActive(false);
        playerName.text = steamName;
    }
}
