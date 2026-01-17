using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class StandTextChanger : NetworkBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField]
    private Stand _stand;

    [Header("Settings")]
    [SerializeField]
    private TMP_InputField _text;

    private bool _isInEditingMode;

    public bool CanBeInteracted() => _stand.Placed != null;

    public string GetInteractText() => "Rename";

    public void OnHoverEnter() 
    {
        
    }

    public void OnHoverExit() 
    {
        _text.DeactivateInputField();
        UpdateTextRpc(_text.text);
    }

    public void OnInteractDown() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        _text.Select();
        _isInEditingMode = true;
        UserPermissions.Singleton.SetUIStunPermissons(true);
    }

    public void OnInteractUp() { }

    private void Update()
    {
        if (_isInEditingMode && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)))
        {
            _text.DeactivateInputField();
            UpdateTextRpc(_text.text);
            UserPermissions.Singleton.SetUIStunPermissons(false);
            EventSystem.current.SetSelectedGameObject(null);

        }
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateTextRpc(string text) => _text.text = text;

}
