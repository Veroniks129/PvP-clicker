using Unity.Netcode;
using UnityEngine;

public class SimpleNetHUD : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Start Server")) NetworkManager.Singleton.StartServer();
            if (GUILayout.Button("Start Client")) NetworkManager.Singleton.StartClient();
        }
        else
        {
            GUILayout.Label($"Mode: {(NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client")}");
        }

        GUILayout.EndArea();
    }
}
