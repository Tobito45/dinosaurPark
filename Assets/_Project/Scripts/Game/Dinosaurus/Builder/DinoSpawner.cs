using DI;
using Dinosaurus.Builder;
using Dinosaurus.Factory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Dinosaurus
{
    [Priority]
    public class DinoSpawner : NetworkBehaviour
    {
        [Inject]
        private ColonyBuilder _colonyBuilder;

        [SerializeField]
        private Transform _spawnPoint;
        [SerializeField]
        private List<Transform> _points;
        [SerializeField]
        private ColonyConfig config;
        [SerializeField]
        private DinoType typeFactory;

        public void Init()
        {
            if (!IsServer) return;

            this.Inject();

            var colony = _colonyBuilder
                .WithConfig(config)
                .WithSpawnPoint(_spawnPoint)
                .WithPoints(_points)
                .WithFactory(typeFactory)
                .Build();
        }
    }

    public enum DinoType
    {
        Coelophysis, Placerias
    }
}
