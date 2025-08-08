using System.Collections;
using UnityEngine;


namespace Door
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField]
        private float _rotationSpeed = 2f;
        
        private bool _isRotating = false;
        internal bool PlayerInFront { get; set; }
        internal bool PlayerInBack { get; set; }
        private Quaternion _targetRotation;
        private Vector3 _startRotation;

        private void Start() => _startRotation = transform.rotation.eulerAngles;


        public void RotateToAngle(Vector3 eulers)
        {
            //if (_isRotating) return;

            if (PlayerInBack || PlayerInFront)
                return;

            if(eulers == Vector3.zero)
                eulers = _startRotation;

            _targetRotation = Quaternion.Euler(eulers);
            StartCoroutine(RotateToTarget());
        }

        private IEnumerator RotateToTarget()
        {
            _isRotating = true;

            while (Quaternion.Angle(transform.rotation, _targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
                yield return null;
            }

            transform.rotation = _targetRotation;
            _isRotating = false;
        }
    }
}

