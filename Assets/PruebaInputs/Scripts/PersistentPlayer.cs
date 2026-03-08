using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    private void Awake()
    {
        // Esto le dice a Unity: "Cuando cambies de escena, NO destruyas este objeto"
        DontDestroyOnLoad(gameObject);
    }
}
