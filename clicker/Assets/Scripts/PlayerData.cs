using UnityEngine;
using Unity.Netcode;

public class Playerdata : NetworkBehaviour
{
    public NetworkVariable<string> current_team = new NetworkVariable<string>("neutral",
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
}
