using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Nodedata : NetworkBehaviour
{
    public List<Object> neighbors;
    public NetworkVariable<int> power = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<string> color = new NetworkVariable<string>("neutral",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
}
