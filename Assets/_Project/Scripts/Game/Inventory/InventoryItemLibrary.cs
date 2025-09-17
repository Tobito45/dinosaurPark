
using ConstantLibrary;
using Library;
using NPC;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemLibrary", menuName = "Scriptable Objects/InventoryItemLibrary")]
public class InventoryItemLibrary : ScriptableObject, ILibraryKey
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

    public string Key => ItemName;

    [field: SerializeField]
    private List<RarityType> ItemRarityPriority = new List<RarityType>();
    public ItemRarityEnum ItemQualityGenerated { get; private set; }

    [field: SerializeField]
    public EraNameEnum Era { get; private set; } = EraNameEnum.None;

    [field: SerializeField]
    public List<ItemTypeEnum> ItemTypes { get; private set; }

    [field: SerializeField]
    private MinMax ConditionMinMax;
    public float ConditionQuality { get; set; }

    [field: SerializeField]
    public int ChanceSpawn {get; private set;}

    public virtual void OnSelect() { }

    public virtual void OnDeselect() { }
    // Run this function to generate random rarity for item
    public void SetCurrentRarity()
    {
        float totalWeight = 0;
        foreach (var entry in ItemRarityPriority)
            totalWeight += entry.Value;

        int roll = (int)Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var entry in ItemRarityPriority)
        {
            cumulative += entry.Value;
            if (roll < cumulative)
                ItemQualityGenerated = entry.Key;
        }

        // fallback (shouldn't happen if weights > 0)
        ItemQualityGenerated = ItemRarityPriority[0].Key;
    }
    public void SetConditionQuality()
    {
        ConditionQuality = Random.Range(ConditionMinMax.min, ConditionMinMax.max);
    }

    public float GetRandomQuality() => Random.Range(ConditionMinMax.min, ConditionMinMax.max);

    public ItemRarityEnum GetRandomRarity()
    {
        float total = ItemRarityPriority.Sum(r => r.Value);

        float randomPoint = Random.Range(0f, total);

        float current = 0f;
        foreach (var rarity in ItemRarityPriority)
        {
            current += rarity.Value;
            if (randomPoint <= current)
                return rarity.Key;
        }

        return ItemRarityPriority.Last().Key;
    }

}
[System.Serializable]
internal struct MinMax
{
    [field: SerializeField]
    public float min;
    [field: SerializeField]
    public float max;
}
