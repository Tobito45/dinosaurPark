using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Dinosaurus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class DinosaurusController : MonoBehaviour
    {
        [Header("Zones")]
        [SerializeField]
        private ZoneDetecter _warningZone;
        [SerializeField]
        private ZoneDetecter _redZone;
        [SerializeField]
        private ZoneDetecter _attackZone;

        [Header("Parameters")]
        private float _minReachedPointDistance = 5f;
        [SerializeField]
        private float _maxReachedPointDistance = 20f;

        [SerializeField]
        private float _minSpeed = 3.5f;
        [SerializeField]
        private float _maxSpeed = 4.0f;

        [SerializeField]
        private float _minTimeToWait = 50;
        [SerializeField]
        private float _maxTimeToWait = 100f;


        [Header("Animator")]
        [SerializeField]
        protected Animator _animator;
        [SerializeField]
        private string _wait, _attack;
        
        public float ActualReachedPointDistance { get; private set; }
        public float TimerMarkIdle { get; private set; }

        protected NavMeshAgent _navMeshAgent;
        protected bool _isAnimaton;

        public Action<DinosaurusController> OnEnterThePoint;
        public Action<DinosaurusController> OnStartHuntering;
        public Action<DinosaurusController> OnEndHuntering;
        protected GameObject _target;


        protected bool _isAttacked = false, _canAttack = false;

        protected virtual void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            ActualReachedPointDistance = UnityEngine.Random.Range(_minReachedPointDistance, _maxReachedPointDistance);
            _navMeshAgent.speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);

            _warningZone.OnEntityEnter += OnWarningZoneEnter;
            _warningZone.OnEntityExit += OnWarningZoneExit;

            _redZone.OnEntityEnter += OnRedZoneEnter;
            _redZone.OnEntityEnter += OnRedZoneExit;

            _attackZone.OnEntityEnter += OnAttackZoneEnter;
            _attackZone.OnEntityExit += OnAttackZoneExit;

            GenerateMark();
        }


        private void GenerateMark() => TimerMarkIdle += UnityEngine.Random.Range(_minTimeToWait, _maxTimeToWait);

        public bool IsTimerAfterMark(float time) => TimerMarkIdle < time;

        public void StartWait()
        {
            if (TimerMarkIdle == 0 || _isAnimaton || _target != null)
                return;

            StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            _isAnimaton = true;
            _navMeshAgent.isStopped = true;
            _animator.SetBool("Wait", true);

            while(true)
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);

                if (info.IsName(_wait) && info.normalizedTime > 0.9f)
                {
                    TimerMarkIdle += info.length;
                    break;
                }

                yield return null;
            }

            _animator.SetBool("Wait", false);
            _navMeshAgent.isStopped = false;
            _isAnimaton = false;
            GenerateMark();
        }

        private void Update()
        {
            if (_navMeshAgent.destination != null &&
                    !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= ActualReachedPointDistance && _target == null)
            {
                _animator.SetBool("Run", false);
                OnEnterThePoint?.Invoke(this);
            }else
                _animator.SetBool("Run", true);

            if (_target != null)
            {
                _navMeshAgent.destination = _target.transform.position;
                if (_canAttack && !_isAttacked)
                    StartCoroutine(Attack());
            }
        }
        public void SetNextPoint(Transform point, Vector3 offset) => _navMeshAgent.SetDestination(point.position + offset);
        public void SetNextPoint(Vector3 point, Vector3 offset) => _navMeshAgent.SetDestination(point + offset);

        private IEnumerator Attack()
        {
            _isAttacked = true;
            _navMeshAgent.isStopped = true;
            _animator.SetBool("Attack", true);
            Debug.Log("Attack from " + gameObject.name);

            while (true)
            {
                var info = _animator.GetCurrentAnimatorStateInfo(0);

                if (info.IsName(_attack) && info.normalizedTime > 0.9f)
                    break;

                yield return null;
            }

            _animator.SetBool("Attack", false);
            _navMeshAgent.isStopped = false;
            _isAttacked = false;
        }


        protected abstract void OnWarningZoneEnter(GameObject player);

        protected abstract void OnWarningZoneExit(GameObject player);

        protected abstract void OnRedZoneEnter(GameObject player);
        protected abstract void OnRedZoneExit(GameObject player);

        protected abstract void OnAttackZoneEnter(GameObject player);

        protected abstract void OnAttackZoneExit(GameObject player);
    }
}
