using Library;
using Unity.Netcode;
using UnityEngine;


namespace Placement
{
    [CreateAssetMenu(fileName = "Placement", menuName = "Scriptable Objects/Placement")]
    public class Placement : ScriptableObject, ILibraryKey
    {
        [field: SerializeField]
        public string PlacmentName { get; private set; }

        [field: SerializeField]
        public NetworkObject Prefab { get; private set; }

        [field: SerializeField]
        public int Height { get; private set; }

        [field: SerializeField]
        public int Width { get; private set; }

        public string Key => PlacmentName;
    }
}
