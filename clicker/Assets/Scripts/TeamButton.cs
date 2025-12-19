using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamButton : MonoBehaviour
{
    public Colors button_color;
    public StartManager dicts_holder;
    public Playerdata player_data;
    public TMP_Text team_text;
    public Image panel;
    public Button button;

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

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        player_data.current_team = button_color;
        team_text.text = $"Ваш выбор: {dicts_holder.shortname_dict[button_color]}";
        panel.color = dicts_holder.color_dict[button_color];
    }
}
