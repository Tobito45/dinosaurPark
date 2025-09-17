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


    public void ActivePanelPick(ItemRuntimeInfo item)
    {
        _panel.SetActive(true);
        _text.text = item.Name + _baseTextPick;
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
