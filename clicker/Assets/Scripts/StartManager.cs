using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameObject playerdataObj;
    public Playerdata playerdata;

    public Dictionary<Colors, string> fullname_dict = new Dictionary<Colors, string>();
    public Dictionary<Colors, string> shortname_dict = new Dictionary<Colors, string>();
    public Dictionary<Colors, Color> color_dict = new Dictionary<Colors, Color>();

    [SerializeField] private CanvasGroup game_map;
    [SerializeField] private GameObject game_paths;

    [SerializeField] private MenuLogic menu_logic_script;

    private bool is_determined = false;
    void Start()
    {
        StartMultiplayer();
        SetVisibilities();
        InitDicts();
        playerdata = playerdataObj.GetComponent<Playerdata>();
    }

    void InitDicts()
    {
        fullname_dict.Add(Colors.Neutral, "Разрозненные региональные князья");
        fullname_dict.Add(Colors.Red, "Второе народное ополчение Минина и Пожарского");
        fullname_dict.Add(Colors.Blue, "Шведская интервенция за царя Шуйского");
        fullname_dict.Add(Colors.Green, "Крестьянское восстание Болотникова");
        fullname_dict.Add(Colors.Yellow, "Войско Лжедмитрия II при поддержке Речи Посполитой");

        shortname_dict.Add(Colors.Neutral, "Нейтральные земли");
        shortname_dict.Add(Colors.Red, "Минин и Пожарский");
        shortname_dict.Add(Colors.Blue, "Шуйский");
        shortname_dict.Add(Colors.Green, "Болотников");
        shortname_dict.Add(Colors.Yellow, "Лжедмитрий II");

        color_dict.Add(Colors.Neutral, Color.white);
        color_dict.Add(Colors.Red, Color.red);
        color_dict.Add(Colors.Blue, Color.blue);
        color_dict.Add(Colors.Green, Color.green);
        color_dict.Add(Colors.Yellow, Color.yellow);
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
        /*
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
        */
    }
}
