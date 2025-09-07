using UnityEngine;

namespace Inventory
{
    public interface IPlaceItem
    {
        public bool CheckIfCanPlaceItem(string id);
        public void PlaceItem(string id);
        public void OnHoverEnter(string id);
        public void OnHoverExit();
    }
}
