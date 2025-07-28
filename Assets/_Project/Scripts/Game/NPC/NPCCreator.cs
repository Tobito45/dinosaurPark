using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;


namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCCreator : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] _skins;

        [SerializeField]
        private TextMeshPro _nameText;

        private List<Transform> _points = new();
        public IEnumerable<Transform> Points => _points;

        public Transform MuseumPoint { get; private set; }

        private NPCPointsController _pointsController;
        private NavMeshAgent _agent;
        public Action<NPCInfo> OnInit;

        private void Awake()
        {
            _pointsController = FindFirstObjectByType<NPCPointsController>();
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Init(NPCInfo nPCInfo)
        {
            foreach (int i in nPCInfo.Way)
                _points.Add(_pointsController.GetPoint(i));

            InitClientsRPC(nPCInfo.Skin, nPCInfo.Name, nPCInfo.Priorty);

            MuseumPoint = _pointsController.MuseumPoint;

            OnInit?.Invoke(nPCInfo);
        }

        [Rpc(SendTo.Everyone)]
        public void InitClientsRPC(int skin, string name, int priority)
        {
            _skins[skin].SetActive(true);
            _nameText.text = name;
            _agent.avoidancePriority = priority;
        }
    }
}
