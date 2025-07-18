using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StandTextChanger : NetworkBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField]
    private Stand _stand;

    [Header("Settings")]
    [SerializeField]
    private TMP_InputField _text;

    private bool _isInEditingMode;

    public bool CanBeInteracted() => !_stand.CheckIfCanPlaceItem(-1);

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
        _text.Select();
        _isInEditingMode = true;
        GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(true);
    }

    public void OnInteractUp() { }

    private void Update()
    {
        if (_isInEditingMode && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)))
        {
            _text.DeactivateInputField();
            UpdateTextRpc(_text.text);
            GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(false);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateTextRpc(string text) => _text.text = text;

}
