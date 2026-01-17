using Character;
using Dinosaurus.Strategy;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Dinosaurus.Factory
{
    public interface IColonyFactory
    {
        ColonyController Create(Transform spawnPoint, ColonyConfig config, List<Transform> points);
    }

    public class CoelophysisColonyFactory : IColonyFactory
    {
        public ColonyController Create(Transform spawnPoint, ColonyConfig config, List<Transform> points)
        {
            var colony = new GameObject("CoelophysisColony").AddComponent<ColonyController>();
            colony.transform.SetParent(spawnPoint);
            colony.SetInfo(config, spawnPoint);
            colony.SetPoints(points);
            colony.SetStrategic(new CoelophysisHunteringStrategy(5));
            colony.Spawn();
            return colony;
        }
    }

    public class PlaceriasColonyFactory : IColonyFactory
    {
        public ColonyController Create(Transform spawnPoint, ColonyConfig config, List<Transform> points)
        {
            var colony = new GameObject("PlaceriassColony").AddComponent<ColonyController>();
            colony.transform.SetParent(spawnPoint);
            colony.SetInfo(config, spawnPoint);
            colony.SetPoints(points);
            colony.SetStrategic(new PlaceriasHuntingStrategy(15));
            colony.Spawn();
            return colony;
        }
    }
}
