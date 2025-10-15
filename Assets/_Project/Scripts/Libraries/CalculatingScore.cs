using System;
using System.Collections.Generic;
using Steamworks.Ugc;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using ConstantLibrary;
using NPC;
using Library;
using System.Linq;

public static class CalculatingScore
{
    private static readonly float w_type = 0.4f;
    private static readonly float w_rarity = 0.2f;
    private static readonly float w_era = 0.3f;
    private static readonly float w_condition = 0.1f;

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

        //float score = Mathf.Clamp(CalculateScore(npc, item), 0f, 100f);
        if (score > 100)
            score = 100f;
        float rnd = UnityEngine.Random.Range(0f, 1f);

        // Центры зон
        float[] centers = { 10f, 30f, 50f, 70f, 90f };
        // Ширина размытия (чем больше — тем плавнее переход)
        float spread = 20f;

        // Вычисляем "вес" каждой зоны по гауссовой кривой
        float[] weights = new float[5];
        for (int i = 0; i < centers.Length; i++)
        {
            float diff = (score - centers[i]) / spread;
            weights[i] = Mathf.Exp(-diff * diff); // e^(-x²)
        }
        Debug.Log("Weights:" + string.Join("|", weights));
        // Нормализуем, чтобы сумма = 1
        float sum = weights.Sum();
        for (int i = 0; i < weights.Length; i++)
            weights[i] /= sum;
        Debug.Log("Weights after sum:" + string.Join("|", weights));
        // Выбираем реакцию по весам
        float cumulative = rnd;


        Debug.Log("Cumulative:" + cumulative);
        if ((cumulative -= weights[0]) < 0) return ReactionsEnum.StronglyNegative;
        if ((cumulative -= weights[1]) < 0) return ReactionsEnum.Negative;
        if ((cumulative -= weights[2]) < 0) return ReactionsEnum.Middle;
        if ((cumulative -= weights[3]) < 0) return ReactionsEnum.Positive;
        return ReactionsEnum.StronglyPositive;
        // Basic method
        // if (score < 20)
        //     return ReactionsEnum.StronglyNegative;
        // else if (score <= 40)
        //     return ReactionsEnum.Negative;
        // else if (score <= 60)
        //     return ReactionsEnum.Middle;
        // else if (score <= 80)
        //     return ReactionsEnum.Positive;
        // else
        //     return ReactionsEnum.StronglyPositive;
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
