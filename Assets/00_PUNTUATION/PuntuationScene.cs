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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = pantalla.GetComponent<Animator>();
        StartCoroutine(Empezar());
        puntuacionManager = FindObjectOfType<PuntuacionManager>();
        for (int i = 0; i < estrellas.Length; i++)
        {
            animadoresEstrellas[i] = estrellas[i].GetComponent<Animator>();
        }
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
        if (a == 1) animadoresEstrellas[0].SetBool("Ap", true);
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
            panelesVictoria[0].SetActive(true);
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
            panelesVictoria[1].SetActive(true);
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
            panelesVictoria[2].SetActive(true);
        }

        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
