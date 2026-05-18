using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string escenaMinijuego = "Platformero";
    [SerializeField] private TextMeshProUGUI textoListo;
    [SerializeField] private float tiempoLimite = 30f; // esto es para que no se queden inf tiempo por si se buguea o algo si se pone 0 se desactiva esta función

    private PersistentPlayer[] jugadores;
    private HashSet<int> jugadoresListos = new HashSet<int>();
    private bool cargando = false;
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float startDelay;

    private void Start()
    {
        jugadores = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);
        ActualizarTexto();
    }

    private void Update()
    {
        if (cargando) return;

        foreach (PersistentPlayer pp in jugadores)
        {
            if (jugadoresListos.Contains(pp.playerIndex)) continue;

            PlayerInputHandler mando = pp.GetComponent<PlayerInputHandler>();
            if (mando == null) continue;

            if (mando.isJumping || mando.isGrabbing || mando.isSwitchingTool)
            {
                jugadoresListos.Add(pp.playerIndex);
                ActualizarTexto();
            }
        }

        if (jugadores.Length > 0 && jugadoresListos.Count >= jugadores.Length)
            StartCoroutine(Cargar());

        if (tiempoLimite > 0f)
        {
            tiempoLimite -= Time.deltaTime;
            if (tiempoLimite <= 0f) StartCoroutine(Cargar());
        }
    }

    private void ActualizarTexto()
    {
        if (textoListo != null)
            textoListo.text = $"Press     to continue ({jugadoresListos.Count}/{jugadores.Length}) Ready";
    }

    private IEnumerator Cargar()
    {
        cargando = true;
        yield return new WaitForSeconds(0.8f);
        TransitionManager.Instance().Transition(escenaMinijuego, transition, startDelay);
    }
}