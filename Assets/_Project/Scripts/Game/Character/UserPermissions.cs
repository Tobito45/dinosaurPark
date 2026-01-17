using System.Collections.Generic;
using UnityEngine;


namespace Character
{
    public class UserPermissions
    {
        private static readonly UserPermissions _instance = new UserPermissions();
        public static UserPermissions Singleton => _instance;
        private UserPermissions() { }  

        private HashSet<CharacterPermissionsType> _permissions = new();
        public bool HasPermission(CharacterPermissionsType characterPermissions) => _permissions.Contains(characterPermissions);

        public void AddPermission(CharacterPermissionsType characterPermissions) => _permissions.Add(characterPermissions);
        public void RemovePermission(CharacterPermissionsType characterPermissions) => _permissions.Remove(characterPermissions);

        public void SetUIStunPermissons(bool enable)
        {
            if (enable)
            {
                RemovePermission(CharacterPermissionsType.Movement);
                RemovePermission(CharacterPermissionsType.Input);
                RemovePermission(CharacterPermissionsType.CameraRotate);
                RemovePermission(CharacterPermissionsType.Jump);
            }
            else
            {
                AddPermission(CharacterPermissionsType.Movement);
                AddPermission(CharacterPermissionsType.Input);
                AddPermission(CharacterPermissionsType.CameraRotate);
                AddPermission(CharacterPermissionsType.Jump);
            }
        }

        public void SetBasePermissions()
        {
            AddPermission(CharacterPermissionsType.Movement);
            AddPermission(CharacterPermissionsType.Input);
            AddPermission(CharacterPermissionsType.CameraRotate);
            AddPermission(CharacterPermissionsType.Jump);
        }
    }

    public enum CharacterPermissionsType
    {
        Movement, Input, CameraRotate, Jump
    }
}


