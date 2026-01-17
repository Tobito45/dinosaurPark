using Character;
using System;
using UnityEngine;

namespace Character
{
    public class PlayerProxy : IPlayerProxy
    {
        private IPlayerProxy _mainPlayer;

        public PlayerProxy(CharacterGO mainPlayer) => _mainPlayer = mainPlayer;

        public void AddOnHealthChange(Action<int> action) =>
            _mainPlayer.AddOnHealthChange(action);

        public GameObject GetMainPlayer() => _mainPlayer.GetMainPlayer();

        public void DealDamage(int damage) =>
            _mainPlayer.DealDamage(damage);

        public void TeleportToPoint(Vector3 point) =>
            _mainPlayer.TeleportToPoint(point);

        public bool PutItemToInventory(ItemRuntimeInfo info) =>
            _mainPlayer.PutItemToInventory(info);

        public void DropItemFromInventory() =>
            _mainPlayer.DropItemFromInventory();   

        public Camera GetPlayerCamera() =>
            _mainPlayer.GetPlayerCamera();
        public Transform GetPlayerCameraTransform() =>
            _mainPlayer.GetPlayerCameraTransform();
    }

    public interface IPlayerProxy
    {
        void AddOnHealthChange(Action<int> action);
        GameObject GetMainPlayer();
        void DealDamage(int damage);
        void TeleportToPoint(Vector3 point);
        bool PutItemToInventory(ItemRuntimeInfo info);
        void DropItemFromInventory();
        Camera GetPlayerCamera();
        Transform GetPlayerCameraTransform();   
    }
}
