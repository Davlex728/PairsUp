using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInputManager))]
public class LobbyManager : MonoBehaviour
{
    [Header("Configuración de partida")]
    [SerializeField] private int requiredPlayers = 6;
    [SerializeField] private string nextSceneName = "MainGame";

    [Header("Referencias UI")]
    [SerializeField] private LobbySlotUI[] playerSlots;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI playersCountText;

    [Header("Segundos antes de cargar la escena")]
    [SerializeField] private float autoStartDelay = 3f;

    private PlayerInputManager inputManager;
    private readonly List<PlayerInput> joinedPlayers = new();
    private Coroutine countdownCoroutine;

    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        inputManager.EnableJoining();

        DontDestroyOnLoad(gameObject);

        if (countdownText) countdownText.gameObject.SetActive(false);
        RefreshCounterUI();
    }

    // Conectar desde Inspector → PlayerInputManager → On Player Joined
    public void HandlePlayerJoined(PlayerInput player)
    {
        Debug.Log($"[Lobby] Joined. Index: {player.playerIndex}");

        if (!joinedPlayers.Contains(player))
            joinedPlayers.Add(player);

        int idx = player.playerIndex;
        if (idx < playerSlots.Length)
            playerSlots[idx].SetReady(idx);

        RefreshCounterUI();
        CheckAllReady();
    }

    // Conectar desde Inspector → PlayerInputManager → On Player Left
    public void HandlePlayerLeft(PlayerInput player)
    {
        Debug.LogError($"[Lobby] Left. Index: {player.playerIndex}\n{System.Environment.StackTrace}");

        joinedPlayers.Remove(player);

        int idx = player.playerIndex;
        if (idx < playerSlots.Length)
            playerSlots[idx].SetWaiting();

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            if (countdownText) countdownText.gameObject.SetActive(false);
            inputManager.EnableJoining();
        }

        RefreshCounterUI();
    }

    private void CheckAllReady()
    {
        if (joinedPlayers.Count < requiredPlayers) return;

        inputManager.DisableJoining();
        countdownCoroutine = StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        if (countdownText) countdownText.gameObject.SetActive(true);

        float remaining = autoStartDelay;
        while (remaining > 0f)
        {
            if (countdownText)
                countdownText.text = $"Comenzando en {Mathf.CeilToInt(remaining)}...";
            yield return null;
            remaining -= Time.deltaTime;
        }

        Destroy(gameObject);
        SceneManager.LoadScene(nextSceneName);
    }

    private void RefreshCounterUI()
    {
        if (playersCountText)
            playersCountText.text = $"{joinedPlayers.Count} / {requiredPlayers} jugadores";
    }
}