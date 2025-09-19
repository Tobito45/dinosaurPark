using System.Linq;
using TMPro;
using UnityEngine;

public class ItemPickUpInfoShower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string _baseTextPick, _baseTextPlace, _baseTextInteract;

    [Header("Scene Objects")]
    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private TextMeshProUGUI _text;


    public void ActivePanelPick(ItemRuntimeInfo item, InventoryItemLibrary itemInfo)
    {
        _panel.SetActive(true);
        string itemInfoResult = item.ItemRarityEnum + "\n"
            + "Condition:" + (int)item.Condition + "\n"
            + itemInfo.Era.ToString() + "\n"
            + string.Join("\n", itemInfo.ItemTypes.Select(i => "- " + i));

        _text.text = item.Name + _baseTextPick + "\n" + itemInfoResult;

        // Get the preferred size of the text
        // Vector2 textSize = _text.GetPreferredValues(_text.text, _text.rectTransform.rect.width, Mathf.Infinity);
        // Vector2 padding = new Vector2(2f, 2f);
        // // Apply size to panel with padding
        // _panel.GetComponent<RectTransform>().sizeDelta = new Vector2(textSize.x + padding.x, textSize.y + padding.y);


    }
    public void ActivePanelPlace(ItemRuntimeInfo item)
    {
        _panel.SetActive(true);
        _text.text = item.Name + _baseTextPlace;
    }

    public void ActivePanelInteract(string item)
    {
        _panel.SetActive(true);
        _text.text = item + _baseTextInteract;
    }

    public void DeactivePanel() => _panel.SetActive(false);
}
