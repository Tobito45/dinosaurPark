using DI;
using Dinosaurus;
using UnityEngine;

[Priority]
public class DinoSpawnerGlobal : MonoBehaviour
{
    public void Init()
    {
        foreach (var spawner in GetComponentsInChildren<DinoSpawner>())
            spawner.Init();
    }
}
