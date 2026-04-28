using UnityEngine;

// Guarda los datos globales del jugador. Va en el prefab del player del lobby.
public class PersistentPlayer : MonoBehaviour
{
    [Header("Datos Globales")]
    public int playerIndex; // = slotId elegido en lobby (0-5) — asignado por LobbyManager
    public int teamIndex;   // 0 = Equipo A, 1 = Equipo B (para futuro)
    public int scoreGame;   // Rondas ganadas

    private void Awake()
    {
        // si quitais esto jodes literalmente todo el juego, no lo borreis
        DontDestroyOnLoad(gameObject);
    }
}