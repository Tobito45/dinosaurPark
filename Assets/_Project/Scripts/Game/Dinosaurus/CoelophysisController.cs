using Dinosaurus;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CoelophysisController : DinosaurusController
{
    private float _startSpeed;
    private float _startRunSpeed;

    protected override void Start()
    {
        base.Start();

        _startSpeed = _navMeshAgent.speed;
        _startRunSpeed = _animator.speed;
    }

    protected override void OnAttackZoneEnter(GameObject player)
    {
        if (player == _target)
            _canAttack = true;
    }

    protected override void OnAttackZoneExit(GameObject player)
    {
        if (player == _target)
            _canAttack = false;
    }

    protected override void OnRedZoneEnter(GameObject player)
    {
        if (player == _target)
        {
            _timerWaring = -1;
            _navMeshAgent.destination = _target.transform.position;
            _navMeshAgent.speed = _startSpeed;
            _animator.speed = _startRunSpeed;
        }
    }
    protected override void OnRedZoneExit(GameObject player) { }

    private float _timerWaring = -1;
    protected override void OnWarningZoneEnter(GameObject player)
    {
        if (_target != null)
            return;

        _timerWaring = 0;
        OnStartHuntering?.Invoke(this);
        _target = player;

        _navMeshAgent.destination = _target.transform.position;
        _navMeshAgent.speed = 1;
        _animator.speed = 0.5f;
    }

    protected override void OnWarningZoneExit(GameObject player)
    {
        if (_target == null)
            return;

        _timerWaring = -1;
        OnEndHuntering?.Invoke(this);
        _navMeshAgent.speed = _startSpeed;
        _animator.speed = _startRunSpeed;
        _target = null;
    }
}
