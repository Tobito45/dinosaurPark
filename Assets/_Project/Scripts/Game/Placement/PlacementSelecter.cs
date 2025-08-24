using UnityEngine;
using UnityEngine.UI;


namespace Placement
{
    public class PlacementSelecter : MonoBehaviour
    {
        public Placement SelectedPlacement { get; private set; }


        public void SelectItem(Placement placement) => SelectedPlacement = placement;

        public void DeselectItem() => SelectedPlacement = null;

    }
}
