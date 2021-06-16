using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public AudioSource musicaJuegoTerminado;
    public Image fondo;
    public Button botonExit;
    public Image ImagenBotonExit;
    public Musica cancionCreditos;

    void Awake()
    {
        botonExit.interactable = false;
        fondo.canvasRenderer.SetAlpha(0.0f);
        ImagenBotonExit.canvasRenderer.SetAlpha(0.0f);
       //cancionCreditos = GetComponent<Musica>();
        Invoke("mostarPantallaFinal", 80f);
    }

    public void mostarPantallaFinal()
    {
        cancionCreditos.apagarMusica();
        gameObject.SetActive(true);
        musicaJuegoTerminado.Play();
        fondo.CrossFadeAlpha(1, 0.5f, false);
        ImagenBotonExit.CrossFadeAlpha(1, 0.6f, false);
        botonExit.interactable = true;
        
    }

     public void PresionarBotonExit()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    
}
