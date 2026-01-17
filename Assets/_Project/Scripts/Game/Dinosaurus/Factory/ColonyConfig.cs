using Unity.Netcode;
using UnityEngine;


namespace Dinosaurus.Factory
{
    [CreateAssetMenu(fileName = "ColonyConfig", menuName = "Scriptable Objects/ColonyConfig")]
    public class ColonyConfig : ScriptableObject
    {
        [field: SerializeField]
        public int Count { get; private set; }

        [field: SerializeField]
        public float DistanceToGather { get; private set; }

        [field: SerializeField]
        public NetworkObject Prefab { get; private set; }

    }
}
