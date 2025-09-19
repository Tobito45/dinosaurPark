using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Museum
{
    public class MuseumController : MonoBehaviour
    {
        //private List<Stand> _stands = new();

        private Dictionary<(float x, float z), Stand> _stands = new();
        private List<Transform> _standsWaypoints = new();

        public Vector3? GetRandomFullStand(Vector3? last)
        {
            List<int> fulls = new();

            for (int i = 0; i < _stands.Count; i++)
                if (_stands.Values.ElementAt(i).Placed && (last == null || Vector3.Distance( //slow???
                        new Vector3(_standsWaypoints[i].position.x, last.Value.y, _standsWaypoints[i].position.z), 
                            last.Value) > 0.1f))
                    fulls.Add(i);

            if (fulls.Count == 0)
                return null;

            int index = Random.Range(0, fulls.Count);

            return _standsWaypoints[fulls[index]].position;
        }

        public void AddNewStand(Stand stand)
        {
            _stands.Add((stand.Point.position.x, stand.Point.position.z), stand);
            _standsWaypoints.Add(stand.Point);
        }

        public Stand GetStandByPos(float x, float z) => _stands[(x, z)];
    }
}
