using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject multiMenu, multiLobby;

    [SerializeField]
    private GameObject chatPanel, textObject;
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private int maxChatMessages = 20;
    
    [SerializeField] 
    private GameObject playerFieldBox, playerCardPrefab;
    [SerializeField] 
    private GameObject readyButton, NotreadyButton, startButton;
    private List<Message> chatMessages = new List<Message>();

    public bool connected;
    public bool inGame;
    public bool isHost;
    public ulong myClientId;

    public Dictionary<ulong, GameObject> playerInfo = new Dictionary<ulong, GameObject>();

    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
       startButton.GetComponent<Button>().onClick.AddListener(() => FindFirstObjectByType<SceneChanger>().StartGame());
    }

    private void Update()
    {
        if (inputField.text != string.Empty && inputField.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.text == " ")
                {
                    inputField.text = string.Empty;
                    inputField.DeactivateInputField();
                    return;
                }
                NetworkTransmission.instance.IWishToSendAChatServerRPC(inputField.text, myClientId);
                inputField.text = string.Empty;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputField.ActivateInputField();
                inputField.text = " ";
            }
        }
    }

    public void SendMessageToChat(string text, ulong fromWho, bool server)
    {
        if(chatMessages.Count >= maxChatMessages)
        {
            Destroy(chatMessages[0].TextObject.gameObject);
            chatMessages.Remove(chatMessages[0]);
        }
        Message newMessage = new Message();
        string name = "Server";
        
        if (!server)
            if (playerInfo.ContainsKey(fromWho))
                name = playerInfo[fromWho].GetComponent<PlayerInfo>().steamName;

        newMessage.Text = name + ": " + text; 
        GameObject nexText = Instantiate(textObject, chatPanel.transform);
        newMessage.TextObject = nexText.GetComponent<TMP_Text>();
        newMessage.TextObject.text = newMessage.Text;

        chatMessages.Add(newMessage);
    }
    public void ClearChat()
    {
        chatMessages.Clear();
        GameObject[] chat = GameObject.FindGameObjectsWithTag("ChatMessage");
        foreach (GameObject chit in chat)
        {
            Destroy(chit);
        }
        Debug.Log("clearing chat");
    }

    public void HostCreated()
    {
        multiMenu.SetActive(false);
        multiLobby.SetActive(true);
        isHost = true;
        connected = true;
    }

    public void ConnectedAsClient()
    {
        multiMenu.SetActive(false);
        multiLobby.SetActive(true);
        isHost = false;
        connected = true;
    }

    public void Disconnected()
    {
        playerInfo.Clear();
        GameObject[] playerCards = GameObject.FindGameObjectsWithTag("PlayerCard");
        foreach (GameObject playerCard in playerCards)
            Destroy(playerCard);

        multiMenu.SetActive(true);
        multiLobby.SetActive(false);
        isHost = false;
        connected = false;
    }

    public void AddPlayerToDictionary(ulong _cliendId, string _steamName, ulong _steamId)
    {
        if (!playerInfo.ContainsKey(_cliendId))
        {
            PlayerInfo _pi = Instantiate(playerCardPrefab, playerFieldBox.transform).GetComponent<PlayerInfo>();
            _pi.steamId = _steamId;
            _pi.steamName = _steamName;
            playerInfo.Add(_cliendId, _pi.gameObject);
        }
    }

    public void UpdateClients()
    {
        foreach (KeyValuePair<ulong, GameObject> _player in playerInfo)
        {
            ulong _steamId = _player.Value.GetComponent<PlayerInfo>().steamId;
            string _steamName = _player.Value.GetComponent<PlayerInfo>().steamName;
            ulong _clientId = _player.Key;

            NetworkTransmission.instance.UpdateClientsPlayerInfoClientRPC(_steamId, _steamName, _clientId);

        }
    }

    public void RemovePlayerFromDictionary(ulong _steamId)
    {
        GameObject _value = null;
        ulong _key = 100;
        foreach (KeyValuePair<ulong, GameObject> _player in playerInfo)
        {
            if (_player.Value.GetComponent<PlayerInfo>().steamId == _steamId)
            {
                _value = _player.Value;
                _key = _player.Key;
            }
        }
        if (_key != 100)
        {
            playerInfo.Remove(_key);
        }
        if (_value != null)
        {
            Destroy(_value);
        }
    }
    public void ReadyButton(bool _ready)
    {
        NetworkTransmission.instance.IsTheClientReadyServerRPC(_ready, myClientId);
    }

    public bool CheckIfPlayersAreReady()
    {
        bool _ready = false;

        foreach (KeyValuePair<ulong, GameObject> _player in playerInfo)
        {
            if (!_player.Value.GetComponent<PlayerInfo>().isReady)
            {
                startButton.SetActive(false);
                return false;
            }
            else
            {
                startButton.SetActive(true);
                _ready = true;
            }
        }

        return _ready;
    }
    public void Quit()
    {
        Application.Quit();
    }


    private class Message
    {
        public string Text { get; set; }
        public TMP_Text TextObject { get; set; }
    }
}
