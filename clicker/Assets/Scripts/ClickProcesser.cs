using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ClickProcesser : NetworkBehaviour
{
    // Example of variable, working in network
    public NetworkVariable<int> var_example = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Example of external object, used in network code
    [SerializeField] private TMP_Text external_example;

    // First entry into the network
    public override void OnNetworkSpawn()
    {
        // From the start getting info from the host
        LocalSync(var_example.Value);

        // Subscribing to changing (by host) of nets variables 
        var_example.OnValueChanged += (oldValue, newValue) =>
        {
            LocalSync(newValue);
        };
    }

    // This function should be called when network vars changed on host and
    // we must do something locally
    private void LocalSync(int var_example_new_value)
    {
        // In this example, text UI (showing amount of clicks) is updated
        external_example.text = var_example_new_value.ToString();
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
        // Doing here any logic (in this example just adding 1 to click counter)
        var_example.Value++;
    }
}
