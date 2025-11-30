using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private int _baseHealth = 100;
    public int Health { get; private set; }

    public event Action<int> OnHealthChange;

    private void Start() => Health = _baseHealth;

    public void DealDmg(int dgm)
    {
        Health -= dgm;
        if (Health <= 0)
            Death();

        OnHealthChange?.Invoke(Health);
    }

    public void Death()
    {
        Debug.Log("Character Dead");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            DealDmg(10);
    }

}
