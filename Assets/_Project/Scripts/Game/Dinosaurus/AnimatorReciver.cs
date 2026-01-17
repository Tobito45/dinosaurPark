using System;
using UnityEngine;

public class AnimatorReciver : MonoBehaviour
{
    public Action OnAttackStart, OnAttackEnd;
    public Action OnIdleStart, OnIdleEnd;
    private void AttackStart() => OnAttackStart?.Invoke();
    private void AttackEnd() => OnAttackEnd?.Invoke();

    private void IdleStart() => OnIdleStart?.Invoke();
    private void IdleEnd() => OnIdleEnd?.Invoke();
}
