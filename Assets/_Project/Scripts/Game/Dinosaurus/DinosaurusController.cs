using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Dinosaurus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DinosaurusController : MonoBehaviour
    {
        [Header("Parameters")]
        private float _minReachedPointDistance = 5f;
        [SerializeField]
        private float _maxReachedPointDistance = 20f;

        public float ActualReachedPointDistance {get; private set;}
        public float TimerMarkIdle { get; private set;}

        private NavMeshAgent _navMeshAgent;
        private bool _isAnimaton;

        public Action<DinosaurusController> OnEnterThePoint;

        public float DistanceDistionation => _navMeshAgent.remainingDistance;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            ActualReachedPointDistance = UnityEngine.Random.Range(_minReachedPointDistance, _maxReachedPointDistance);
            _navMeshAgent.speed = UnityEngine.Random.Range(3.5f, 4.0f);

            GenerateMark();
        }

        public Vector3? Distination()
        {
            if (_navMeshAgent.destination == null)
                return null;

            return _navMeshAgent.destination;
        }

        private void GenerateMark() => TimerMarkIdle += UnityEngine.Random.Range(50, 100);

        public bool IsTimerAfterMark(float time) => TimerMarkIdle < time;

        public void StartWait()
        {
            if (TimerMarkIdle == 0 || _isAnimaton)
                return;

            StartCoroutine(Wait());
        }

        private IEnumerator Wait() 
        {
            _isAnimaton = true;
            _navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(3);
            TimerMarkIdle += 3;
            _navMeshAgent.isStopped = false;
            _isAnimaton = false;
            GenerateMark();
        }
 
        private void Update()
        {
            if (_navMeshAgent.destination != null &&
                    !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= ActualReachedPointDistance)
                OnEnterThePoint?.Invoke(this);  
        }

        public void SetNextPoint(Transform point, Vector3 offset) => _navMeshAgent.SetDestination(point.position + offset);
        public void SetNextPoint(Vector3 point, Vector3 offset) => _navMeshAgent.SetDestination(point + offset);

        private void GoToNextPoint()
        {
            //if (_points.Count == 0)
            //    return;

            //_navMeshAgent.SetDestination(_points[_currentPointIndex].position);

            //_currentPointIndex = (_currentPointIndex + 1) % _points.Count;
        }
    }
}
