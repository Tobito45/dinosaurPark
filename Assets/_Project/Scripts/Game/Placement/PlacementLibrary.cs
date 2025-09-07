using Library;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlacementLibrary : BaseLibrary<Placement.Placement>
{
    private void Start() => Initialize("placement");
}
