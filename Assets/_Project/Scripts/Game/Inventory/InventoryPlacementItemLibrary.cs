using Inventory;
using Placement;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryPlacementItemLibrary", menuName = "Scriptable Objects/InventoryPlacementItemLibrary")]
public class InventoryPlacementItemLibrary : InventoryItemLibrary
{
    [SerializeField]
    private Placement.Placement item;

    private PlacementController _placementController;
    private PlayerInventoryController _playerInventoryController;

    public override void OnSelect()
    {
        if (_placementController == null)
            _placementController = FindFirstObjectByType<PlacementController>();

        _placementController.ActiveByItem(item);
    }

    public override void OnDeselect()
    {
        if (_placementController == null)
            _placementController = FindFirstObjectByType<PlacementController>();

        _placementController.Disable();
    }
}