using UnityEngine;



namespace GameUI
{
    public class UIGameController : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField]
        private Canvas _statisticsCanvas;

        private void Start() => _statisticsCanvas.gameObject.SetActive(false);


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
                _statisticsCanvas.gameObject.SetActive(true);

            if (Input.GetKeyUp(KeyCode.Tab))
                _statisticsCanvas.gameObject.SetActive(false);

        }
    }
}
