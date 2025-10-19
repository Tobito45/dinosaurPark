using NUnit.Framework;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


namespace Dinosaurus
{
    public class ColonyController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int _count = 3;
        [SerializeField]
        private float _distanceToGather = 5;

         
        [Header("References")]
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Transform _spawnPoint;
        
        [SerializeField]
        private List<Transform> _points = new();
        private Transform _currentPoint;

        private float _mainTimer;

        private Dictionary<DinosaurusController, DinoStatuses> _dinosauruses = new();

        private void Start()
        {
            for (int i = 0; i < _count; i++)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 4f;
                Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);

                GameObject obj = Instantiate(_prefab, _spawnPoint.position + offset, Quaternion.identity, transform);
                DinosaurusController dinosaurusController = obj.GetComponent<DinosaurusController>();
                _dinosauruses.Add(dinosaurusController, DinoStatuses.Stand);
                dinosaurusController.OnEnterThePoint += OnDinoComeToPoint;
                dinosaurusController.OnStartHuntering += OnDinoStartHuntering;
                dinosaurusController.OnEndHuntering += OnDinoEndHuntering;
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
            UpdateTimer();
            UpdateDistance();
        }

        private void UpdateTimer()
        {
            _mainTimer += Time.deltaTime;
            foreach (DinosaurusController dino in _dinosauruses.Keys)
            {
                if (dino.IsTimerAfterMark(_mainTimer))
                    dino.StartWait();
            }
        }

        private void UpdateDistance()
        {
            if (_currentPoint == null)
                return;

            Vector3 center = Vector3.zero;
            foreach (DinosaurusController obj in _dinosauruses.Keys)
            {
                if (_dinosauruses[obj] == DinoStatuses.Huntering)
                    continue;

                center += obj.transform.position;
            }
            
            center /= _dinosauruses.Select(n => n.Value != DinoStatuses.Huntering).Count();

            foreach (DinosaurusController obj in _dinosauruses.Keys)
            {
                if (_dinosauruses[obj] == DinoStatuses.Huntering)
                    continue;

                float distance = Vector3.Distance(obj.transform.position, center);
                if (distance > _distanceToGather)
                {
                    SetAllDinoPointAndStatus(center, DinoStatuses.Gathering);
                    break;
                }
            }
        }


        private Coroutine _waitCorortine = null;
        private void OnDinoComeToPoint(DinosaurusController controller)
        {
            if (_dinosauruses[controller] == DinoStatuses.Huntering || _waitCorortine != null)
                return;

            DinoStatuses last = _dinosauruses[controller];
             _dinosauruses[controller] = DinoStatuses.OnPoint;

            if (GetCountDinoStatuses(DinoStatuses.OnPoint).all)
                _waitCorortine = StartCoroutine(WaiterOnPoint(last));
        }

        private IEnumerator WaiterOnPoint(DinoStatuses last)
        {
            yield return new WaitForSeconds(5);
            OnAllDinoOnPoint(last == DinoStatuses.Gathering);
            _waitCorortine = null;
        }

        private void OnDinoStartHuntering(DinosaurusController controller) => _dinosauruses[controller] = DinoStatuses.Huntering;
        private void OnDinoEndHuntering(DinosaurusController controller)
        {
            _dinosauruses[controller] = DinoStatuses.Walked;
            SetDinoPoint(_currentPoint.position, controller);
        }

        private void OnAllDinoOnPoint(bool withoutNewPoint = false)
        {
            if (!withoutNewPoint)
            {
                Transform point = _points[Random.Range(0, _points.Count)];
                while (point == _currentPoint)
                    point = _points[Random.Range(0, _points.Count)];
                
                _currentPoint = point;
            }

            SetAllDinoPointAndStatus(_currentPoint.position, DinoStatuses.Walked);
        }
        
        private void SetAllDinoPointAndStatus(Vector3 position, DinoStatuses status)
        {
            foreach (DinosaurusController dinosaurusController in _dinosauruses.Keys.ToList())
            {
                if (_dinosauruses[dinosaurusController] == DinoStatuses.Huntering)
                    continue;

                _dinosauruses[dinosaurusController] = status;
                SetDinoPoint(position, dinosaurusController);
            }
        }

        private void SetDinoPoint(Vector3 position, DinosaurusController dinosaurusController)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 4f;
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);
            dinosaurusController.SetNextPoint(position, offset);
        }

        private (bool all, int count) GetCountDinoStatuses(DinoStatuses goalStatus)
        {
            int countFinded = 0;
            foreach(DinoStatuses value in _dinosauruses.Values)
                if(value == goalStatus || value == DinoStatuses.Huntering)
                    countFinded++;
        
            return (countFinded == _dinosauruses.Count,  countFinded);
        }


        private enum DinoStatuses
        {
            Walked, OnPoint, Stand, Gathering, Huntering
        }
    }
}
