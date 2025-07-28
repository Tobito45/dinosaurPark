using System.Collections.Generic;
using UnityEngine;


namespace NPC
{
    [CreateAssetMenu(fileName = "NPCInfo", menuName = "Scriptable Objects/NPCInfo")]
    public class NPCInfo : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public int Skin { get; private set; }

        [field: SerializeField]
        public float Money { get; private set; }

        [field: SerializeField]
        public int Priorty { get; private set; }

        [SerializeField]
        private int[] _way;
        public IEnumerable<int> Way => _way;
    }
}
