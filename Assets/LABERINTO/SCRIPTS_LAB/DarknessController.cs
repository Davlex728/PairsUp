using UnityEngine;

public class DarknessController : MonoBehaviour
{
    public Material darknessMaterial;
    public MazeGameManager mazeGameManager;

    void Update()
    {
        Vector4[] posiciones = new Vector4[6];

        for (int i = 0; i < mazeGameManager.jugadoresVivos.Count; i++)
        {
            if (mazeGameManager.jugadoresVivos[i] != null)
            {
                Vector3 pos = mazeGameManager.jugadoresVivos[i].transform.position;
                posiciones[i] = new Vector4(pos.x, pos.y, 0, 0);
            }
        }

        darknessMaterial.SetVectorArray("_PlayerPositions", posiciones);
    }
}