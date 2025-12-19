using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WinScoreManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text scoreText; // UI очков
    [SerializeField] private GameObject mapBlocker;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private StartManager dictsHolder;

    [SerializeField] private Playerdata playerdata;
    [SerializeField] private Nodedata MoscowNode;
    private Coroutine scoreCoroutine;

    void Start()
    {
        // Если объект ещё не спавнен, запланируем инициализацию на OnNetworkSpawn
        if (IsSpawned)
        {
            Init();
        }
    }

    public override void OnNetworkSpawn()
    {
        Init();
    }

    private void Init()
    {
        StartManager sm = FindFirstObjectByType<StartManager>();

        if (playerdata == null) Debug.LogWarning("playerdata не найден!");
        if (MoscowNode == null) Debug.LogWarning("MoscowNode не найден!");
        if (scoreText == null) Debug.LogWarning("scoreText не назначен!");

        if (MoscowNode != null)
        {
            MoscowNode.color.OnValueChanged += OnMoscowColorChanged;
        }

        UpdateText();

        UpdateText();
    }


    void Update()
    {
        if (playerdata == null || MoscowNode == null) return;

        // Проверяем контроль Москвы
        if (MoscowNode.color.Value == playerdata.current_team)
        {
            if (scoreCoroutine == null && playerdata.current_team != Colors.Neutral)
                scoreCoroutine = StartCoroutine(AwardScore());
        }
        else
        {
            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = null;
            }
        }
    }

    private void OnMoscowColorChanged(Colors oldColor, Colors newColor)
    {
        // Лог для проверки, что клиент видит изменения
        if (IsOwner)
            Debug.Log($"Цвет Москвы изменился: {oldColor} -> {newColor}");
    }

    private IEnumerator AwardScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            playerdata.win_score++;
            Debug.Log("Очки начислены локально: " + playerdata.win_score);
            UpdateText();

            if (playerdata.win_score >= 5)
            {
                Debug.Log("[WinScoreManager] Игрок достиг 100 очков! Игра завершена.");

                // Завершение игры
                EndGameServerRpc(playerdata.current_team);

                yield break;
            }
        }
    }

    private void UpdateText()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("scoreText не назначен!");
            return;
        }

        if (playerdata == null)
        {
            Debug.LogWarning("playerdata не найден!");
            return;
        }

        scoreText.text = $"{playerdata.win_score}";
        Debug.Log("UI обновлён локально: " + playerdata.win_score);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void EndGameServerRpc(Colors winnerTeam)
    {
        EndGameClientRpc(winnerTeam);
    }

    [ClientRpc]
    private void EndGameClientRpc(Colors winnerTeam)
    {
        // Можно заблокировать UI карты и показать экран победы
        if (scoreText != null)
            scoreText.text = $"Команда {winnerTeam} победила!";

        mapBlocker.SetActive(true);
        winCanvas.SetActive(true);
        winnerText.text = dictsHolder.fullname_dict[winnerTeam];
        winnerText.color = dictsHolder.color_dict[winnerTeam];

        // Дополнительно можно заблокировать дальнейшие действия
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
            scoreCoroutine = null;
        }

        Debug.Log($"[WinScoreManager] Игра завершена для всех игроков. Победитель: {winnerTeam}");
    }
}
