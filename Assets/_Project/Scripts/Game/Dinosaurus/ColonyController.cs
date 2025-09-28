using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Dinosaurus
{
    public class ColonyController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int _count = 3;

         
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
                center += obj.transform.position;
            
            center /= _dinosauruses.Count;

            foreach (DinosaurusController obj in _dinosauruses.Keys)
            {
                float distance = Vector3.Distance(obj.transform.position, center);
                if (distance > 5f)
                {
                    SetAllDinoPointAndStatus(center, DinoStatuses.Gathering);
                    break;
                }
            }
        }    

        
        private void OnDinoComeToPoint(DinosaurusController controller)
        {
             _dinosauruses[controller] = DinoStatuses.OnPoint;

            if (GetCountDinoStatuses(DinoStatuses.OnPoint).all)
                OnAllDinoOnPoint();
        }

        private void OnAllDinoOnPoint()
        {
            var first = _dinosauruses.Keys.First();
            Transform point = _points[Random.Range(0, _points.Count)];
            while (point == _currentPoint)
                point = _points[Random.Range(0, _points.Count)];

            _currentPoint = point;
            SetAllDinoPointAndStatus(_currentPoint.position, DinoStatuses.Walked);
        }

        private void SetAllDinoPointAndStatus(Vector3 position, DinoStatuses status)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 4f;
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);

            foreach (DinosaurusController dinosaurusController in _dinosauruses.Keys.ToList())
            {
                dinosaurusController.SetNextPoint(position, offset);
                _dinosauruses[dinosaurusController] = status;
            }
        }

        private (bool all, int count) GetCountDinoStatuses(DinoStatuses goalStatus)
        {
            int countFinded = 0;
            foreach(DinoStatuses value in _dinosauruses.Values)
                if(value == goalStatus)
                    countFinded++;
        
            return (countFinded == _dinosauruses.Count,  countFinded);
        }
        


        private enum DinoStatuses
        {
            Walked, OnPoint, Stand, Gathering
        }
    }
}
