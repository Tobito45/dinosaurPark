using System;
using UnityEngine;


namespace Dinosaurus
{
    [RequireComponent(typeof(SphereCollider))]
    internal class ZoneDetecter : MonoBehaviour
    {
        public Action<GameObject> OnEntityEnter;

        public Action<GameObject> OnEntityExit;

        private SphereCollider _collider;

        private void Start()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Entity")
                OnEntityEnter?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Entity")
                OnEntityExit?.Invoke(other.gameObject);
        }

        public float Radius => _collider.radius;
    }
}

