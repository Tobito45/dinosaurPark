using UnityEngine;


namespace Placement
{
    public class PlacementInfo : MonoBehaviour
    {
        [SerializeField]
        private Material _freeMaterial, _occupiedMaterial;

        public MeshRenderer MeshRender { get; private set; }
        private bool _isOccuped = false;
        public bool IsOcucupied { 
            get => _isOccuped;
            set
            {
                _isOccuped = value;
                if (MeshRender == null)
                    return;

                if (_isOccuped)
                    MeshRender.material = _occupiedMaterial;
                else
                    MeshRender.material = _freeMaterial;
            }
        }
        private void Start() => MeshRender = GetComponent<MeshRenderer>();
    }
}
