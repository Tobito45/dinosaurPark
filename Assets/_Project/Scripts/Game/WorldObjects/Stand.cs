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
    private GameObject _hovered;

    public bool CheckIfCanPlaceItem(string id) => _placed == null;


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

    public void PlaceItem(string id) => RequestSummonRpc(id);

    [Rpc(SendTo.Server)]
    private void RequestSummonRpc(string id)
    {
        var item = Instantiate(InventoryItemsLibrary.GetItem(id).NetworkObject, _target.position, Quaternion.identity);
        item.Spawn();
        SetEveryonePlacedRpc(item, id);
    }

    [Rpc(SendTo.Everyone)]
    private void SetEveryonePlacedRpc(NetworkObjectReference itemRef, string id)
    {
        if (itemRef.TryGet(out NetworkObject netObj))
        {
            _placed = netObj;
            _collider.enabled = false;
            _text.text = InventoryItemsLibrary.GetItem(id).ItemName;
        }
    }

    public void OnHoverEnter(string id)
    {
        if (_hovered != null)
            return;

        _hovered = Instantiate(InventoryItemsLibrary.GetItem(id).NetworkObject, _target.position, Quaternion.identity).gameObject;

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
