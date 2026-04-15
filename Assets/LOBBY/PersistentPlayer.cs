using UnityEngine;

//  guarda los datos globales del jugador. Va en el prefab del player del lobby.s
public class PersistentPlayer : MonoBehaviour
{
    [Header("Datos Globales")]
    public int playerIndex; // 0, 1, 2
    public int teamIndex;   // 0 = Equipo A, 1 = Equipo B
    public int scoreGame; // Rondas ganadas hugo pancho

    private void Awake()
    {
        // si quitais esto jodes literalmente todo el juego, no lo borreis
        DontDestroyOnLoad(gameObject);
    }
}