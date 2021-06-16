using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    //public static GameOverScreen Instancia;// { get; private set; }
    public AudioSource musicaGameOver;
    public Image fondo;
    public Image logoSnakeIsDead;
    public Image marco1;
    public Image marco2;
    public Image ImagenBotonContinue;
    public Button botonContinue;
    public Image ImagenBotonExit;
    public Button botonExit;


    void Awake()
    {
        /*
        if (Instancia == null){
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        } */
        ocultarPantallaGameOver();
        musicaGameOver = GetComponent<AudioSource>();
    }
    public void mostrarPantallaGameOver()
    {
        //gameObject.SetActive(true);
        //Debug.Log("Activando pantalla Game Over");
        fondo.CrossFadeAlpha(1, 0.5f, false);
        logoSnakeIsDead.CrossFadeAlpha(1, 5f, false);
        marco1.CrossFadeAlpha(1, 1f, false);
        marco2.CrossFadeAlpha(1, 1f, false);
        musicaGameOver.Play();
        StartCoroutine(encenderBotonesGameOver());
    }

    public void ocultarPantallaGameOver()
    {
        fondo.canvasRenderer.SetAlpha(0.0f);
        logoSnakeIsDead.canvasRenderer.SetAlpha(0.0f);
        //logoSnakeIsDead.canvasRenderer.enabled = false;
        marco1.canvasRenderer.SetAlpha(0.0f);
        marco2.canvasRenderer.SetAlpha(0.0f);
        ImagenBotonContinue.canvasRenderer.SetAlpha(0f);
        botonContinue.interactable = false;
        //botonContinue.enabled = false;
        ImagenBotonExit.canvasRenderer.SetAlpha(0f);
        botonExit.interactable = false;
        //botonExit.enabled = false;
        musicaGameOver.Stop();
        //Debug.Log("Desactivando pantalla Game Over");
        //gameObject.SetActive(false);
    }

    public void botonContinuar()
    {
        ocultarPantallaGameOver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameManager.Nullify();
        
    }

    
    IEnumerator encenderBotonesGameOver()
    {
        yield return new WaitForSeconds(5);
        ImagenBotonContinue.CrossFadeAlpha(1, 0.7f, false);
        ImagenBotonExit.CrossFadeAlpha(1, 0.7f, false);
        botonContinue.interactable = true;
        botonExit.interactable = true;
    }

    public void botonSalir()
    {
        ocultarPantallaGameOver();
        SceneManager.LoadScene("MenuPrincipal");
    }
}
