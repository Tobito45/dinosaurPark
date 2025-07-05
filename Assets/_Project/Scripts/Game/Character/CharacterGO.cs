using Steamworks;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterGO : NetworkBehaviour
{
    [SerializeField]
    private GameObject _body, _underGround, _canvas;

    [SerializeField]
    private CharacterMovement _characterMovement;

    [SerializeField]
    private Camera _playerCamera;

    [SerializeField]
    private TextMeshPro _nickName; 

    private void Start()
    {
        if (IsOwner)
            if (_body.activeSelf)
                OwnerInit();

        if (!IsOwner)
            NotOwnerInit();
    }

    private void NotOwnerInit()
    {
        _playerCamera.gameObject.SetActive(false);
        _underGround.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
        StartCoroutine(SetDataAfter());
    }

    private void OwnerInit()
    {
        GameClientsNerworkInfo.Singleton.AddPlayerRpc(OwnerClientId, SteamClient.Name, SteamClient.SteamId); //???
        _body.SetActive(false);
        _nickName.gameObject.SetActive(false);
    }

    private IEnumerator SetDataAfter()
    {
        yield return new WaitForSeconds(1);
        _nickName.text = GameClientsNerworkInfo.Singleton.GetPlayer(OwnerClientId).name;
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Debug.Log(GameClientsNerworkInfo.Singleton.ToString());
    }


}
