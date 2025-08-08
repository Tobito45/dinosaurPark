using Inventory;
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

    private NetworkObject _placed;
    public NetworkObject Placed => _placed;
    private GameObject _hovered;

    public bool CheckIfCanPlaceItem(int id) => _placed == null;

    private void Update()
    {
        if (_placed == null || !_placed.IsSpawned)
        {
            _collider.enabled = true;
            _text.text = string.Empty;
        }
    }

    public void PlaceItem(int id) => RequestSummonRpc(id);

    [Rpc(SendTo.Server)]
    private void RequestSummonRpc(int id)
    {
        var item = Instantiate(InventoryItemsLibrary.GetIInventoryItem(id).NetworkObject, _target.position, Quaternion.identity);
        item.Spawn();
        SetEveryonePlacedRpc(item, id);
    }

    [Rpc(SendTo.Everyone)]
    private void SetEveryonePlacedRpc(NetworkObjectReference itemRef, int id)
    {
        if (itemRef.TryGet(out NetworkObject netObj))
        {
            _placed = netObj;
            _collider.enabled = false;
            _text.text = InventoryItemsLibrary.GetIInventoryItem(id).ItemName;
        }
    }

    public void OnHoverEnter(int id)
    {
        if (_hovered != null)
            return;

        _hovered = Instantiate(InventoryItemsLibrary.GetIInventoryItem(id).NetworkObject, _target.position, Quaternion.identity).gameObject;
        
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
