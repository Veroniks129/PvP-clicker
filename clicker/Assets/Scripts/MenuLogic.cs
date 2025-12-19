using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;

public class MenuLogic : NetworkBehaviour
{
    [SerializeField] private TMP_Text join_code_field;
    [SerializeField] private TMP_InputField join_code_input;
    [SerializeField] private GameObject host_menu;
    [SerializeField] private GameObject common_menu;
    [SerializeField] private GameObject choice_buttons;
    [SerializeField] private GameObject exit_button;

    [SerializeField] private TeamButton red_team_button;
    [SerializeField] private TeamButton green_team_button;
    [SerializeField] private TeamButton blue_team_button;
    [SerializeField] private TeamButton yellow_team_button;

    [SerializeField] private TMP_Text players_count_field;
    private NetworkVariable<int> players_count = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private CanvasGroup game_map_cg;
    [SerializeField] private MapInit game_map_mi;
    [SerializeField] private GameObject game_paths;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject map_blocker;

    private ISession session;
    bool is_host;
    private NetworkVariable<ulong> host_id = new NetworkVariable<ulong>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void MenuStart()
    {
        menu.SetActive(true);
        ChoiceMenuVisibility(true);
        HostMenuVisibility(false);
        ClientMenuVisibility(false);
    }

    public void SelectHost()
    {
        is_host = true;
        ChoiceMenuVisibility(false);
        ClientMenuVisibility(false);
        HostGame();
        HostMenuVisibility(true);
        InitButtons();
        game_map_mi.InitializeMap();
    }

    public void SelectClient()
    {
        is_host = false;
        ChoiceMenuVisibility(false);
        HostMenuVisibility(false);
        JoinGame(join_code_input.text);
        ClientMenuVisibility(true);
        InitButtons();
    }

    public void SelectExit()
    {
        Debug.Log($"Selected Exit");
        HostMenuVisibility(false);
        ClientMenuVisibility(false);
        ExitGame();
        ChoiceMenuVisibility(true);
        join_code_field.text = "...wait...";
        players_count_field.text = "...wait...";
    }

    public void InitButtons()
    {
        red_team_button.Init(Colors.Red);
        green_team_button.Init(Colors.Green);
        blue_team_button.Init(Colors.Blue);
        yellow_team_button.Init(Colors.Yellow);
    }

    async System.Threading.Tasks.Task HostGame()
    {
        var options = new SessionOptions
        {
            MaxPlayers = 4
        }.WithRelayNetwork();

        session = await MultiplayerService.Instance.CreateSessionAsync(options);
        join_code_field.text = session.Code;

        NetworkManager.Singleton.StartHost();
    }

    async System.Threading.Tasks.Task JoinGame(string join_code)
    {
        session = await MultiplayerService.Instance.JoinSessionByCodeAsync(join_code);
        join_code_field.text = session.Code;
        NetworkManager.Singleton.StartClient();
    }

    async System.Threading.Tasks.Task ExitGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        await session.LeaveAsync();
        session = null;
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

    void ChoiceMenuVisibility(bool is_active)
    {
        choice_buttons.SetActive(is_active);
        exit_button.SetActive(!is_active);
    }
    
    public void TriggerStart()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        game_map_cg.alpha = 1;
        game_map_cg.blocksRaycasts = true;
        game_map_cg.interactable = true;

        map_blocker.SetActive(false);
        game_paths.SetActive(true);
        menu.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            host_id.Value = NetworkManager.Singleton.LocalClientId;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedHost;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedHost;
        } else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedLocal;
        }

        host_id.OnValueChanged += (_, newValue) => Debug.Log($"Host ID received: {newValue}");

        players_count.OnValueChanged += (_, newValue) =>
        {
            players_count_field.text = $"{newValue}/4";
        };
    }

    private void OnClientConnectedHost(ulong _)
    {
        players_count.Value = NetworkManager.Singleton.ConnectedClients.Count;
        host_id.Value = NetworkManager.Singleton.LocalClientId;
    }

    private void OnClientDisconnectedHost(ulong _)
    {
        players_count.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
    
    // Тут должен был быть выход клиента при выходе хоста, но мне никак не удалось заставить его работать
    private void OnClientDisconnectedLocal(ulong disconnected_id)
    {
        Debug.Log($"and so I am here: disc_id: {disconnected_id}, host_id.Value: {host_id.Value}, is_host: {is_host}");
        if (!IsOwner && (!NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.ShutdownInProgress))
        {
            SelectExit();
        }
    }
}
