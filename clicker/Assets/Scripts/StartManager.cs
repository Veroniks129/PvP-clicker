using System.Net;
using Unity.Netcode;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerdataObj;
    public Playerdata playerdata;

    private bool is_determined = false;
    void Start()
    {
        playerdata = playerdataObj.GetComponent<Playerdata>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_determined) return;
        if (NetworkManager.Singleton.IsHost)
        {
            playerdata.current_team = Colors.Red;
            is_determined = true;
        }
        else if (NetworkManager.Singleton.IsClient) {
            playerdata.current_team = Colors.Blue;
            is_determined = true;
        }
    }
}
