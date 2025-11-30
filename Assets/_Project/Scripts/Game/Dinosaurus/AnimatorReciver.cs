using System;
using UnityEngine;

public class AnimatorReciver : MonoBehaviour
{
    public Action OnAttackAStart, OnAttackEnd;
    private void AttackStart() => OnAttackAStart?.Invoke();
    private void AttackEnd() => OnAttackEnd?.Invoke();
}
