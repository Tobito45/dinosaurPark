using DI;
using Dinosaurus.Factory;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Dinosaurus.Builder
{
    public class ColonyBuilder
    {
        private Transform _spawnPoint;
        private ColonyConfig _config;
        private List<Transform> _points; 
        private DinoType _factory;


        public ColonyBuilder WithSpawnPoint(Transform spawnPoint)
        {
            _spawnPoint = spawnPoint;
            return this;
        }

        public ColonyBuilder WithConfig(ColonyConfig config)
        {
            _config = config;
            return this;
        }

        public ColonyBuilder WithPoints(List<Transform> points)
        {
            _points = points;
            return this;
        }

        public ColonyBuilder WithFactory(DinoType factory)
        {
            _factory = factory;
            return this;
        }

        public ColonyController Build()
        {
            var factory = UnityEngine.Object.FindFirstObjectByType<GameBootstrapper>().Container.Resolve(_factory);
            var colony = (factory as IColonyFactory).Create(_spawnPoint, _config, _points);
            return colony;
        }
    }
}
