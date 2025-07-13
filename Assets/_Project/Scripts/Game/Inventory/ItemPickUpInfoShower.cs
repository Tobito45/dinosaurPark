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


    public void ActivePanelPick(InventoryItemLibrary item)
    {
        _panel.SetActive(true);
        _text.text = item.ItemName + _baseTextPick;
    }
    public void ActivePanelPlace(InventoryItemLibrary item)
    {
        _panel.SetActive(true);
        _text.text = item.ItemName + _baseTextPlace;
    }

    public void ActivePanelInteract(string item)
    {
        _panel.SetActive(true);
        _text.text = item + _baseTextInteract;
    }

    public void DeactivePanel() => _panel.SetActive(false);
}
