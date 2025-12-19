using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TeamButton : NetworkBehaviour
{
    public Colors button_color;
    public StartManager dicts_holder;
    public Playerdata player_data;
    public TMP_Text team_text;
    public Image panel;
    public Button button;
    public MenuLogic team_buttons_dict_holder;

    public NetworkVariable<bool> is_occupied;
    public NetworkVariable<ulong> occupied_by;

    private void Awake()
    {
        is_occupied = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        occupied_by = new NetworkVariable<ulong>(
            999,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    }

    public void Init(Colors given_color)
    {
        button_color = given_color;

        button.image.color = dicts_holder.color_dict[button_color];

        TMP_Text button_text = button.GetComponentInChildren<TMP_Text>();
        button_text.text = dicts_holder.fullname_dict[button_color];
        button_text.enableAutoSizing = true;
        button_text.fontSizeMin = 10;
        button_text.fontSizeMax = 50;

        player_data.current_team = Colors.Neutral;
        team_text.text = "Вы не выбрали команду!";
        panel.color = Color.white;

        button.onClick.AddListener(() => TeamButtonClickServerRpc(NetworkManager.Singleton.LocalClientId, player_data.current_team));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void TeamButtonClickServerRpc(ulong client_id, Colors local_team)
    {
        Debug.Log($"find click: id {client_id}, team {local_team}, is_occ {is_occupied.Value}");
        if (is_occupied.Value)
        {
            return;
        }
        is_occupied.Value = true;
        occupied_by.Value = client_id;

        TeamButtonClickClientRpc(client_id);

        if (local_team != Colors.Neutral)
        {
            team_buttons_dict_holder.team_buttons_dict[local_team].is_occupied.Value = false;
            team_buttons_dict_holder.team_buttons_dict[local_team].occupied_by.Value = 999;
        }
    }

    [ClientRpc]
    void TeamButtonClickClientRpc(ulong client_id)
    {
        Debug.Log($"local click: id {client_id}, nm id {NetworkManager.Singleton.LocalClientId}");
        if (NetworkManager.Singleton.LocalClientId != client_id)
        {
            return;
        }

        player_data.current_team = button_color;
        team_text.text = $"Ваш выбор: {dicts_holder.shortname_dict[button_color]}";
        panel.color = dicts_holder.color_dict[button_color];
    }

    private void Update()
    {
        button.interactable = !is_occupied.Value;
    }
}
