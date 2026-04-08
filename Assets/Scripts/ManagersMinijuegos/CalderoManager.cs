using UnityEngine;

public class CalderoManager : MonoBehaviour
{
    //Esto no es seguro que lo hagamos asi 
    [Header("Prefabs Sprites")]
    public GameObject prefabCesta; // abajo
    public GameObject prefabMano;  // El cursor de arriba

    [Header("Spawn Points")]
    public Transform[] spawnPointsCestas; // Dónde aparecen los P1 (abajo)
    public Transform[] spawnPointsManos;  // Dónde aparecen los P2 (arriba)

    private void Start()
    {
        //  Buscamos los players (busca todos los objetos con el script persistant por eso otro script para el lobbymanager)
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        //Esto se puede quitar ocasionalmente para debug pero se puede liar
        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[CalderoManager] No hay jugadores. ¿Has pasado por el lobby ?");
            return;
        }

        // Creamos nuestro propio contador para ignorar los IDs que asigne Unity por que se raya y se salta numeros por que le sale de los huevos, si lo haceis con el id de unity tendreis 2 cestas en este caso en vez de 1 d ecada
        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            // Conseguimos player input de cada uno
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            // Lógica de parejas para sacar el p1 o p2 de cada pàreja y asiganr spawns
            if (contadorJugadores % 2 == 0)
            {
                SpawnearCesta(contadorJugadores, lectorBotones);
            }
            else
            {
                SpawnearMano(contadorJugadores, lectorBotones);
            }

            // Sumamos 1 al contador para que el siguiente jugador sea impar/par correctamente
            contadorJugadores++;
        }
    }

    private void SpawnearCesta(int idJugador, PlayerInputHandler mando)
    {
        // Calcular spawn point  (ID 0 = Spawn 0 -- ID 2 = Spawn 1--ID 4 = Spawn 2)
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsCestas[indexSpawn];

        // Instanciamos la Cesta
        GameObject sprite = Instantiate(prefabCesta, puntoSpawn.position, Quaternion.identity);

        // Le "enchufamos" el mando
        if (sprite.TryGetComponent<SpriteCesta>(out var scriptCesta))
        {
            scriptCesta.ConectarMando(mando);
        }
    }

    private void SpawnearMano(int idJugador, PlayerInputHandler mando)
    {
        // Calculamos qué spawn point (ID 1 = Spawn 0-- ID 3 = Spawn 1 -- ID 5 = Spawn 2)
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsManos[indexSpawn];

        // Instanciamos  la mano de la wii
        GameObject avatar = Instantiate(prefabMano, puntoSpawn.position, Quaternion.identity);


        if (avatar.TryGetComponent<SpriteMano>(out var scriptMano))
        {
            scriptMano.ConectarMando(mando);
        }
    }
}