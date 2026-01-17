using Character;
using DI;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


namespace Placement
{
    public class PlacementController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlacementSelecter _placementSelecter;

        [Header("Scene References")]
        [SerializeField]
        private Transform _border;
        [SerializeField]
        private Transform _startPont, _parent;

        [SerializeField]
        private GameObject _placementPrefab;

        [SerializeField]
        private LayerMask placementMask;


        Dictionary<Vector3, PlacementInfo> _places = new();
        public bool IsPlacementModeOn => _parent.gameObject.activeSelf;
        private List<PlacementInfo> _lastSelected = new List<PlacementInfo>();

        [Inject]
        private PlayerProxy _characterFacade;

        private void Start() => Init();

        private void Update()
        {
            //if (playerCamera == null)
            //{
            //    playerCamera = GameClientsNerworkInfo.Singleton.MainPlayer?.MainCamere;
            //    _playerInventoryController = GameClientsNerworkInfo.Singleton.MainPlayer?.GetComponent<PlayerInventoryController>();
            //}

            if (_characterFacade == null)
                this.Inject();

            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    _parent.gameObject.SetActive(!_parent.gameObject.activeSelf);
            //    _placementSelecter.ActivePanel(_parent.gameObject.activeSelf);
            //}

            if (IsPlacementModeOn && _placementSelecter.SelectedPlacement != null)
            {
                Ray ray = new Ray(_characterFacade.GetPlayerCameraTransform().position, _characterFacade.GetPlayerCameraTransform().forward);

                if (Physics.Raycast(ray, out RaycastHit hit, 5f, placementMask))
                {
                    ResetLastSelected();

                    if (CanPlaceX(_placementSelecter.SelectedPlacement.Width, _placementSelecter.SelectedPlacement.Height, 
                            hit.collider.transform.position, out List<PlacementInfo> cells))
                    {
                        foreach (var mesh in cells)
                            mesh.IsOcucupied = true;

                        _lastSelected = cells;
                    }

                    if(Input.GetKeyDown(KeyCode.V) && _lastSelected != null)
                        PlaceItem(hit.transform);
                }
                else
                    ResetLastSelected();
            } else
                 ResetLastSelected();
        }

        public void ActiveByItem(Placement item)
        {
            _parent.gameObject.SetActive(true);
            _placementSelecter.SelectItem(item);
        }

        public void Disable()
        {
            _parent.gameObject.SetActive(false);
            _placementSelecter.DeselectItem();
        }

        private void PlaceItem(Transform point)
        {
            Debug.Log(PlacementLibrary.GetItem(_placementSelecter.SelectedPlacement.PlacmentName).Prefab.name);

            SpawnPlacementItemRPC(point.position, _placementSelecter.SelectedPlacement.PlacmentName);
            _placementSelecter.DeselectItem();

            foreach (PlacementInfo info in _lastSelected)
                ReservePointRPC(info.gameObject.transform.position);

            _lastSelected = null;
            _characterFacade.DropItemFromInventory();
            Disable();
        }

        [Rpc(SendTo.Server)]
        private void SpawnPlacementItemRPC(Vector3 vector3, string item)
        {
            Debug.Log(item);
            var obj = Instantiate(PlacementLibrary.GetItem(item).Prefab, vector3, Quaternion.identity);
            obj.Spawn();
        }

        [Rpc(SendTo.Everyone)]
        private void ReservePointRPC(Vector3 vector3)
        {
            if (_places[vector3].IsOcucupied)
                _lastSelected = null;
            _places[vector3].IsOcucupied = true;
        }

        private void ResetLastSelected()
        {
            if (_lastSelected != null)
            {
                foreach (var info in _lastSelected)
                    info.IsOcucupied = false;

                _lastSelected = null;
            }
        }

        public bool CanPlaceX(int width, int height, Vector3 start, out List<PlacementInfo> cells)
        {
            cells = new();
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (_places.TryGetValue(start + new Vector3(i, 0, j), out PlacementInfo value))
                    {
                        if (!value.IsOcucupied)
                            cells.Add(value);
                        else return false;
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        public void Init()
        {
            InitPlacements();
        }

        private void InitPlacements()
        {
            Vector3 offset = _startPont.position;
            while (true)
            {
                var obj = Instantiate(_placementPrefab,  offset, Quaternion.identity, _parent);

                _places.Add(offset, obj.GetComponent<PlacementInfo>());

                offset += new Vector3(0, 0, 1f);
                if(offset.z > _border.position.z)
                    offset = new Vector3(offset.x - 1, 0, _startPont.position.z);

                if (offset.x < _border.position.x)
                    break;
            }
        }
    }
}
