using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        private const int BASIC_INDEX_NOT_SELECTED = -1;

        [SerializeField]
        private List<InventoryUIItem> _list = new();

        [SerializeField]
        private Color32 _baseColor;

        private int _currectSelected = BASIC_INDEX_NOT_SELECTED;

        private void Start() => Reset();

        public void Reset()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                DeSelectItem(i);
                ResetItem(i);
                _list[i].BaseColor = _baseColor;
            }

            _currectSelected = BASIC_INDEX_NOT_SELECTED;
        }

        public void SelectItem(int index)
        {
            if (_currectSelected != BASIC_INDEX_NOT_SELECTED)
                DeSelectItem(_currectSelected);

            _list[index].SelectBackground.SetActive(true);
            _currectSelected = index;
        }

        public void DeSelectItem(int index) => _list[index].SelectBackground.SetActive(false);

        public void PutItem(int index, InventoryItemLibrary itemLibrary) =>
            _list[index].Image.color = itemLibrary.Color;

        public void ResetItem(int index) =>
            _list[index].Image.color = _list[index].BaseColor;
    }

    [System.Serializable]
    internal class InventoryUIItem
    {
        [field:SerializeField]
        public GameObject SelectBackground { get; private set; }

        [field: SerializeField]
        public Image Image { get ; private set; }

        public Color32 BaseColor { get; set; }


    }
}
