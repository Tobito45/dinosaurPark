using System;
using System.Collections.Generic;
using System.Linq;
using Bootstrap;
using NPC;
using TMPro;
using Unity.Netcode;
using UnityEngine;



namespace GameUI
{
    public class StatisticsUI : MonoBehaviour, IInit
    {
        [SerializeField]
        private TextMeshProUGUI _textCountNegative, _textCountMiddle, _textCountPozitive, _textCountAll;
        [SerializeField]
        private GameObject _scrollNPCItems;
        [SerializeField]
        private GameObject _prefabNPCItem;

        private Dictionary<string, SpawnedItem> spawnedItemNPCScroll = new();

        private int _countNegative = 0, _countMiddle = 0, _countPozitive = 0, _countStronglyNegative = 0, _countStronglyPositive = 0, _countAll = 0;

        public void Init()
        {
            _textCountNegative.text = _countNegative.ToString();
            _textCountMiddle.text = _countMiddle.ToString();
            _textCountPozitive.text = _countPozitive.ToString();
            _textCountAll.text = _countAll.ToString();
            GenerateListNPCScroll();
        }

        public void OnStartWatching()
        {
            _countAll++;
            UpdateTextsRPC(_countMiddle, _countNegative, _countPozitive, _countAll);
        }

        public void OnNewEmothion(EmotionType emotion, string name)
        {
            SpawnedItem item = spawnedItemNPCScroll[name];

            switch (emotion)
            {
                case EmotionType.Happy:
                    _countPozitive++;
                    item.CountPositive++;
                    break;
                case EmotionType.Sad:
                    _countMiddle++;
                    item.CountNeutral++;
                    break;
                case EmotionType.Angry:
                    _countNegative++;
                    item.CountNegative++;
                    break;
                case EmotionType.MoreAngry:
                    _countStronglyNegative++;
                    item.CountStronglyNegative++;
                    break;
                case EmotionType.MoreHappy:
                    _countStronglyPositive++;
                    item.CountStronglyPositive++;
                    break;
            }
            UpdateTextsRPC(_countMiddle, _countNegative, _countPozitive, _countAll);

            UpdateTextsNPCRPC(item.CountNeutral, item.CountNegative, item.CountPositive, name);
        }

        [Rpc(SendTo.Everyone)]
        public void UpdateTextsRPC(int countMiddle, int countNegative, int countPozitive, int _countAll)
        {
            _textCountMiddle.text = countMiddle.ToString();
            _textCountNegative.text = countNegative.ToString();
            _textCountPozitive.text = countPozitive.ToString();
            _textCountAll.text = _countAll.ToString();
        }

        [Rpc(SendTo.Everyone)]
        public void UpdateTextsNPCRPC(int countMiddle, int countNegative, int countPositive, string name)
        {
            SpawnedItem item = spawnedItemNPCScroll[name];
            item.CountNegative = countNegative;
            item.CountPositive = countPositive;
            item.CountNeutral = countMiddle;

            UpdateNPCScrollItem(name);
        }

        private void GenerateListNPCScroll()
        {
            NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
            Debug.Log(npcs.Count());
            foreach (var npc in npcs)
            {
                GameObject newItem = Instantiate(_prefabNPCItem, _scrollNPCItems.transform);
                spawnedItemNPCScroll.Add(npc.GetNPCInfo().Name, new SpawnedItem(newItem, npc));

                // Example: if prefab has a TMP_Text component
                TMP_Text text = newItem.GetComponentsInChildren<TMP_Text>()[0];
                if (text != null)
                {
                    text.text = npc.GetNPCInfo().Name;
                }
            }
            UpdateNPCScrollItems();
        }

        public void UpdateNPCScrollItems()
        {
            foreach (var itemNpc in spawnedItemNPCScroll)
                UpdateTextItem(itemNpc.Value.UiObject, itemNpc.Value.NpcObject.GetNPCInfo().Name);
        }

        public void UpdateNPCScrollItem(string name)
        {
            SpawnedItem item = spawnedItemNPCScroll[name];

            UpdateTextItem(item.UiObject, item.NpcObject.GetNPCInfo().Name, item.CountNegative, item.CountNeutral, item.CountPositive, item.CountStronglyNegative, item.CountStronglyPositive);
        }

        private void UpdateTextItem(GameObject uiObject, string name, int negative = 0, int neutral = 0, int positive = 0, int stronglyNegative = 0, int stronglyPositive = 0)
        {
            //sick moment
            TMP_Text[] text = uiObject.GetComponentsInChildren<TMP_Text>();
            // Name
            text[0].text = name;
            // Text Count Negative
            text[1].text = "" + stronglyNegative;
            // Text Count Negative
            text[2].text = "" + negative;
            // Text Count Middle
            text[3].text = "" + neutral;
            // Text Count Positive
            text[4].text = "" + positive;
            // Text Count Strong Positive
            text[5].text = "" + stronglyPositive;
        }

    }
    public class SpawnedItem
    {
        public GameObject UiObject { get; private set; } // the UI element in the scroll
        public NPCController NpcObject { get; private set; } // the NPC this UI item represents

        public SpawnedItem(GameObject uiObject, NPCController npcObject)
        {
            UiObject = uiObject;
            NpcObject = npcObject;
        }

        public int CountNegative { get; set; } = 0;
        public int CountPositive { get; set; } = 0;
        public int CountNeutral { get; set; } = 0;

        public int CountStronglyNegative { get; set; } = 0;
        public int CountStronglyPositive { get; set; } = 0;
    }
}
