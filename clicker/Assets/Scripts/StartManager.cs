using System.Net;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerdataObj;
    public Playerdata playerdata;

    [SerializeField] private CanvasGroup game_map;
    [SerializeField] private GameObject game_paths;

    [SerializeField] private MenuLogic menu_logic_script;

    private bool is_determined = false;
    void Start()
    {
        StartMultiplayer();
        SetVisibilities();
        playerdata = playerdataObj.GetComponent<Playerdata>();
    }

    void SetVisibilities()
    {
        // game_map.SetActive(true);
        game_map.alpha = 0;
        game_map.blocksRaycasts = false;
        game_map.interactable = false;

        game_paths.SetActive(false);
        menu_logic_script.MenuStart();
    }

    async void StartMultiplayer()
    {
        await UnityServices.InitializeAsync();

        /*
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        */
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
