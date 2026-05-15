using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Camera cameraStartScene;
    [SerializeField] private float smoothVelocityCamera = 0.01f;
    private Animator animator;
    [SerializeField] private GameObject pantallaAnimada;
    [SerializeField] private Animator azul;
    [SerializeField] private Animator rojo;
    [SerializeField] private Animator amarillo;
    [SerializeField] private GameObject[] LobbyMangers;
    bool primerpulsado = false;
    void Start()
    {
        animator = pantallaAnimada.GetComponent<Animator>();
    }
    public void StartButtonClicked()
    {
        StartCoroutine(OnStartButtonClicked());
    }
    public IEnumerator OnStartButtonClicked()
    {
        // Load the main game scene
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        //smoothly move the camera to the new position
        for (float t = 0; t < 1; t += Time.deltaTime * smoothVelocityCamera)
        {
            cameraStartScene.transform.position = Vector3.Lerp(cameraStartScene.transform.position, new Vector3(0, -10.04f, 0), t);
            yield return null;
        }

    }
    public void ExitButtonClicked()
    {
        Application.Quit();
    }

    public void BackButtonClicked()
    {
        StartCoroutine(OnBackButtonClicked());

    }

    private IEnumerator OnBackButtonClicked()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        //smoothly move the camera to the new position
        for (int i = 0; i < LobbyMangers.Length; i++)
        {
            LobbyMangers[i].SetActive(false);
        }
        for (float t = 0; t < 1; t += Time.deltaTime * smoothVelocityCamera)
        {
            cameraStartScene.transform.position = Vector3.Lerp(cameraStartScene.transform.position, new Vector3(0, 0, 0), t);
            yield return null;
        }
        primerpulsado = false;
        azul.SetBool("Ap", false);
        rojo.SetBool("Ap", false);
        amarillo.SetBool("Ap", false);
        animator.SetBool("FinishRodando", false);
    }

    public void Debugeo()
    {
        Debug.Log("Button clicked!");
    }
    // Update is called once per frame
    public void AnimacionPantalla()
    {
        StartCoroutine(AnimacionPantallaCoroutine());
    }

    private IEnumerator AnimacionPantallaCoroutine()
    {
        if (primerpulsado) yield break; // Evita que se ejecute nuevamente si ya se ha pulsado antes
        animator.SetBool("Rodando", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Rodando", false);
        animator.SetBool("FinishRodando", true);
        azul.SetBool("Ap", true);
        rojo.SetBool("Ap", true);
        amarillo.SetBool("Ap", true);
        primerpulsado = true;
        for (int i = 0; i < LobbyMangers.Length; i++)
        {
            LobbyMangers[i].SetActive(true);
        }
    }
    void Update()
    {
        
    }
}
