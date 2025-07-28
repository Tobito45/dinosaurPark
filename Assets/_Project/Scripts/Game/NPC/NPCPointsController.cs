using UnityEngine;


namespace NPC
{
    public class NPCPointsController : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _points;
        [field:SerializeField]
        public Transform MuseumPoint { get; private set; }
        public Transform GetPoint(int index) => _points[index];
    }
}
