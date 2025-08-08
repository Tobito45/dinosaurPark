using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


namespace Museum
{
    public class MuseumController : MonoBehaviour
    {
        [SerializeField]
        private List<Stand> _stands = new();

        [SerializeField]
        private List<Transform> _standsWaypoints = new();

        public Vector3? GetRandomFullStand(Vector3? last)
        {
            List<int> fulls = new();

            for (int i = 0; i < _stands.Count; i++)
                if (_stands[i].Placed && (last == null || Vector3.Distance(
                        new Vector3(_standsWaypoints[i].position.x, last.Value.y, _standsWaypoints[i].position.z), 
                            last.Value) > 0.1f))
                    fulls.Add(i);

            if (fulls.Count == 0)
                return null;

            int index = Random.Range(0, fulls.Count);

            return _standsWaypoints[fulls[index]].transform.position;
        }
    }
}
