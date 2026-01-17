using Dinosaurus.Command;
using Dinosaurus.States;
using Dinosaurus.Strategy;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;


namespace Dinosaurus
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DinosaurusController : NetworkBehaviour
    {
        [Header("Zones")]
        [SerializeField]
        private ZoneDetecter _warningZone;
        [SerializeField]
        private ZoneDetecter _redZone;
        [SerializeField]
        private ZoneDetecter _attackZone;
        [SerializeField]
        private ZoneDetecter _attackHitZone;

        [Header("Parameters")]
        [SerializeField]
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
        protected AnimatorReciver _animatorReciver;
        [SerializeField]
        private string _attack;
        
        public float ActualReachedPointDistance { get; private set; }
        private float timerToIdle;
        public event Action<DinosaurusController> OnIdleTimeOver;

        protected NavMeshAgent _navMeshAgent;
        protected bool _isAnimaton;

        public Action<DinosaurusController> OnEnterThePoint;
        public Action<DinosaurusController> OnStartHuntering;
        public Action<DinosaurusController> OnEndHuntering;

        public event Action<DinosaurusController> OnIdleStart;
        public event Action<DinosaurusController> OnIdleEnd;
        
        public GameObject Target { get; set; }

        public bool IsAttacked { get; set; }
        public bool CanAttack { get; set; }

        private IDinoState _currentState;
        public IDinoState CurrentState => _currentState;

        private IHuntingDinoStrategy _huntingStrategy;

        public void ChangeState(IDinoState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);
        }

        public void Init()
        {
            if (!IsServer)
                return;

            _navMeshAgent = GetComponent<NavMeshAgent>();

            ActualReachedPointDistance = UnityEngine.Random.Range(_minReachedPointDistance, _maxReachedPointDistance);
            _navMeshAgent.speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);

            _warningZone.OnEntityEnter += OnWarningZoneEnter;
            _warningZone.OnEntityExit += OnWarningZoneExit;

            _redZone.OnEntityEnter += OnRedZoneEnter;
            _redZone.OnEntityExit += OnRedZoneExit;

            _attackZone.OnEntityEnter += OnAttackZoneEnter;
            _attackZone.OnEntityExit += OnAttackZoneExit;

            _attackHitZone.OnEntityEnter += OnAttackHitZoneEnter;

            _attackHitZone.gameObject.SetActive(false);

            _animatorReciver.OnAttackStart += () => _attackHitZone.gameObject.SetActive(true);
            _animatorReciver.OnAttackEnd += () => _attackHitZone.gameObject.SetActive(false);

            _animatorReciver.OnIdleStart += () => OnIdleStart?.Invoke(this);
            _animatorReciver.OnIdleEnd += () => OnIdleEnd?.Invoke(this);

            ResetIdleTimer();
        }

        public void SetStrategic(IHuntingDinoStrategy strategy) => _huntingStrategy = strategy;

        public void ResetIdleTimer() => timerToIdle = UnityEngine.Random.Range(_minTimeToWait, _maxTimeToWait);

        private void Update()
        {
            if (!IsServer)
                return;

            Debug.Log(gameObject.name + " State Update: " + _currentState);

            UpdateTimer();

            _currentState?.Update(this);

            if (Target != null)
            {
                _navMeshAgent.destination = Target.transform.position;
                if (CanAttack && !IsAttacked)
                    StartCoroutine(Attack());
            }
        }
        public void UpdateTimer()
        {
            if (timerToIdle < 0)
                return;

            if (_currentState == null || _currentState.IsHunting())
                return;

            timerToIdle -= Time.deltaTime;
            if (timerToIdle < 0)
                OnIdleTimeOver?.Invoke(this);
        }
         
        public void ExecuteCommand(IDinoCommand command) => command.Execute(this);

        public void SetNextPoint(Transform point, Vector3 offset) => _navMeshAgent.SetDestination(point.position + offset);
        public void SetNextPoint(Vector3 point, Vector3 offset) => _navMeshAgent.SetDestination(point + offset);
        public void SetAnimationRun(bool isRun) => _animator.SetBool("Run", isRun);
        public void SetAnimationWait(bool isWait) => _animator.SetBool("Wait", isWait);
        public bool IsAnimationWait() => _animator.GetBool("Wait");
        public void SetStopNavMesh(bool stop) => _navMeshAgent.isStopped = stop;
        public bool IsDinoReachedPoint() => _navMeshAgent.destination != null &&
                    !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= ActualReachedPointDistance && Target == null;

        public void SetAnimatorSpeed(float speed) => _animator.speed = speed;
        public void SetNavMeshSpeed(float speed) => _navMeshAgent.speed = speed;

        public float GetAnimatorSpeed() => _animator.speed;
        public float GetNavMeshSpeed() => _navMeshAgent.speed;

        private IEnumerator Attack()
        {
            IsAttacked = true;
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
            IsAttacked = false;
        }

        private void OnWarningZoneEnter(GameObject player) => _huntingStrategy.OnWarningZoneEnter(this, player);

        private void OnWarningZoneExit(GameObject player) => _huntingStrategy.OnWarningZoneExit(this,player);

        private void OnRedZoneEnter(GameObject player) => _huntingStrategy.OnRedZoneEnter(this, player);
        private void OnRedZoneExit(GameObject player) => _huntingStrategy.OnRedZoneExit(this, player);

        private void OnAttackZoneEnter(GameObject player) => _huntingStrategy.OnAttackZoneEnter(this, player);
        private void OnAttackZoneExit(GameObject player) => _huntingStrategy.OnAttackZoneExit(this, player);
        private void OnAttackHitZoneEnter(GameObject player) => _huntingStrategy.OnAttackHitZoneEnter(this, player);
        private void OnAttackHitZoneExit(GameObject player) => _huntingStrategy.OnAttackHitZoneExit(this, player);
    }
}
