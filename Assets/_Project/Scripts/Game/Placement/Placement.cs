using UnityEngine;


namespace Placement
{
    [CreateAssetMenu(fileName = "Placement", menuName = "Scriptable Objects/Placement")]
    public class Placement : ScriptableObject
    {
        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        [field: SerializeField]
        public int Height { get; private set; }

        [field: SerializeField]
        public int Width { get; private set; }
    }
}
