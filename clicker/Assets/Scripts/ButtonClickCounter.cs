using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ButtonClickCounter : NetworkBehaviour
{
    public NetworkVariable<int> count = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private TMP_Text visible_counter;

    public override void OnNetworkSpawn()
    {
        UpdateText(count.Value);

        count.OnValueChanged += (oldValue, newValue) =>
        {
            UpdateText(newValue);
        };
    }

    public void OnLocalButtonClick()
    {
        SendClickServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendClickServerRpc(ServerRpcParams rpcParams = default)
    {
        count.Value++;
    }

    private void UpdateText(int value)
    {
        if (visible_counter != null)
        {
            visible_counter.text = value.ToString();
        }
    }
}
