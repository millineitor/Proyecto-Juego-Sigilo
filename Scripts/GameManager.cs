using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public static GameManager Instancia { get; private set; }
    //public Transform principioNivel;
    public ScriptJugador Jugador;
    public GameOverScreen pantallaGameOver;// 
    public HUD HUD;
    public Musica musica;
    //public EndScreen pantallaFinal;
    public NextLevelScreen pantallaNivelCompletado;

    public bool JugadorMuerto, juegoTerminado, reiniciandoJuego;
    void Awake()
    {
        //Debug.Log(Instancia);
       /*
        if (Instancia == null){
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        } */
        //Debug.Log(Instancia);
        //Debug.Log(GameObject.Find("PantallaGameOver").GetComponent<GameOverScreen>());
        Jugador = GameObject.Find("Jugador3raPersona").GetComponent<ScriptJugador>();
        HUD = GameObject.Find("HUD").GetComponent<HUD>();
        musica = GameObject.Find("Musica").GetComponent<Musica>();
        pantallaGameOver = GameObject.Find("PantallaGameOver").GetComponent<GameOverScreen>();
        pantallaNivelCompletado = GameObject.Find("PantallaNivelCompletado").GetComponent<NextLevelScreen>();
        //principioNivel = GameObject.Find("principioNivel").GetComponent<Transform>();
        reiniciandoJuego = false;
        juegoTerminado = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Jugador.vida <= 0 && !reiniciandoJuego){
            juegoTerminado = true;
            GameOver();
        }
        if (Jugador.terminoNivel && !reiniciandoJuego)
        {
            finalDelNivel();
        }
    }

    public void GameOver()
    {
        musica.apagarMusica();
        HUD.ocultarHUD();
        pantallaGameOver.mostrarPantallaGameOver();
        StartCoroutine(reiniciarJuego());

    }

    public void finalDelNivel()
    {
        Jugador.desactivarJugador();
        musica.apagarMusica();
        //Time.timeScale = 0;
        reiniciandoJuego = true;
        //pantallaFinal.mostarPantallaFinal();
        HUD.ocultarHUD();
        pantallaNivelCompletado.mostrarPantallaNivelTerminado();
    }
    /*
    public static void Nullify(){
        Instancia = null;
        Instancia.pantallaGameOver = null;
        Instancia.pantallaNivelCompletado = null;
    } */

    IEnumerator reiniciarJuego()
    {
        reiniciandoJuego = true;
        yield return null;
    }
}
