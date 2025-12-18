using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

//using System.Collections.Generic;

public class ClickProcesser : NetworkBehaviour
{
    // Example of variable, working in network
    //public NetworkVariable<int> var_example = new NetworkVariable<int>(0,
    //    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Example of external object, used in network code
    [SerializeField] private TMP_Text external_example;

    public GameObject playerdataObj; // set in editor
    public Playerdata playerdata; 
    public Nodedata nodedata;

    private Dictionary<Colors, Color> colorDict = new Dictionary<Colors, Color>();

    // First entry into the network
    public override void OnNetworkSpawn()
    {
        InitColorDict();
        playerdata = playerdataObj.GetComponent<Playerdata>();
        nodedata = GetComponent<Nodedata>();
        // From the start getting info from the host
        UpdateText(nodedata.power.Value);
        UpdateColor(nodedata.color.Value);

        // Subscribing to changing (by host) of nets variables 
        nodedata.power.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateText(newValue);
        };

        nodedata.color.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateColor(newValue);
        };
    }

  
    void InitColorDict()
    {
        colorDict.Add(Colors.Neutral, Color.white);
        colorDict.Add(Colors.Red, Color.red);
        colorDict.Add(Colors.Blue, Color.blue);
    }

    // This function should be called when network vars changed on host and
    // we must do something locally
    private void UpdateText(int var_example_new_value)
    {
        // In this example, text UI (showing amount of clicks) is updated
        external_example.text = var_example_new_value.ToString();
    }

    private void UpdateColor(Colors new_color)
    {
        //GameObject thisCityObj = transform.parent.gameObject;
        //GameObject nodeObj = thisCityObj.transform.Find("node").gameObject;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.color = colorDict[new_color];
    }

    // What we are doing when clicking object locally
    public void OnLocalClick()
    {
        // Just send click to host should be enough
        HostClickHandlerRpc();
    }

    // Construction below says that we call function on the client (locally),
    // but execute it on the server (host)
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void HostClickHandlerRpc()
    {
        string city = nodedata.GetName();
        Debug.Log($"Clicked on {city} : {nodedata.color.Value} color, {nodedata.power.Value} power");
        bool is_any_neigh_my = nodedata.IsAnyNeighMy(playerdata.current_team);
        bool is_my_color = (nodedata.color.Value == playerdata.current_team);
        if (!is_any_neigh_my && !is_my_color)
        {
            Debug.Log("not nieghbor nor your city");
            //if (playerdata.current_team == Colors.Red)
            //{
            //    playerdata.current_team = Colors.Blue;
            //}
            //else
            //{
            //    playerdata.current_team = Colors.Red;
            //}
            return;
        }
        if (nodedata.power.Value == 0)
        {
            Debug.Log("its power = 0 => conquer it");
            nodedata.color.Value = playerdata.current_team;
            nodedata.power.Value++;
        }
        else if (nodedata.color.Value == playerdata.current_team)
        {
            Debug.Log("its color the same as player's");
            nodedata.power.Value++;
        }
        else
        {
            Debug.Log("it has another color");
            nodedata.power.Value--;
            if (nodedata.power.Value == 0)
            {
                nodedata.color.Value = Colors.Neutral;
            }
        }

        //if (playerdata.current_team == Colors.Red)
        //{
        //    playerdata.current_team = Colors.Blue;
        //}
        //else
        //{
        //    playerdata.current_team = Colors.Red;
        //}
    }
}
