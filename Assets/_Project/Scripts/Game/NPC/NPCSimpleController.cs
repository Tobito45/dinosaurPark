using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCSimpleController : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private NavMeshAgent _navMeshAgent;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                _navMeshAgent.destination = _target.position;

        }
    }
}
