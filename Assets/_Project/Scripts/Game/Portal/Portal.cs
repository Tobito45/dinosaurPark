using Character;
using DI;
using UnityEngine;


namespace Portal
{
    public class Portal : MonoBehaviour
    {
        [SerializeField]
        private PortalController _portalController;

        [SerializeField]
        private Transform _otherSidePosition;

        [Inject]
        private PlayerProxy _characterFacade;

        private void OnTriggerEnter(Collider other)
        {
            if (_characterFacade == null)
                this.Inject();

            if(other.gameObject == _characterFacade.GetMainPlayer())
                _characterFacade.TeleportToPoint(_otherSidePosition.position);
        }

    }
}
