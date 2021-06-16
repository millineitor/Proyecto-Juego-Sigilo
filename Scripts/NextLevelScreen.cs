using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLevelScreen : MonoBehaviour
{
    //public static NextLevelScreen Instancia { get; private set; }
    public AudioSource cancionNivelCompletado;
    public Image fondo;
    public Image logoStageClear;
    public Button botonContinuar;
    public Image imagenBotonContinuar;

    void Awake()
    {
        /*
        if (Instancia == null){
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        } */
        ocultarPantallaNivelTerminado();
        cancionNivelCompletado = GetComponent<AudioSource>();
    }

    public void mostrarPantallaNivelTerminado()
    {
        //gameObject.SetActive(true);
        gameObject.GetComponent<GraphicRaycaster>().enabled = true;
        fondo.CrossFadeAlpha(1, 0.4f, false);
        cancionNivelCompletado.Play();
        logoStageClear.CrossFadeAlpha(1, 2f, false);
        botonContinuar.interactable = true;
        imagenBotonContinuar.CrossFadeAlpha(1, 0.8f, false);
        
    }

    public void ocultarPantallaNivelTerminado()
    {
        gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        fondo.canvasRenderer.SetAlpha(0.0f);
        logoStageClear.canvasRenderer.SetAlpha(0.0f);
        imagenBotonContinuar.canvasRenderer.SetAlpha(0.0f);
        botonContinuar.interactable = false;
        //gameObject.SetActive(false);
    }


    public void botonSiguienteNivel()
    {
        //CODIGO
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
         ocultarPantallaNivelTerminado();
    }


}
