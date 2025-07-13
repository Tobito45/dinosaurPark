using Character;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggedItem : NetworkBehaviour, IInteractable
{
    [SerializeField] 
    private float grabRange = 3f;
    [SerializeField] 
    private float holdDistance = 2f;
    [SerializeField] 
    private float dragSpeed = 10f;

    private Transform holdPoint;

    private Rigidbody heldObject;

    private bool _isDragged = false;

    private void Awake()
    {
        heldObject = GetComponent<Rigidbody>();
        heldObject.isKinematic = false; //TEST
    }

    void FixedUpdate()
    {
        if (_isDragged && IsServer)
        {
            Vector3 targetPosition = holdPoint.transform.position + holdPoint.transform.forward * holdDistance;

            Vector3 direction = targetPosition - heldObject.position;
            heldObject.linearVelocity = direction * dragSpeed;
        }
    }

    public bool CanBeInteracted()
    {
        return !_isDragged;
    }

    public string GetInteractText() => "Drag";

    public void OnHoverEnter() { }

    public void OnHoverExit() { }

    public void OnInteractDown()
    {
        SetEveryoneDragRpc(GameClientsNerworkInfo.Singleton.MainPlayer.gameObject);
    }


    public void OnInteractUp()
    {
        SetEveryoneDropRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SetEveryoneDragRpc(NetworkObjectReference itemRef)
    {

        if (itemRef.TryGet(out NetworkObject netObj))
        {
            holdPoint = netObj.GetComponent<CharacterGO>().MainCamere.transform;

            heldObject.useGravity = false;
            heldObject.linearDamping = 10f;

            _isDragged = true;
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetEveryoneDropRpc()
    {
        holdPoint = null;

        heldObject.useGravity = true;
        heldObject.linearDamping = 0f;

        _isDragged = false;
    }
}
