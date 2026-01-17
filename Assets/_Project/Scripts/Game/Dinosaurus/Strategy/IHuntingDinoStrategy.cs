using Character;
using DI;
using Dinosaurus;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


namespace Dinosaurus.Strategy
{
    public interface IHuntingDinoStrategy
    {
        void OnWarningZoneEnter(DinosaurusController dino, GameObject player);
        void OnWarningZoneExit(DinosaurusController dino, GameObject player);
        void OnRedZoneEnter(DinosaurusController dino, GameObject player);
        void OnRedZoneExit(DinosaurusController dino, GameObject player);
        void OnAttackZoneEnter(DinosaurusController dino, GameObject player);
        void OnAttackZoneExit(DinosaurusController dino, GameObject player);
        void OnAttackHitZoneEnter(DinosaurusController dino, GameObject player);
        void OnAttackHitZoneExit(DinosaurusController dino, GameObject player);
    }

    public class CoelophysisHunteringStrategy : IHuntingDinoStrategy
    {
        private float _startSpeed = -float.MinValue;
        private float _startRunSpeed = -float.MinValue;
        private int _damage;
        
        private PlayerProxy _characterFacade;

        public CoelophysisHunteringStrategy(int damage)
        {
            _damage = damage;
            _characterFacade = UnityEngine.Object.FindFirstObjectByType<GameBootstrapper>().Container.Resolve<PlayerProxy>();
        }

        public void OnAttackHitZoneEnter(DinosaurusController dino, GameObject player) 
        {
            if (player == _characterFacade.GetMainPlayer())
                _characterFacade.DealDamage(_damage);
        }

        public void OnAttackHitZoneExit(DinosaurusController dino, GameObject player) { }

        public void OnAttackZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (player == dino.Target)
                dino.CanAttack = true;
        }

        public void OnAttackZoneExit(DinosaurusController dino, GameObject player)
        {
            if (player == dino.Target)
                dino.CanAttack = false;
        }

        public void OnRedZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (player == dino.Target)
            {
                dino.SetNextPoint(dino.Target.transform.position, Vector3.zero);
                dino.SetAnimatorSpeed(_startSpeed);
                dino.SetNavMeshSpeed(_startRunSpeed);
            }
        }

        public void OnRedZoneExit(DinosaurusController dino, GameObject player) { }
        public void OnWarningZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (dino.Target != null)
                return;

            dino.OnStartHuntering?.Invoke(dino);
            dino.Target = player;

            dino.SetNextPoint(dino.Target.transform.position, Vector3.zero);

            if(_startRunSpeed == -float.MinValue)
                _startRunSpeed = dino.GetNavMeshSpeed();

            if (_startSpeed == -float.MinValue)
                _startSpeed = dino.GetAnimatorSpeed();

            dino.SetAnimatorSpeed(0.5f);
            dino.SetNavMeshSpeed(1f);
        }

        public void OnWarningZoneExit(DinosaurusController dino, GameObject player)
        {
            if (dino.Target == null)
                return;

            dino.OnEndHuntering?.Invoke(dino);
            dino.SetAnimatorSpeed(_startSpeed);
            dino.SetNavMeshSpeed(_startRunSpeed);
            dino.Target = null;
        }
    }


    public class PlaceriasHuntingStrategy : IHuntingDinoStrategy
    {
        private int _damage;
        private PlayerProxy _characterFacade;
        public PlaceriasHuntingStrategy(int damage)
        {
            _damage = damage;
            _characterFacade = UnityEngine.Object.FindFirstObjectByType<GameBootstrapper>().Container.Resolve<PlayerProxy>();
        }

        public void OnAttackHitZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (player == _characterFacade.GetMainPlayer())
                _characterFacade.DealDamage(_damage);
        }

        public void OnAttackHitZoneExit(DinosaurusController dino, GameObject player) { }

        public void OnAttackZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (player == dino.Target)
                dino.CanAttack = true;
        }

        public void OnAttackZoneExit(DinosaurusController dino, GameObject player)
        {
            if (player == dino.Target)
                dino.CanAttack = false;
        }

        public void OnRedZoneEnter(DinosaurusController dino, GameObject player)
        {
            if (dino.Target != null)
                return;

            dino.OnStartHuntering?.Invoke(dino);
            dino.Target = player;
            dino.SetNextPoint(dino.Target.transform.position, Vector3.zero);
        }

        public void OnRedZoneExit(DinosaurusController dino, GameObject player)
        {
            if (dino.Target != player)
                return;

            dino.OnEndHuntering?.Invoke(dino);
            dino.Target = null;
        }
        public void OnWarningZoneEnter(DinosaurusController dino, GameObject player) { }

        public void OnWarningZoneExit(DinosaurusController dino, GameObject player) { }
    }
}
