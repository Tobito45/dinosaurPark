using UnityEngine;

namespace Inventory
{
    public interface IPlaceItem
    {
        public bool CheckIfCanPlaceItem(int id);
        public void PlaceItem(int id);
        public void OnHoverEnter(int id);
        public void OnHoverExit();
    }
}
