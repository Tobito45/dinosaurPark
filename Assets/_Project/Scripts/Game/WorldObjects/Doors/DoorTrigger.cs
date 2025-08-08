using Unity.VisualScripting;
using UnityEngine;


namespace Door
{
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField]
        private DoorController _doorController;
        [SerializeField]
        private bool _isFront, _isBack;
        [SerializeField]
        private Vector3 _targetRotation;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Entity")
            {
                _doorController.RotateToAngle(_targetRotation);
                SetControllerSide(true);
            }
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (other.tag == "Entity")
        //    {
        //        if (Quaternion.Angle(_doorController.transform.rotation, Quaternion.Euler(_targetRotation)) > 0.1f)
        //        {
        //            _doorController.RotateToAngle(_targetRotation);
        //            SetControllerSide(true);
        //        }
        //    }
        //}

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Entity")
            {
                SetControllerSide(false);
                _doorController.RotateToAngle(Vector3.zero);
            }
        }

        private void SetControllerSide(bool active)
        {
            if(_isBack)
                _doorController.PlayerInBack = active;
            else if (_isFront)
                _doorController.PlayerInFront = active;
        }

        private bool CheckSide()
        {
            if (_isBack)
                return _doorController.PlayerInBack;
            else if (_isFront)
                return _doorController.PlayerInFront;
            else
                return false;
        }
    }
}
