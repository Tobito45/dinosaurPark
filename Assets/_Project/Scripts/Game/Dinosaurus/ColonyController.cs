using Dinosaurus.Command;
using Dinosaurus.Factory;
using Dinosaurus.States;
using Dinosaurus.Strategy;
using NUnit.Framework;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


namespace Dinosaurus
{
    public class ColonyController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int _count;
        [SerializeField]
        private float _distanceToGather;

        [Header("References")]
        [SerializeField]
        private NetworkObject _prefab;

        [SerializeField]
        private Transform _spawnPoint;
        
        [SerializeField]
        private List<Transform> _points = new();
        private Transform _currentPoint;

        private List<DinosaurusController> _dinosauruses = new();
        private IHuntingDinoStrategy _huntingDinoStrategy;

        public void SetInfo(ColonyConfig colonyConfig, Transform spawnPoint)
        {
            _count = colonyConfig.Count;
            _distanceToGather = colonyConfig.DistanceToGather;
            _spawnPoint = spawnPoint;
            _prefab = colonyConfig.Prefab;
        }

        public void SetPoints(List<Transform> points) => _points = points;

        public void SetStrategic(IHuntingDinoStrategy huntingDinoStrategy) => 
            _huntingDinoStrategy = huntingDinoStrategy;

        public void Spawn()
        {
            //if (!IsServer)
            //    return;

            for (int i = 0; i < _count; i++)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 4f;
                Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);

                NetworkObject networkObject = Instantiate(_prefab, _spawnPoint.position + offset, Quaternion.identity, _spawnPoint);
                networkObject.Spawn(true);

                DinosaurusController dinosaurusController = networkObject.GetComponent<DinosaurusController>();
                _dinosauruses.Add(dinosaurusController);
                dinosaurusController.Init();
                dinosaurusController.SetStrategic(_huntingDinoStrategy);
                dinosaurusController.ChangeState(new StandDinoState());
                dinosaurusController.OnEnterThePoint += OnDinoComeToPoint;
                dinosaurusController.OnStartHuntering += OnDinoStartHuntering;
                dinosaurusController.OnEndHuntering += OnDinoEndHuntering;

                dinosaurusController.OnIdleTimeOver += OnDinoTimerIdleOver;
            }
            StartCoroutine(LastStart());
        }

        private IEnumerator LastStart()
        {
            yield return null;
            OnAllDinoOnPoint();
        }

        private void Update()
        {
            //if (!IsServer)
            //    return;

            UpdateDistance();
        }

        private void OnDinoTimerIdleOver(DinosaurusController dino) 
            => SendCommnadToDino(dino, new TimerOverIdleDinoCommand());

        private void UpdateDistance()
        {
            if (_currentPoint == null)
                return;

            Vector3 center = Vector3.zero;
            var notHuntering = _dinosauruses.Where(n => !n.CurrentState.IsHunting());

            foreach (DinosaurusController obj in notHuntering)
                center += obj.transform.position;


            center /= notHuntering.Count();

            foreach (DinosaurusController obj in notHuntering)
            {
                float distance = Vector3.Distance(obj.transform.position, center);
                if (distance > _distanceToGather)
                {
                    SendCommnadToAllDino(new GatheringDinoCommand(center));
                    break;
                }
            }
        }


        private Coroutine _waitCorortine = null;
        private void OnDinoComeToPoint(DinosaurusController controller)
        {
            if (controller.CurrentState.IsHunting())
                return;

            bool isGathering = controller.CurrentState.IsGathering();
            controller.ChangeState(new OnPointDinoState());

            if(_waitCorortine == null)
                _waitCorortine = StartCoroutine(WaiterOnPoint(isGathering));
        }

        private IEnumerator WaiterOnPoint(bool isGathering)
        {
            yield return new WaitUntil(() =>
                    _dinosauruses.All(d => d.CurrentState.IsWaitingOnPoint() || d.CurrentState.IsHunting())
                ); 
            yield return new WaitForSeconds(5);
            OnAllDinoOnPoint(isGathering);
            _waitCorortine = null;
        }

        private void OnDinoStartHuntering(DinosaurusController controller) => 
            SendCommnadToDino(controller, new HunteringDinoCommand());
        private void OnDinoEndHuntering(DinosaurusController controller) =>
            SendCommnadToDino(controller, new MoveToPointDinoCommand(_currentPoint.position, true));

        private void OnAllDinoOnPoint(bool withoutNewPoint = false)
        {
            if (!withoutNewPoint)
            {
                Transform point = _points[UnityEngine.Random.Range(0, _points.Count)];
                while (point == _currentPoint)
                    point = _points[UnityEngine.Random.Range(0, _points.Count)];
                
                _currentPoint = point;
            }
            SendCommnadToAllDino(new MoveToPointDinoCommand(_currentPoint.position));
        }

        public void SendCommnadToAllDino(IDinoCommand dinoCommand)
        {
            foreach (DinosaurusController dinosaurusController in _dinosauruses)
                dinosaurusController.ExecuteCommand(dinoCommand);
        }

        public void SendCommnadToDino(DinosaurusController dinosaurusController, IDinoCommand dinoCommand)
            => dinosaurusController.ExecuteCommand(dinoCommand);
    }
}
