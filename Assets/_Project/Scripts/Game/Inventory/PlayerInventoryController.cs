using NUnit.Framework;
using System.Drawing;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace Inventory
{
    public class PlayerInventoryController : NetworkBehaviour
    {
        private const int BASIC_INDEX_NOT_SELECTED = -1;
        private const int MAX_INVENTAR_COUNT = 5;

        [Header("Settings")]
        [SerializeField]
        private Camera playerCamera;
        [SerializeField]
        private float pickupRange = 3f;
        [SerializeField]
        private NetworkObject _prefabSumon;
        [SerializeField]
        private LayerMask summonLayerMask;

        [Header("References")]
        [SerializeField]
        private InventoryUIController _inventoryUIController;

        [SerializeField]
        private ItemPickUpInfoShower _itemPickUpInfoShower;

        private IPlaceItem _lastHoveredItem;
        private IInteractable _lastHoveredInteractable;
        private int[] _items = new int[MAX_INVENTAR_COUNT];
        private int _selectedItem = BASIC_INDEX_NOT_SELECTED;

        private void Start()
        {
            if (!IsOwner)
                return;

            _selectedItem = BASIC_INDEX_NOT_SELECTED;

            for (int i = 0; i < _items.Length; i++)
                _items[i] = BASIC_INDEX_NOT_SELECTED;
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (!GameClientsNerworkInfo.Singleton.CharacterPermissions.HasPermission(Character.CharacterPermissionsType.Input))
                return;

            UpdatePickUp();

            UpdateSummonItem();

            UpdateInteract();

            UpdateSelectItemInInventory();
            UpdateSelectMouseWheelItemInInventory();
        }

        private void UpdatePickUp()
        {
            ItemPickup item = RaycastToPickUp();

            if (item != null && Input.GetKeyDown(KeyCode.F))
                TryPickupItem(item);
        }

        private void UpdateSummonItem()
        {
            var (placeItem, point) = RaycastToSummonItem();

            if (_lastHoveredItem != null && placeItem != _lastHoveredItem)
                _lastHoveredItem.OnHoverExit();

            _lastHoveredItem = placeItem;

            if (Input.GetKeyDown(KeyCode.G))
                TrySummonItem(placeItem, point);
        }

        private void UpdateInteract()
        {
            if (_lastHoveredInteractable != null && Input.GetKeyUp(KeyCode.R))
                _lastHoveredInteractable.OnInteractUp();

            IInteractable interactable = RaycastToInteract();

            if (!Input.GetKey(KeyCode.R))
            {
                if (_lastHoveredInteractable != null && interactable != _lastHoveredInteractable)
                    _lastHoveredInteractable.OnHoverExit();

                _lastHoveredInteractable = interactable;
            }

            if (interactable != null && Input.GetKeyDown(KeyCode.R))
                TryInteract(interactable);
        }

        private void UpdateSelectItemInInventory()
        {
            if (!IsOwner) return;

            for (int i = 0; i < MAX_INVENTAR_COUNT; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    if (_selectedItem == i)
                    {
                        _inventoryUIController.DeSelectItem(_selectedItem);
                        _selectedItem = BASIC_INDEX_NOT_SELECTED;
                    }
                    else
                    {
                        _selectedItem = i;
                        _lastHoveredItem?.OnHoverExit();
                        _inventoryUIController.SelectItem(_selectedItem);
                    }
                    return;
                }
            }
        }

        private void UpdateSelectMouseWheelItemInInventory()
        {
            if (!IsOwner) return;

            float scroll = Input.mouseScrollDelta.y;

            if (scroll != 0)
            {
                if (_selectedItem != BASIC_INDEX_NOT_SELECTED)
                    _inventoryUIController.DeSelectItem(_selectedItem);

                _selectedItem += scroll > 0 ? 1 : -1;
                if (_selectedItem >= MAX_INVENTAR_COUNT)
                    _selectedItem = 0;
                else if (_selectedItem < 0)
                    _selectedItem = MAX_INVENTAR_COUNT - 1;

                _inventoryUIController.SelectItem(_selectedItem);
                _lastHoveredItem?.OnHoverExit();
            }
        }

        private ItemPickup RaycastToPickUp()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
                if (hit.collider.TryGetComponent<ItemPickup>(out var item))
                {
                    _itemPickUpInfoShower.ActivePanelPick(InventoryItemsLibrary.GetIInventoryItem(item.ItemId));
                    return item;
                }

            _itemPickUpInfoShower.DeactivePanel();
            return null;
        }

        private void TryPickupItem(ItemPickup item)
        {
            if (PutItemToList(item.ItemId))
                RequestPickupRpc(item.NetworkObject);
        }

        private bool PutItemToList(int id)
        {
            if (_selectedItem == BASIC_INDEX_NOT_SELECTED)
            {
                for(int i = 0; i < _items.Length; i++)
                {
                    if (_items[i] == BASIC_INDEX_NOT_SELECTED)
                    {
                        _items[i] = id;
                        _inventoryUIController.PutItem(i, InventoryItemsLibrary.GetIInventoryItem(id));
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (_items[_selectedItem] != BASIC_INDEX_NOT_SELECTED)
                    return false;

                _items[_selectedItem] = id;
                _inventoryUIController.PutItem(_selectedItem, InventoryItemsLibrary.GetIInventoryItem(id));
                return true;
            }
        }

        private (bool removed, int id) RemoveItemFromList()
        {
            if (_selectedItem == BASIC_INDEX_NOT_SELECTED)
                return (false, BASIC_INDEX_NOT_SELECTED);

            int savedId = _items[_selectedItem];
            _items[_selectedItem] = BASIC_INDEX_NOT_SELECTED;
            _inventoryUIController.ResetItem(_selectedItem);
            return (true, savedId);
        }


        private (IPlaceItem obj, Vector3? point) RaycastToSummonItem()
        {
            if (_selectedItem == BASIC_INDEX_NOT_SELECTED || _items[_selectedItem] == BASIC_INDEX_NOT_SELECTED)
                return (null, null);

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, summonLayerMask))
            {
                IPlaceItem placeItem = hit.collider.GetComponent<IPlaceItem>();
                if (placeItem != null)
                {
                    if (!placeItem.CheckIfCanPlaceItem(_items[_selectedItem]))
                        return (null, null);

                    _itemPickUpInfoShower.ActivePanelPlace(InventoryItemsLibrary.GetIInventoryItem(_items[_selectedItem]));
                    placeItem.OnHoverEnter(_items[_selectedItem]);
                    return (placeItem, hit.point);
                    
                }
                else
                    return (null, hit.point);
            }
            return (null, null);
        }
        private void TrySummonItem(IPlaceItem obj, Vector3? point)
        {
            if (obj == null && point == null)
                return;

            if(obj != null)
            {
                var resultObj = RemoveItemFromList();
                obj.OnHoverExit();
                obj.PlaceItem(resultObj.id);
                return;
            }

            var result = RemoveItemFromList();
            if (result.removed)
                RequestSummonRpc(point.Value.x, 1, point.Value.z, result.id);

            return;
        }


        public IInteractable RaycastToInteract()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
                if (hit.collider.TryGetComponent<IInteractable>(out var item))
                {
                    if (!item.CanBeInteracted())
                        return null;

                    _itemPickUpInfoShower.ActivePanelInteract(item.GetInteractText());
                    item.OnHoverEnter();
                    return item;
                }

            return null;
        }

        private void TryInteract(IInteractable item)
        {
            item.OnInteractDown();
            item.OnHoverExit();
        }

        [Rpc(SendTo.Server)]
        private void RequestPickupRpc(NetworkObjectReference itemRef)
        {
            if (itemRef.TryGet(out NetworkObject netObj))
            {
                var item = netObj.GetComponent<ItemPickup>();
                if (item != null)
                    item.Pickup(OwnerClientId);
            }
        }

        [Rpc(SendTo.Server)]
        private void RequestSummonRpc(float x, float y, float z, int id)
        {
            var item = Instantiate(InventoryItemsLibrary.GetIInventoryItem(id).NetworkObject, new Vector3(x, y, z), Quaternion.identity);
            item.Spawn();
        }
    }
}
