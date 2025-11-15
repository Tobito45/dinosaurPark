using Dinosaurus;
using UnityEngine;

public class PlaceriasController : DinosaurusController
{
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
        if (_target != null)
            return;

        Debug.Log("???");
        OnStartHuntering?.Invoke(this);
        _target = player;
        _timerWaring = -1;
        _navMeshAgent.destination = _target.transform.position;
    }
    protected override void OnRedZoneExit(GameObject player)
    {
        if (_target != player)
            return;

        Debug.Log("???2");
        OnEndHuntering?.Invoke(this);
        _target = null;
    }

    private float _timerWaring = -1;
    protected override void OnWarningZoneEnter(GameObject player) { }

    protected override void OnWarningZoneExit(GameObject player) { }
}
