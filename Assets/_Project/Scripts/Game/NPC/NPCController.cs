using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(NPCCreator))]
    public class NPCController : NetworkBehaviour
    {
        [SerializeField] 
        private float _stoppingDistance = 0.1f;

        private NavMeshAgent _agent;
        private NPCCreator _creator;
        private NPCInfo _info;

        private List<Transform> _path = new(); //goes from end
        private bool _waiting = false;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _creator = GetComponent<NPCCreator>();
            _creator.OnInit += Init;
        }

        private void Update()
        {
            if (!IsServer)
                return;

            if (_agent.destination != null && !_agent.pathPending && _agent.remainingDistance <= _stoppingDistance)
                if(!_waiting)
                    StartCoroutine(Waiter());
        }

        private void Init(NPCInfo info)
        {
            _info = info;
            CreatePath();
            MoveToNextWayPoint();
        }


        private void CreatePath()
        {
            _path.Clear();

            int count = Random.Range(1, 4 + 1);
            var pointsList = _creator.Points.ToList();
            _path.Add(_creator.MuseumPoint);

            for (int i = 0; i < count; i++)
                _path.Add(pointsList[Random.Range(0, pointsList.Count)]);
        }

        private void MoveToNextWayPoint()
        {
            _path.Remove(_path.Last());

            _agent.destination = _path.Last().position;
        }


        private void OnComeToMuseum()
        {
            Debug.Log(_info.Name + " in museum");
            CreatePath();
            MoveToNextWayPoint();
        }

        public IEnumerator Waiter()
        {
            _waiting = true;
            yield return new WaitForSeconds(Random.Range(2,5));
            _waiting = false;
            
            if (_path.Count() == 1)
                OnComeToMuseum();
            else
                MoveToNextWayPoint();
        }

    }
}
