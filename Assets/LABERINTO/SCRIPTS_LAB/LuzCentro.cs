using UnityEngine;

public class LuzCentro : MonoBehaviour
{
    public Material darknessMaterial;
    public Transform player;

    void Update()
    {
        Vector4 posicion = new Vector4(player.position.x, player.position.y, 0, 0);
        darknessMaterial.SetVector("_PlayerPosition", posicion);
    }
}
