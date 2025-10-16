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

        [Header("Animator")]
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private string _wait, _attack;


        public float ActualReachedPointDistance { get; private set; }
        public float TimerMarkIdle { get; private set; }

        private NavMeshAgent _navMeshAgent;
        private bool _isAnimaton;

        public Action<DinosaurusController> OnEnterThePoint;
        public Action<DinosaurusController> OnStartHuntering;
        public Action<DinosaurusController> OnEndHuntering;
        private GameObject _target;

        private float _startSpeed;
        private float _startRunSpeed;
        private bool _isAttacked = false, _canAttack = false;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            ActualReachedPointDistance = UnityEngine.Random.Range(_minReachedPointDistance, _maxReachedPointDistance);
            _navMeshAgent.speed = UnityEngine.Random.Range(3.5f, 4.0f);
            _startSpeed = _navMeshAgent.speed;
            _startRunSpeed = _animator.speed;

            _warningZone.OnEntityEnter += OnWarningZoneEnter;
            _warningZone.OnEntityExit += OnWarningZoneExit;

            _redZone.OnEntityEnter += OnRedZoneEnter;

            _attackZone.OnEntityEnter += OnAttackZoneEnter;
            _attackZone.OnEntityExit += OnAttackZoneExit;


            GenerateMark();
        }


        private void GenerateMark() => TimerMarkIdle += UnityEngine.Random.Range(50, 100);

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
                    break;

                yield return null;
            }

            _animator.SetBool("Wait", false);
            TimerMarkIdle += 3;
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
            {
                Debug.Log("Kek");
                _animator.SetBool("Run", true);
            }
            //    Debug.Log(_navMeshAgent.velocity);
            //if (_navMeshAgent.velocity.sqrMagnitude > 0.1)
            //    _animator.SetBool("Run", true);
            //else
            //    _animator.SetBool("Run", false);

            if (_timerWaring >= 0)
                _timerWaring += Time.deltaTime;

            if (_target != null)
            {
                _navMeshAgent.destination = _target.transform.position;
                if (_canAttack && !_isAttacked) //debug
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


        private float _timerWaring = -1;
        private void OnWarningZoneEnter(GameObject player)
        {
            if (_target != null)
                return;

            _timerWaring = 0;
            OnStartHuntering?.Invoke(this);
            _target = player;

            _navMeshAgent.destination = _target.transform.position;
            _navMeshAgent.speed = 1;
            _animator.speed = 0.5f;
        }

        private void OnWarningZoneExit(GameObject player)
        {
            if (_target == null)
                return;

            _timerWaring = -1;
            OnEndHuntering?.Invoke(this);
            _navMeshAgent.speed = _startSpeed;
            _animator.speed = _startRunSpeed;
            _target = null;
        }

        private void OnRedZoneEnter(GameObject player)
        {
            if (player == _target)
            {
                _timerWaring = -1;
                _navMeshAgent.destination = _target.transform.position;
                _navMeshAgent.speed = _startSpeed;
                _animator.speed = _startRunSpeed;
            }
        }

        private void OnAttackZoneEnter(GameObject player)
        {
            if (player == _target) _canAttack = true;
        }
        private void OnAttackZoneExit(GameObject player)
        {
            if (player == _target) _canAttack = false;
        }
    }
}
