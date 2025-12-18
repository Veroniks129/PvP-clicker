using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEditor;
using UnityEngine;

public class MenuLogic : NetworkBehaviour
{
    [SerializeField] private TMP_Text join_code_field;
    [SerializeField] private TMP_InputField join_code_input;
    [SerializeField] private GameObject host_menu;
    [SerializeField] private GameObject common_menu;

    [SerializeField] private CanvasGroup game_map;
    [SerializeField] private GameObject game_paths;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject map_blocker;

    public void MenuStart()
    {
        menu.SetActive(true);
        HostMenuVisibility(false);
        ClientMenuVisibility(false);
    }

    public void SelectHost()
    {
        ClientMenuVisibility(false);
        _ = HostGame();
        HostMenuVisibility(true);
    }

    public void SelectClient()
    {
        HostMenuVisibility(false);
        _ = JoinGame(join_code_input.text);
        ClientMenuVisibility(true);
    }

    async Task HostGame()
    {
        var options = new SessionOptions
        {
            MaxPlayers = 4
        }.WithRelayNetwork();

        var session = await MultiplayerService.Instance.CreateSessionAsync(options);
        join_code_field.text = session.Code;

        // NetworkManager.Singleton.StartHost();
    }

    async Task JoinGame(string join_code)
    {
        await MultiplayerService.Instance.JoinSessionByCodeAsync(join_code);

        // NetworkManager.Singleton.StartClient();
    }

    void HostMenuVisibility(bool is_active)
    {
        host_menu.SetActive(is_active);
        common_menu.SetActive(is_active);
    }

    void ClientMenuVisibility(bool is_active)
    {
        common_menu.SetActive(is_active);
    }
    
    public void TriggerStart()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        // game_map.SetActive(true);
        game_map.alpha = 1;
        game_map.blocksRaycasts = true;
        game_map.interactable = true;

        map_blocker.SetActive(false);
        game_paths.SetActive(true);
        menu.SetActive(false);
    }
}
