using System;
using System.Collections.Generic;
using Steamworks.Ugc;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using ConstantLibrary;
using NPC;
using Library;

public static class CalculatingScore
{
    private static readonly float w_type = 0.4f;
    private static readonly float w_rarity = 0.2f;
    private static readonly float w_era = 0.2f;
    private static readonly float w_condition = 0.2f;

    public static ReactionsEnum GetReactEmotion(NPCInfo npc, ItemRuntimeInfo item)
    {

        // EXAMPLE
        // Инициализация NPC
        // NPC npcHrant = new NPC(
        //     name: "Грант",
        //     engType: new Dictionary<string, float>
        //     {
        //         { ItemTypeEnum.Gems.ToString(), 70f }, { ItemTypeEnum.Living.ToString(), 50f }, { ItemTypeEnum.Archaeological.ToString(), 40f },
        //         { ItemTypeEnum.Food.ToString(), 10f }, { ItemTypeEnum.Trash.ToString(), 5f }, { ItemTypeEnum.Plant.ToString(), 20f }
        //     },
        //     engRarity: new Dictionary<string, float>
        //     {
        //         { ItemRarityEnum.Common.ToString(), 20f }, { ItemRarityEnum.Rare.ToString(), 50f }, { ItemRarityEnum.Epic.ToString(), 80f }, { ItemRarityEnum.Legendary.ToString(), 100f }
        //     },
        //     engEra: new Dictionary<string, float>
        //     {
        //         { EraNameEnum.Triassic.ToString(), 80f }, { EraNameEnum.None.ToString(), 10f }
        //     },
        //     engCondition: 70f
        // );
        // Item itemRubin = new Item("Рубин", ItemRarityEnum.Common.ToString(), 80f, EraNameEnum.Triassic.ToString(), new List<string> { ItemTypeEnum.Gems.ToString() });
        // //items.Add(new Item("Коробка от сока", ItemRarityEnum.Common.ToString(), 30f, EraNameNum.None.ToString(), new List<string> { ItemTypeEnum.Food.ToString(), ItemTypeEnum.Trash.ToString() }));



        float score = CalculateScore(npc, item);
        if (score < 33)
            return ReactionsEnum.Negative;
        else if (score <= 66)
            return ReactionsEnum.Middle;
        else
            return ReactionsEnum.Positive;
    }

    //NPC, ITEM
    public static float CalculateScore(NPCInfo npc, ItemRuntimeInfo item)
    {

        // Type score: среднее по всем типам (0 если тип не в словаре)
        float typeScore = 0f;
        foreach (var type in InventoryItemsLibrary.GetItem(item.Name).ItemTypes)
        {
            typeScore += npc.IsTypePrioretyContainsKey(type) ? npc.GetTypePrioretyByEnunName(type) : 0f;
        }
        typeScore /= InventoryItemsLibrary.GetItem(item.Name).ItemTypes.Count;
        Debug.Log("TYPE SCORE:" + typeScore);

        // Rarity score
        float rarityScore = npc.IsRarityPrioretyContainsKey(item.ItemRarityEnum) ? npc.GetRarityPrioretyByEnunName(item.ItemRarityEnum) : 0f;
        Debug.Log("rarityScore:" + rarityScore);
        // Era score
        float eraScore = npc.IsEraPrioretyContainsKey(InventoryItemsLibrary.GetItem(item.Name).Era) ? npc.GetEraPrioretyByEnunName(InventoryItemsLibrary.GetItem(item.Name).Era) : 0f;
        Debug.Log("eraScore:" + eraScore);
        // Condition score
        float conditionScore = item.Condition / 100f * npc.ConditionPriority;
        Debug.Log("conditionScore:" + conditionScore);
        // Общий score с весами: 0.4 type, 0.2 rarity, 0.2 era, 0.2 condition
        float score = (w_type * typeScore) + (w_rarity * rarityScore) + (w_era * eraScore) + (w_condition * conditionScore);
        Debug.Log("SCORE:" + score);
        return score;
    }
    // public class Item
    // {
    //     public string Name { get; set; }
    //     public string Rarity { get; set; }
    //     public float Condition { get; set; } // 0-100
    //     public string Era { get; set; }
    //     public List<string> Types { get; set; }

    //     public Item(string name, string rarity, float condition, string era, List<string> types)
    //     {
    //         Name = name;
    //         Rarity = rarity;
    //         Condition = condition;
    //         Era = era;
    //         Types = types;
    //     }
    // }

    // public class NPC
    // {
    //     public string Name { get; set; }
    //     public Dictionary<string, float> EngType { get; set; } // Вовлеченность по типам
    //     public Dictionary<string, float> EngRarity { get; set; } // Вовлеченность по редкости
    //     public Dictionary<string, float> EngEra { get; set; } // Вовлеченность по эре
    //     public float EngCondition { get; set; } // Чувствительность к состоянию

    //     public NPC(string name, Dictionary<string, float> engType, Dictionary<string, float> engRarity,
    //                Dictionary<string, float> engEra, float engCondition)
    //     {
    //         Name = name;
    //         EngType = engType;
    //         EngRarity = engRarity;
    //         EngEra = engEra;
    //         EngCondition = engCondition;
    //     }
    // }
}
