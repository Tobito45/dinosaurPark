  using Inventory;
using Steamworks;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public class CharacterGO : NetworkBehaviour, IPlayerProxy
    {
        [SerializeField]
        private GameObject _body, _underGround, _canvas;

        [SerializeField]
        private CharacterMovement _characterMovement;

        [SerializeField]
        private PlayerInventoryController _playerInventoryController;

        [SerializeField]
        private HealthController _healthController;

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

            UserPermissions.Singleton.SetBasePermissions();
            _body.SetActive(false);
            _nickName.gameObject.SetActive(false);

            _playerInventoryController = GetComponent<PlayerInventoryController>();
        }

        private IEnumerator SetDataAfter()
        {
            yield return new WaitForSeconds(1);
            _nickName.text = GameClientsNerworkInfo.Singleton.GetPlayer(OwnerClientId).name;
            enabled = false;
        }

        public void AddOnHealthChange(Action<int> action) => _healthController.OnHealthChange += action;

        public GameObject GetMainPlayer() => gameObject;

        public void DealDamage(int damage) => _healthController.DealDmg(damage);

        public void TeleportToPoint(Vector3 point) => _characterMovement.TeleportToPoint(point);

        public bool PutItemToInventory(ItemRuntimeInfo info) => _playerInventoryController.PutItemToList(info);

        public void DropItemFromInventory() => _playerInventoryController.DropItem();

        public Camera GetPlayerCamera() => _playerCamera;

        public Transform GetPlayerCameraTransform() => _playerCamera.transform;
    }
} 