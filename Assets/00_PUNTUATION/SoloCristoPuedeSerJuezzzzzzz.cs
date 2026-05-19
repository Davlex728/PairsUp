using UnityEngine;
using UnityEngine.SceneManagement;

public class SoloCristoPuedeSerJuezzzzzzz : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Reset()
    {
        SceneManager.LoadScene("StartScene");
        PuntuacionManager puntuacionManager = FindObjectOfType<PuntuacionManager>();
        puntuacionManager.ResetPuntuacion();
    }
}
