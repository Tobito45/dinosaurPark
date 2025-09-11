using Inventory;
using Steamworks;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public class CharacterGO : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _body, _underGround, _canvas;

        [field:SerializeField]
        public CharacterMovement CharacterMovement { get; private set; }

        [field: SerializeField]
        public PlayerInventoryController PlayerInventoryController { get; private set; }

        [SerializeField]
        private Camera _playerCamera;
        public Camera MainCamere => _playerCamera;

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
            GameClientsNerworkInfo.Singleton.MainPlayer = this;

            GameClientsNerworkInfo.Singleton.CharacterPermissions.SetBasePermissions();
            _body.SetActive(false);
            _nickName.gameObject.SetActive(false);

            PlayerInventoryController = GetComponent<PlayerInventoryController>();
        }

        private IEnumerator SetDataAfter()
        {
            yield return new WaitForSeconds(1);
            _nickName.text = GameClientsNerworkInfo.Singleton.GetPlayer(OwnerClientId).name;
            enabled = false;
        }
    }
}