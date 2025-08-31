using System;
using System.Collections.Generic;
using NPC;
using TMPro;
using Unity.Netcode;
using UnityEngine;



namespace GameUI
{
    public class StatisticsUI : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textCountNegative, _textCountMiddle, _textCountPozitive;
        [SerializeField]
        private GameObject _scrollNPCItems;
        [SerializeField]
        private GameObject _prefabNPCItem;
        // Items in Scroll NPC TODO MAKE DICTIONARY
        [Serializable]
        public class SpawnedItem
        {
            public GameObject uiObject;  // the UI element in the scroll
            public NPCController npcObject; // the NPC this UI item represents
        }
        private readonly List<SpawnedItem> spawnedItemNPCScroll = new();

        private int _countNegative = 0, _countMiddle = 0, _countPozitive = 0;

        private void Start()
        {
            _textCountNegative.text = _countNegative.ToString();
            _textCountMiddle.text = _countMiddle.ToString();
            _textCountPozitive.text = _countPozitive.ToString();
            //TODO values to stat
            GenerateListNPCScroll();
        }

        public void OnNewEmothion(EmotionType emotion)
        {
            switch (emotion)
            {
                case EmotionType.Happy:
                    _countPozitive++;
                    break;
                case EmotionType.Sad:
                    _countMiddle++;
                    break;
                case EmotionType.Angry:
                    _countNegative++;
                    break;
            }
            UpdateTextsRPC(_countMiddle, _countNegative, _countPozitive);
        }

        [Rpc(SendTo.Everyone)]
        public void UpdateTextsRPC(int countMiddle, int countNegative, int countPozitive)
        {
            _textCountMiddle.text = countMiddle.ToString();
            _textCountNegative.text = countNegative.ToString();
            _textCountPozitive.text = countPozitive.ToString();
        }
        private void GenerateListNPCScroll()
        {
            NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
            foreach (var npc in npcs)
            {
                GameObject newItem = Instantiate(_prefabNPCItem, _scrollNPCItems.transform);
                spawnedItemNPCScroll.Add(new SpawnedItem { uiObject = newItem, npcObject = npc });

                // Example: if prefab has a TMP_Text component
                TMP_Text text = newItem.GetComponentsInChildren<TMP_Text>()[0];
                if (text != null)
                {
                    text.text = npc.GetNPCInfo().Name;
                }
            }
            UpdateNPCScrollItems();
        }
        //TODO ADD VALUES HERE
        public void UpdateNPCScrollItems()
        {
            foreach (var itemNpc in spawnedItemNPCScroll)
            {

                TMP_Text[] text = itemNpc.uiObject.GetComponentsInChildren<TMP_Text>();
                if (text == null)
                {
                    Debug.Log("Not found object in npc scroll!");
                    break;
                }
                // Name
                text[0].text = itemNpc.npcObject.GetNPCInfo().Name;
                // Text Count Negative
                text[1].text = "" + 0;
                // Text Count Middle
                text[2].text = "" + 0;
                // Text Count Positive
                text[3].text = "" + 0;
            }
        }
    }
}
