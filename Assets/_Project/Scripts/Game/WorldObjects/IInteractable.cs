using UnityEngine;

public interface IInteractable
{ 
    void OnHoverEnter();
    void OnHoverExit();
    void OnInteractDown();
    void OnInteractUp();
    bool CanBeInteracted();
    string GetInteractText();   
}
