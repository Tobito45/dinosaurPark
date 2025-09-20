using Museum;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(NPCCreator))]
    [RequireComponent(typeof(NPCEmotionsController))]
    public class NPCController : NetworkBehaviour
    {
        [SerializeField] 
        private float _stoppingDistance = 0.1f;

        private NavMeshAgent _agent;
        private NPCCreator _creator;
        private NPCInfo _info;
        private NPCEmotionsController _emotions;

        private List<Transform> _path = new(); //goes from end
        private bool _waiting = false, _isInMuseum = false;
        private int _countLook = -1;

        private MuseumController _museumController;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _creator = GetComponent<NPCCreator>();
            _emotions = GetComponent<NPCEmotionsController>();
            _museumController = FindFirstObjectByType<MuseumController>();
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

        //TO OTHER CLASS?
        public void AddOnEmothion(Action<EmotionType, string> onEmothing, Action OnStartWatch)
        {
            _emotions.OnMakeEmotion += onEmothing;
            _emotions.OnStartWatch += OnStartWatch;
        }

        private void CreatePath()
        {
            _path.Clear();

            int count = UnityEngine.Random.Range(1, 4 + 1);
            var pointsList = _creator.Points.ToList();
            _path.Add(_creator.MuseumPoint);

            for (int i = 0; i < count; i++)
            {
                int index = UnityEngine.Random.Range(0, pointsList.Count);
                _path.Add(pointsList[index]);
                pointsList.RemoveAt(index);
            }
        }

        private void MoveToNextWayPoint()
        {
            _path.Remove(_path.Last());

            _agent.destination = _path.Last().position;
        }


        private void OnComeToMuseum()
        {
            if(_countLook == -1)
            {
                _watchedFirstTimeDirtyFlag = true;
                _countLook = UnityEngine.Random.Range(1, 5);
            }

            InMuseum();
        }

        private bool _watchedFirstTimeDirtyFlag = true;
        public void InMuseum()
        {
            Vector3? point = _museumController.GetRandomFullStand(_agent.destination);
            _isInMuseum = true;

            if(_countLook == 0)
            {
                _countLook = -1;
                point = null;
            } else
                _countLook--;
            


            if (point != null)
            {
                if (_watchedFirstTimeDirtyFlag)
                {
                    _watchedFirstTimeDirtyFlag = false;
                    _emotions.StartWatching();
                }

                _agent.destination = point.Value;

            } else
            {
                _isInMuseum = false;

                CreatePath();
                MoveToNextWayPoint();
            }
        }

        public IEnumerator Waiter()
        {
            _waiting = true;
            if(_isInMuseum && _museumController.GetStandByPos(_agent.destination.x, _agent.destination.z).Placed)
                _emotions.ShowEmogi(_info, _museumController.GetStandByPos(_agent.destination.x, _agent.destination.z).Info);

            yield return new WaitForSeconds(UnityEngine.Random.Range(2,5));
            _waiting = false;
            
            if (_path.Count() == 1)
                OnComeToMuseum();
            else
                MoveToNextWayPoint();
        }
        public NPCInfo GetNPCInfo() => _info;
        

    }
}
