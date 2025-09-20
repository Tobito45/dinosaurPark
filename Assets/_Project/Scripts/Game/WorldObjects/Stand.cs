using Inventory;
using Library;
using Museum;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Stand : NetworkBehaviour, IPlaceItem
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private TMP_InputField _text;
    [SerializeField]
    private Material _hoverMat;
    [field: SerializeField]
    public Transform Point;

    private NetworkObject _placed;
    public NetworkObject Placed => _placed;
    public ItemRuntimeInfo Info => _placed.GetComponent<ItemPickup>().RuntimeInfo;
    private GameObject _hovered;

    public bool CheckIfCanPlaceItem(ItemRuntimeInfo id)
    {
        if (InventoryItemsLibrary.GetItem(id.Name) is InventoryPlacementItemLibrary)
            return false;
        
        return _placed == null;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        FindFirstObjectByType<MuseumController>().AddNewStand(this);
    }

    private void Update()
    {
        if (_placed == null || !_placed.IsSpawned)
        {
            _collider.enabled = true;
            _text.text = string.Empty;
        }
    }

    public void PlaceItem(ItemRuntimeInfo info) => RequestSummonRpc(info);

    [Rpc(SendTo.Server)]
    private void RequestSummonRpc(ItemRuntimeInfo info)
    {
        var item = Instantiate(InventoryItemsLibrary.GetItem(info.Name).NetworkObject, _target.position, Quaternion.identity);
        item.Spawn();
        SetEveryonePlacedRpc(item, info);
    }

    [Rpc(SendTo.Everyone)]
    private void SetEveryonePlacedRpc(NetworkObjectReference itemRef, ItemRuntimeInfo info)
    {
        if (itemRef.TryGet(out NetworkObject netObj))
        {
            _placed = netObj;
            _collider.enabled = false;
            _text.text = InventoryItemsLibrary.GetItem(info.Name).ItemName;
            netObj.GetComponent<ItemPickup>().RuntimeInfo = info;
        }
    }

    public void OnHoverEnter(ItemRuntimeInfo id)
    {
        if (_hovered != null)
            return;

        _hovered = Instantiate(InventoryItemsLibrary.GetItem(id.Name).NetworkObject, _target.position, Quaternion.identity).gameObject;

        foreach (var colider in _hovered.GetComponentsInChildren<Collider>())
            colider.enabled = false;

        foreach (var renderer in _hovered.GetComponentsInChildren<Renderer>())
            renderer.material = _hoverMat;
    }

    public void OnHoverExit()
    {
        if(_hovered == null)
            return;

        Destroy(_hovered);
        _hovered = null;
    }
}
