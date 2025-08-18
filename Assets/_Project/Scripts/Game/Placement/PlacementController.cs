using System.Collections;
using UnityEngine;


namespace Placement
{
    public class PlacementController : MonoBehaviour
    {
        [SerializeField]
        private Transform _border, _startPont, _parent;

        [SerializeField]
        private GameObject _placementPrefab;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                Init();
        }

        public void Init()
        {
            InitPlacements();
        }

        private void InitPlacements()
        {
            Vector3 offset = _startPont.position;
            while (true)
            {
                Instantiate(_placementPrefab,  offset, Quaternion.identity, _parent);

                offset += new Vector3(0, 0, 1f);
                if(offset.z > _border.position.z)
                    offset = new Vector3(offset.x - 1, 0, _startPont.position.z);

                if (offset.x < _border.position.x)
                    break;
            }
        }
    }
}
