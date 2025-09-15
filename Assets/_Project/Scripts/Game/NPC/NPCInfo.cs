using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ConstantLibrary;
using UnityEngine;
using UnityEngine.AI;


namespace NPC
{
    [CreateAssetMenu(fileName = "NPCInfo", menuName = "Scriptable Objects/NPCInfo")]
    public class NPCInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public int Skin { get; private set; }

        [field: SerializeField]
        public float Money { get; private set; }

        [field: SerializeField]
        public int Priorty { get; private set; }

        [SerializeField]
        private int[] _way;
        public IEnumerable<int> Way => _way;

        [SerializeField]
        private List<ItemType> _itemTypePriority = new List<ItemType>();

        private Dictionary<ItemTypeEnum, float> dictTypePriority;

        [SerializeField]
        private List<EraType> _itemEraPriority = new List<EraType>();

        private Dictionary<EraNameEnum, float> dictEraPriority;

        [SerializeField]
        private List<RarityType> _itemRarityPriority = new List<RarityType>();

        private Dictionary<ItemRarityEnum, float> dictRarityPriority;

        //More is better reaction to this one. 
        [Header("Condition Priority")]
        [field: SerializeField]
        public float ConditionPriority { get; private set; } = 0f;

        // Auto-fill all enum values
        // Ensure all enum values are present in the list
        public void OnBeforeSerialize()
        {
            Array enumValues = Enum.GetValues(typeof(ItemTypeEnum));

            foreach (ItemTypeEnum e in enumValues)
            {
                if (!_itemTypePriority.Exists(x => x.Key == e))
                {
                    _itemTypePriority.Add(new ItemType(e, 0f));
                }
            }

            Array enumValuesEra = Enum.GetValues(typeof(EraNameEnum));

            foreach (EraNameEnum e in enumValuesEra)
            {
                if (!_itemEraPriority.Exists(x => x.Key == e))
                {
                    _itemEraPriority.Add(new EraType(e, 0f));
                }
            }
            Array enumValuesRarity = Enum.GetValues(typeof(ItemRarityEnum));
            float[] raryties = new float[4] { 20f, 50f, 80f, 100f };
            int i = 0;
            foreach (ItemRarityEnum e in enumValuesRarity)
            {
                if (!_itemRarityPriority.Exists(x => x.Key == e))
                {
                    _itemRarityPriority.Add(new RarityType(e, raryties[i++]));
                }
            }
        }
        //Need to check how it works
        public void OnAfterDeserialize()
        {
            dictTypePriority = new Dictionary<ItemTypeEnum, float>();
            foreach (var entry in _itemTypePriority)
            {
                if (!dictTypePriority.ContainsKey(entry.Key))
                    dictTypePriority.Add(entry.Key, entry.Value);
            }
            dictRarityPriority = new Dictionary<ItemRarityEnum, float>();
            foreach (var entry in _itemRarityPriority)
            {
                if (!dictRarityPriority.ContainsKey(entry.Key))
                    dictRarityPriority.Add(entry.Key, entry.Value);
            }
            dictEraPriority = new Dictionary<EraNameEnum, float>();
            foreach (var entry in _itemEraPriority)
            {
                if (!dictEraPriority.ContainsKey(entry.Key))
                    dictEraPriority.Add(entry.Key, entry.Value);
            }

        }
        public float GetTypePrioretyByEnunName(ItemTypeEnum nameEnum)
        {
            return dictTypePriority[nameEnum];
        }
        public bool IsTypePrioretyContainsKey(ItemTypeEnum nameEnum)
        {
            return dictTypePriority.ContainsKey(nameEnum);
        }
         public bool IsEraPrioretyContainsKey(EraNameEnum nameEnum)
        {
            return dictEraPriority.ContainsKey(nameEnum);
        }
        public float GetEraPrioretyByEnunName(EraNameEnum nameEnum)
        {
            return dictEraPriority[nameEnum];
        }
        public float GetRarityPrioretyByEnunName(ItemRarityEnum nameEnum)
        {
            return dictRarityPriority[nameEnum];
        }
        public bool IsRarityPrioretyContainsKey(ItemRarityEnum nameEnum)
        {
            return dictRarityPriority.ContainsKey(nameEnum);
        }
    }
    [Serializable]
    internal class ItemType
    {
        [field: SerializeField]
        public ItemTypeEnum Key { get; private set; }

        [field: SerializeField]
        public float Value { get; private set; } = 0f;

        public ItemType(ItemTypeEnum key, float value)
        {
            Key = key;
            Value = value;
        }
    }
    [Serializable]
    internal class EraType
    {
        [field: SerializeField]
        public EraNameEnum Key { get; private set; }

        [field: SerializeField]
        public float Value { get; private set; } = 0f;

        public EraType(EraNameEnum key, float value)
        {
            Key = key;
            Value = value;
        }
    }
    [Serializable]
    public class RarityType
    {
        [field: SerializeField]
        public ItemRarityEnum Key { get; private set; }

        [field: SerializeField]
        public float Value { get; private set; } = 0f;

        public RarityType(ItemRarityEnum key, float value)
        {
            Key = key;
            Value = value;
        }
    }

}
