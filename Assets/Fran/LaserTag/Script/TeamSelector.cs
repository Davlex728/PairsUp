using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TeamSelector : MonoBehaviour
{
    private List<List<Gamepad>> equipos = new List<List<Gamepad>>();
    
    private int maxEquipos = 2;

    private bool todosCompletos;

    private void OnEnable()
    {
        InputSystem.onEvent += DetectarPulsacion;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= DetectarPulsacion;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            equipos.Add(new List<Gamepad>());
        }
    }

    void DetectarPulsacion(InputEventPtr eventPtr, InputDevice device)
    {
        
        if(device is Gamepad gamepad)
        {
            //gamepad.buttonSouth es para saber si se ha pulsado el botón A (en Xbox) o X (en PlayStation) y gamepad.buttonEast para el botón B (en Xbox) o Círculo (en PlayStation)
            if (gamepad.buttonSouth.wasPressedThisFrame /*|| gamepad.buttonWest.wasPressedThisFrame*/)
            {
                Debug.Log($"[TeamSelector] Gamepad '{gamepad.displayName}' pulsó A o X.");
                
                foreach (List<Gamepad> equipo in equipos)
                {
                    if (equipo.Contains(gamepad))
                    {
                        return;
                    }
                }
                
                foreach (List<Gamepad> equipo in equipos)
                {
                    if (equipo.Count < maxEquipos )
                    {
                       
                        equipo.Add(gamepad);
                        Debug.Log($"Mando añadido al equipo");
        
                       
                        break;
                    }
                }

                foreach (List<Gamepad> equipo in equipos)
                {
                    if (equipo.Count < maxEquipos)
                    {
                        todosCompletos = false;
                        break;
                    }
                }

                if (todosCompletos)
                {
                    
                }
              
            }
        }

        
        
    }
}
