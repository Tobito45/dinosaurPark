using Character;
using DI;
using Dinosaurus;
using Dinosaurus.Builder;
using Dinosaurus.Factory;
using GameUI;
using Inventory;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace DI
{
    public class GameBootstrapper : NetworkBehaviour
    {
        private DIContainer _container = new DIContainer();
        public DIContainer Container => _container;

        public override void OnNetworkSpawn()
        {
            GameClientsNerworkInfo.Singleton.OnPlayerSet += Init;
            //Init();
        }

        private void Init(CharacterGO characterGO)
        {
            _container.RegisterAllWithPriority();

            _container.InitGlobal();

            //bind main player
            _container.Bind<PlayerProxy>(new PlayerProxy(characterGO));

            BindAll();
            BindDino();

            ResolveInit();
        }

        private void BindAll()
        {
            _container.Bind<ColonyBuilder>(new ColonyBuilder());
        }

        private void BindDino()
        {
            _container.Bind(DinoType.Placerias, new PlaceriasColonyFactory());
            _container.Bind(DinoType.Coelophysis, new CoelophysisColonyFactory());
        }

        private void ResolveInit()
        {
            _container.Resolve<DinoSpawnerGlobal>().Init();

            _container.Resolve<NPCSpawner>().Init();
            _container.Resolve<Spawner>().Init();
            _container.Resolve<StatisticsUI>().Init();

            _container.Resolve<PlayerInventoryController>().Init();

            _container.Resolve<BloodScreenController>().Init();
        }
    }
}
