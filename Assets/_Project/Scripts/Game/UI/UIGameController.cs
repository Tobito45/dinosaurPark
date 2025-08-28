using UnityEngine;



namespace GameUI
{
    public class UIGameController : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField]
        private Canvas _statisticsCanvas;

        [SerializeField]
        private Canvas _consoleUI;

        private bool isOpen;

        private void Start()
        {
            _statisticsCanvas.gameObject.SetActive(false);
            _consoleUI.gameObject.SetActive(false);
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                _statisticsCanvas.gameObject.SetActive(true);

            if (Input.GetKeyUp(KeyCode.Tab))
                _statisticsCanvas.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.BackQuote)) // ~ key
            {
                isOpen = !isOpen;
                _consoleUI.gameObject.SetActive(isOpen);


                //NEED to add some block for movement and camera movement
                if (isOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(true);

                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    GameClientsNerworkInfo.Singleton.CharacterPermissions.SetUIStunPermissons(false);

                }
            }
        }
    }
}
