using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    public AudioSource musicaMenu;
     void Awake()
    {
        musicaMenu = GetComponent<AudioSource>();
    }


    public void botonStart()
    {
        //SceneManager.LoadScene("Nivel01");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void botonExit()
    {
        Application.Quit();
        Debug.Log("Cerrando Juego...");
    }
}
