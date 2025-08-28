using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using NPC;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CheatConsole : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public ScrollRect scrollRect;

    private Dictionary<string, Action<string[]>> commands;

    // [Header("NPC Info")]
    // [SerializeField]
    // private NPCSpawner _npcInfo;

    void Start()
    {
        commands = new Dictionary<string, Action<string[]>>()
        {
            { "help", args => PrintHelp() },
            { "godmode", args => EnableGodMode() },
            { "give", args => GiveItem(args) },
            { "spawn", args => SpawnItem(args) },
            { "tp", args => Teleport(args) },
            { "money", args => Money(args) },
            { "list", args => ListObjectsMap(args) }
        };

        inputField.onSubmit.AddListener(HandleInput);
    }

    void HandleInput(string input)
    {
        Log("> " + input);

        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string cmd = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        if (commands.TryGetValue(cmd, out var action))
        {
            action.Invoke(args);
        }
        else
        {
            Log("Unknown command. Type 'help'.");
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    void Log(string message)
    {
        outputText.text += "\n" + message;
        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 0f; // auto-scroll
    }

    // Example commands
    void PrintHelp()
    {
        Log("Available commands: " + string.Join(", ", commands.Keys));
    }

    void EnableGodMode()
    {
        Log("God mode enabled!");
    }

    // Player can get Object to inventory as Pedistals, Fountaine, Eggs
    void GiveItem(string[] args)
    {
        if (args.Length < 1) { Log("Usage: give <item>"); return; }
        Log($"Gave player {args[0]}!");
    }
    // Player can spawn item in his coord on head
    void SpawnItem(string[] args)
    {
        List<string> stringsTypes = new List<string> { "item", "mob" };
        if (args.Length < 2) { Log("Usage: spawn <" + string.Join("/", stringsTypes) + "> <name>"); return; }
        switch (args[0])
        {
            case "item":
                Log("Spawn next item: " + args[1]);
                break;
            case "mob":
                Log("Spawn next mob: " + args[1]);
                break;
            default:
                Log("Usage some of next type: " + string.Join(", ", stringsTypes));
                break;
        }
    }

    void Teleport(string[] args)
    {
        List<string> stringsTypes = new List<string> { "player", "npc" };
        if (args.Length < 2) { Log("Usage: tp <" + string.Join("/", stringsTypes) + "> <name>"); return; }

        switch (args[0])
        {
            case "player":
                Log("Tp to player: " + args[1]);
                break;
            case "npc":
                NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
                var player = GameClientsNerworkInfo.Singleton.MainPlayer;
                
                foreach (var npc in npcs)
                {
                    var npcName = npc.GetNPCInfo().Name;

                    if (npcName != null && npcName.ToLower() == args[1].ToLower())
                    {
                        Log("Found NPC: " + npcName);

                        var controller = player.GetComponent<CharacterController>();

                        // Disable controller so it doesn’t override position
                        controller.enabled = false;

                        // Teleport
                        player.transform.position = npc.transform.position + Vector3.up * 1f; // small offset to avoid ground clipping

                        // Re-enable controller
                        controller.enabled = true;

                        return;
                    }
                }

                Log("NPC with this name havent been found");
                break;
            default:
                Log("Usage some of next type: " + string.Join(", ", stringsTypes));
                break;
        }
        // if (args.Length < 3) { Log("Usage: tp <x> <y> <z>"); return; }
        // if (float.TryParse(args[0], out float x) &&
        //     float.TryParse(args[1], out float y) &&
        //     float.TryParse(args[2], out float z))
        // {
        //     var player = GameObject.FindWithTag("Player");
        //     if (player != null)
        //     {
        //         player.transform.position = new Vector3(x, y, z);
        //         Log($"Teleported player to {x},{y},{z}!");
        //     }
        //     else Log("No object tagged 'Player' found!");
        // }
        // else
        // {
        //     Log("Invalid coordinates!");
        // }
    }
    //NEED to add functionallity
    void Money(string[] args)
    {
        if (args.Length < 1) { Log("Usage: money <Number>"); return; }
        if (int.TryParse(args[0], out int moneyCount))
        {
            Log("Player money was increase by " + moneyCount);
        }
        else
        {
            Log("Number should be numerical");
        }
    }
    //NEED to a list of player
    void ListObjectsMap(string[] args)
    {
        List<string> stringsTypes = new List<string> { "players", "npcs" };
        if (args.Length < 1) { Log("Usage: list <" + string.Join("/", stringsTypes) + ">"); return; }

        switch (args[0])
        {
            case "npcs":
                NPCController[] npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
                int counter = 0;
                foreach (var npc in npcs)
                {
                    Log("NPC " + counter++ + ":" + npc.GetNPCInfo().Name);
                }
                break;
        }
    }
}