using UnityEngine;


namespace Portal
{
    public class Portal : MonoBehaviour
    {
        [SerializeField]
        private PortalController _portalController;

        [SerializeField]
        private Transform _otherSidePosition;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject == GameClientsNerworkInfo.Singleton.MainPlayer.gameObject)
                GameClientsNerworkInfo.Singleton.MainPlayer.CharacterMovement.TeleportToPoint(_otherSidePosition.position);
        }

    }
}
