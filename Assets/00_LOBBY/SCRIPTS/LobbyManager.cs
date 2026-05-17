using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


/// Gestiona la lobby con selección manual de slot mediante cursor libre.
/// Slots del 1 al 6. Equipos por columna: 1-2→eq0, 3-4→eq1, 5-6→eq2.

[RequireComponent(typeof(PlayerInputManager))]
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Configuración de partida")]
    [SerializeField] private int requiredPlayers = 6;

    [Header("Prefab del cursor (UI RectTransform)")]
    [SerializeField] private GameObject cursorPrefab;


    [Header("Transición escena")]
    public TransitionSettings transition;// podriasmos llmar varias y luego hace run random para que varien entre partidas
    [Header("Canvas raíz")]
    [SerializeField] private RectTransform lobbyCanvasRect;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI playersCountText;

    [Header("Segundos antes de cargar la escena")]
    [SerializeField] private float autoStartDelay = 3f;

    private PlayerInputManager inputManager;
    private readonly List<PlayerInput> joinedPlayers = new();
    private readonly Dictionary<int, LobbyCursor> cursors = new(); // playerIndex → cursor
    private readonly Dictionary<int, int> confirmedSlots = new(); // playerIndex → slotId (1-6)
    private Coroutine countdownCoroutine;

    PuntuacionManager puntuacionManager;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        inputManager.EnableJoining();
        if (countdownText) countdownText.gameObject.SetActive(false);
        RefreshCounterUI();
        puntuacionManager = FindObjectOfType<PuntuacionManager>();
    }



    public void HandlePlayerJoined(PlayerInput player)
    {
        if (joinedPlayers.Contains(player)) return;
        joinedPlayers.Add(player);

        int idx = player.playerIndex;

        if (cursorPrefab && lobbyCanvasRect)
        {
            GameObject cursorGO = Instantiate(cursorPrefab, lobbyCanvasRect);
            LobbyCursor cursor = cursorGO.GetComponent<LobbyCursor>();
            PlayerInputHandler handler = player.GetComponent<PlayerInputHandler>();

            if (cursor != null && handler != null)
            {
                cursor.Init(idx, handler, lobbyCanvasRect);
                cursors[idx] = cursor;
            }
        }

        Debug.Log($"[Lobby] Jugador {idx + 1} conectado.");
        RefreshCounterUI();
    }

    public void HandlePlayerLeft(PlayerInput player)
    {
        int idx = player.playerIndex;
        joinedPlayers.Remove(player);

        if (cursors.TryGetValue(idx, out LobbyCursor cursor))
        {
            Destroy(cursor.gameObject);
            cursors.Remove(idx);
        }

        if (confirmedSlots.TryGetValue(idx, out int slotId))
        {
            confirmedSlots.Remove(idx);
            foreach (var slot in FindObjectsOfType<LobbySlotUI>())
                if (slot.SlotId == slotId) slot.SetWaiting();
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            if (countdownText) countdownText.gameObject.SetActive(false);
            inputManager.EnableJoining();
        }

        RefreshCounterUI();
    }



    public void OnPlayerConfirmedSlot(int playerIndex, int slotId)
    {
        confirmedSlots[playerIndex] = slotId;

        PlayerInput pi = GetPlayerInput(playerIndex);
        if (pi != null)
        {
            PersistentPlayer pp = pi.GetComponent<PersistentPlayer>();
            if (pp != null)
            {
                pp.playerIndex = slotId;                    // ID permanente (1-6)
                pp.teamIndex = (slotId - 1) / 2;         // 1-2→0, 3-4→1, 5-6→2
                Debug.Log($"[Lobby] Jugador {playerIndex} → Slot {slotId} | Equipo {pp.teamIndex}");
            }
        }

        RefreshCounterUI();
        CheckAllConfirmed();
    }

    public void OnPlayerCancelledSlot(int playerIndex)
    {
        if (!confirmedSlots.ContainsKey(playerIndex)) return;
        confirmedSlots.Remove(playerIndex);
        RefreshCounterUI();

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            if (countdownText) countdownText.gameObject.SetActive(false);
            inputManager.EnableJoining();
        }
    }



    private void CheckAllConfirmed()
    {
        if (joinedPlayers.Count < requiredPlayers) return;
        if (confirmedSlots.Count < requiredPlayers) return;

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

        if (countdownText) countdownText.text = "¡Cargando mapa!";
        yield return null;

        if (puntuacionManager != null)
        {
            string scene = puntuacionManager.SiguienteEscena();
            if (!string.IsNullOrEmpty(scene))
                TransitionManager.Instance().Transition(scene, transition, 0);
            else
                Debug.LogError("[Lobby] Escena vacía en el array.");
        }
        else Debug.LogError("[Lobby] No hay escenas configuradas.");

        Destroy(gameObject);
    }



    private PlayerInput GetPlayerInput(int index)
    {
        foreach (var pi in joinedPlayers)
            if (pi.playerIndex == index) return pi;
        return null;
    }

    private void RefreshCounterUI()
    {
        if (playersCountText)
            playersCountText.text = $"{joinedPlayers.Count} conectados  |  {confirmedSlots.Count} / {requiredPlayers} listos";
    }
}