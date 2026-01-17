using Dinosaurus.States;
using Steamworks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Dinosaurus.Command
{
    public interface IDinoCommand
    {
        void Execute(DinosaurusController dino);
    }
    public class MoveToPointDinoCommand : IDinoCommand
    {
        private Vector3 _point;
        private bool _ignoreHunting;

        public MoveToPointDinoCommand(Vector3 point) => _point = point;
        public MoveToPointDinoCommand(Vector3 point, bool ignoreHunting)
        {
            _ignoreHunting = ignoreHunting;
            _point = point;
        }

        public void Execute(DinosaurusController dino)
        {
            if (!_ignoreHunting && dino.CurrentState.IsHunting())
                return;

            dino.ChangeState(new WalkDinoState(_point, DinoRandom.GenerateOffset(_point)));
        }
    }

    public class HunteringDinoCommand : IDinoCommand
    {
        public void Execute(DinosaurusController dino) => dino.ChangeState(new HunteringDinoState());
    }
    
    public class TimerOverIdleDinoCommand : IDinoCommand
    {
        public void Execute(DinosaurusController dino) => dino.ChangeState(new IdleDinoState(dino.CurrentState));
    }


    public class GatheringDinoCommand : IDinoCommand
    {
        private Vector3 _center;
        public GatheringDinoCommand(Vector3 center) => _center = center;

        public void Execute(DinosaurusController dino)
        {
            if (!dino.CurrentState.CanBeInterruptedByColony() || dino.CurrentState.IsHunting())
                return;
        
            dino.ChangeState(new GatheringDinoState(dino.CurrentState, DinoRandom.GenerateOffset(_center), _center));
        }
    }


    internal static class DinoRandom
    {
        internal static Vector3 GenerateOffset(Vector3 position)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 4f;
            return new Vector3(randomCircle.x, 0, randomCircle.y);
        }
    }
}   