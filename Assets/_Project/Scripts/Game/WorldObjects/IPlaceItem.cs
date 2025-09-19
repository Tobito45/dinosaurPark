using UnityEngine;

namespace Inventory
{
    public interface IPlaceItem
    {
        public bool CheckIfCanPlaceItem(ItemRuntimeInfo id);
        public void PlaceItem(ItemRuntimeInfo id);
        public void OnHoverEnter(ItemRuntimeInfo id);
        public void OnHoverExit();
    }
}
