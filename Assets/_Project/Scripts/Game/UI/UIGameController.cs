using UnityEngine;

namespace GameUI
{
    public class UIGameController : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private Canvas _statisticsCanvas;
        [SerializeField] private Canvas _consoleCanvas;
        [SerializeField] private Canvas _npcStatsCanvas;

        private bool _consoleOpen;
        private bool _npcStatsOpen;

        private void Start()
        {
            SetCanvasActive(_statisticsCanvas, false);
            SetCanvasActive(_consoleCanvas, false);
            SetCanvasActive(_npcStatsCanvas, false);
        }

        private void Update()
        {
            HandleStatisticsCanvas();
            HandleConsoleCanvas();
            HandleNpcStatsCanvas();
        }

        private void HandleStatisticsCanvas()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                SetCanvasActive(_statisticsCanvas, true);

            if (Input.GetKeyUp(KeyCode.Tab))
                SetCanvasActive(_statisticsCanvas, false);
        }

        private void HandleConsoleCanvas()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _consoleOpen = !_consoleOpen;
                SetCanvasActive(_consoleCanvas, _consoleOpen, blockInput: _consoleOpen);
            }
        }

        private void HandleNpcStatsCanvas()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                _npcStatsOpen = !_npcStatsOpen;
                SetCanvasActive(_npcStatsCanvas, _npcStatsOpen, blockInput: _npcStatsOpen);
            }
        }

        private void SetCanvasActive(Canvas canvas, bool active, bool blockInput = false)
        {
            if (canvas == null) return;

            canvas.gameObject.SetActive(active);

            if (blockInput)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(true);
            }
            else if (!active) // restore only when closing
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(false);
            }
        }
    }
}
