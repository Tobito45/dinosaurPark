using Dinosaurus;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dinosaurus.States
{
    public interface IDinoState
    {
        void Enter(DinosaurusController dino);
        void Update(DinosaurusController dino);
        void Exit(DinosaurusController dino);

        bool CanBeInterruptedByColony();    
        bool IsWaitingOnPoint();            
        bool IsGathering();                  
        bool IsHunting();
    }

    public class WalkDinoState : IDinoState
    {
        private Vector3 _target, _offset;
        public WalkDinoState(Vector3 target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        public void Enter(DinosaurusController dino)
        {
            dino.SetNextPoint(_target, _offset);
            dino.SetAnimationRun(true);
        }

        public void Exit(DinosaurusController dino) => dino.SetAnimationRun(false);

        public void Update(DinosaurusController dino)
        {
            if (dino.IsDinoReachedPoint())
                dino.OnEnterThePoint?.Invoke(dino);
        }

        public bool CanBeInterruptedByColony() => true;
        public bool IsWaitingOnPoint() => false;
        public bool IsGathering() => false;
        public bool IsHunting() => false;
    }

    public class OnPointDinoState : IDinoState
    {
        public void Enter(DinosaurusController dino) => dino.SetStopNavMesh(true);

        public void Exit(DinosaurusController dino) => dino.SetStopNavMesh(false);

        public void Update(DinosaurusController dino) { }

        public bool CanBeInterruptedByColony() => true;
        public bool IsWaitingOnPoint() => true;
        public bool IsGathering() => false;
        public bool IsHunting() => false;
    }
    public class IdleDinoState : IDinoState
    {
        public IDinoState _stateBefore;
        public IdleDinoState(IDinoState stateBefore) => _stateBefore = stateBefore;

        public void Enter(DinosaurusController dino)
        {
            if (dino.IsAnimationWait())
                return;

            dino.SetAnimationWait(true);
            dino.SetStopNavMesh(true);
            dino.OnIdleEnd += OnIdleEnded;
        }

        public void Exit(DinosaurusController dino)
        {
            dino.SetAnimationWait(false);
            dino.SetStopNavMesh(false);
            dino.OnIdleEnd -= OnIdleEnded;
        }

        public void Update(DinosaurusController dino) { }

        public void OnIdleEnded(DinosaurusController dino) => dino.ChangeState(_stateBefore);

        public bool CanBeInterruptedByColony() => false;
        public bool IsWaitingOnPoint() => false;
        public bool IsGathering() => false;
        public bool IsHunting() => false;
    }

    public class StandDinoState : IDinoState
    {
        public void Enter(DinosaurusController dino) { }

        public void Exit(DinosaurusController dino) { }

        public void Update(DinosaurusController dino) { }

        public bool CanBeInterruptedByColony() => true;
        public bool IsWaitingOnPoint() => true;
        public bool IsGathering() => false;
        public bool IsHunting() => false;
    }

    public class GatheringDinoState : IDinoState
    {
        public IDinoState _stateBefore;
        private Vector3 _target, _offset;
        public GatheringDinoState(IDinoState stateBefore, Vector3 offset, Vector3 target)
        {
            _stateBefore = stateBefore;
            _offset = offset;
            _target = target;
        }

        public void Enter(DinosaurusController dino)
        {
            dino.SetNextPoint(_target, _offset);
            dino.SetAnimationRun(true);
        }

        public void Exit(DinosaurusController dino) => dino.SetAnimationRun(false);

        public void Update(DinosaurusController dino)
        {
            if (dino.IsDinoReachedPoint())
                dino.OnEnterThePoint?.Invoke(dino);
        }
        public bool CanBeInterruptedByColony() => true;
        public bool IsWaitingOnPoint() => false;
        public bool IsGathering() => true;
        public bool IsHunting() => false;
    }

    public class HunteringDinoState : IDinoState
    {
        public void Enter(DinosaurusController dino) => dino.SetAnimationRun(true);

        public void Exit(DinosaurusController dino) => dino.SetAnimationRun(false);

        public void Update(DinosaurusController dino) { }

        public bool CanBeInterruptedByColony() => false;
        public bool IsWaitingOnPoint() => false;
        public bool IsGathering() => false;
        public bool IsHunting() => true;
    }
}