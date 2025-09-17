using ConstantLibrary;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class ItemRuntimeInfo : INetworkSerializable
{
    private string _name;
    public string Name { get => _name; private set => _name = value; }

    private float _condition;
    public float Condition { get => _condition; private set => _condition = value; }

    private ItemRarityEnum _itemRarityEnum;
    public ItemRarityEnum ItemRarityEnum { get => _itemRarityEnum; private set => _itemRarityEnum = value; }

    public ItemRuntimeInfo(string name, float condition, ItemRarityEnum itemRarityEnum)
    {
        Condition = condition;
        ItemRarityEnum = itemRarityEnum;
        _name = name;
    }

    public ItemRuntimeInfo() { }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _condition);
        serializer.SerializeValue(ref _itemRarityEnum);
        serializer.SerializeValue(ref _name);
    }
}
