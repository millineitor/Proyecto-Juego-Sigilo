using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    //public static HUD Instancia;
    /*
    void Awake(){
        if (Instancia == null){
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }else Destroy(gameObject);
    } */

    public void ocultarHUD()
    {
        gameObject.SetActive(false);
    }

    public void mostrarHUD()
    {
        gameObject.SetActive(true);
    }
    
}
