using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PuntuationScene : MonoBehaviour
{
    private Animator animator;
    [SerializeField] GameObject pantalla;
    PuntuacionManager puntuacionManager;
    [SerializeField] GameObject[] estrellas;
    private Animator[] animadoresEstrellas;
    [SerializeField] GameObject[] panelesVictoria;
    [SerializeField] private GameObject palanca;
    private Animator animadorPalanca;
    int Gabador = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = pantalla.GetComponent<Animator>();
        animadoresEstrellas = new Animator[estrellas.Length];
        for (int i = 0; i < estrellas.Length; i++)
        {
            animadoresEstrellas[i] = estrellas[i].GetComponent<Animator>();
        }
        StartCoroutine(Empezar());
        puntuacionManager = FindObjectOfType<PuntuacionManager>();
        animadorPalanca = palanca.GetComponent<Animator>();
    }
    private IEnumerator Empezar()
    {
        animator.SetBool("Rodando", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Rodando", false);
        animator.SetBool("FinishRodando", true);
        int a = puntuacionManager.ObtenerPuntuacion(0);
        int b = puntuacionManager.ObtenerPuntuacion(1);
        int c = puntuacionManager.ObtenerPuntuacion(2);
        if (a == 1) 
            animadoresEstrellas[0].SetBool("Ap", true);
        if (a == 2)
        {
            animadoresEstrellas[1].SetBool("Ap", true);
            animadoresEstrellas[0].SetBool("Ap", true);
        }
        if (a == 3)
        {
            animadoresEstrellas[2].SetBool("Ap", true);
            animadoresEstrellas[1].SetBool("Ap", true);
            animadoresEstrellas[0].SetBool("Ap", true);
            Gabador = 1;
        }
        if (b == 1) animadoresEstrellas[3].SetBool("Ap", true);
        if (b == 2)
        {
            animadoresEstrellas[4].SetBool("Ap", true);
            animadoresEstrellas[3].SetBool("Ap", true);
        }
        if (b == 3)
        {
            animadoresEstrellas[5].SetBool("Ap", true);
            animadoresEstrellas[4].SetBool("Ap", true);
            animadoresEstrellas[3].SetBool("Ap", true);
            Gabador = 2;
        }
        if (c == 1) animadoresEstrellas[6].SetBool("Ap", true);
        if (c == 2)
        {
            animadoresEstrellas[7].SetBool("Ap", true);
            animadoresEstrellas[6].SetBool("Ap", true);
        }
        if (c == 3) {
            animadoresEstrellas[8].SetBool("Ap", true);
            animadoresEstrellas[7].SetBool("Ap", true);
            animadoresEstrellas[6].SetBool("Ap", true);
            Gabador = 3;
        }
        yield return new WaitForSeconds(5f);
        animadorPalanca.SetTrigger("Pressed");
        yield return new WaitForSeconds(0.3f);
        pantalla.GetComponent<Animator>().SetBool("FinishRodando", false);
        pantalla.GetComponent<Animator>().SetBool("Rodando", true);
        for (int i = 0; i < animadoresEstrellas.Length; i++)
        {
            animadoresEstrellas[i].SetBool("Ap", false);
        }
        yield return new WaitForSeconds(1f);
        if (Gabador == 0)
        {
            string next = puntuacionManager.SiguienteEscena();
            UnityEngine.SceneManagement.SceneManager.LoadScene(next);
            yield return null;
        }
        else
        {
            Ganador(Gabador);
        }
    }
    void Ganador(int index)
    {
        if (index == 1) panelesVictoria[0].SetActive(true);
        if (index == 2) panelesVictoria[1].SetActive(true);
        if (index == 3) panelesVictoria[2].SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
