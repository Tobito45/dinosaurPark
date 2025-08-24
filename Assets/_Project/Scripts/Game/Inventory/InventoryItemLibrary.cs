using Placement;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemLibrary", menuName = "Scriptable Objects/InventoryItemLibrary")]
public class InventoryItemLibrary : ScriptableObject
{
    [field: SerializeField]
    public int Index { get; private set; }

    [field: SerializeField]
    public NetworkObject NetworkObject { get; private set; }

    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField]
    public string Description { get; private set; }

    [field: SerializeField]
    public Color32 Color { get; private set; }

    public virtual void OnSelect() { }

    public virtual void OnDeselect() { }
}

