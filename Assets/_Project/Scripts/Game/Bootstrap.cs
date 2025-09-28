using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Bootstrap
{
    public class Bootstrap : NetworkBehaviour
    {
        [SerializeField, SerializeReference]
        private List<GameObject> _bootstrap = new();

        public override void OnNetworkSpawn()
        {
            _bootstrap.ForEach(bootstrap =>
            {
                IInit init = bootstrap.GetComponent<IInit>();

                if(init == null)
                {
                    Debug.Log("init not found");
                    return;
                }

                init.Init();
            });
        }
    }

    public interface IInit
    {
        public void Init();
    }
}
