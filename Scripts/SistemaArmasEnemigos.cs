using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaArmasEnemigos : MonoBehaviour
{
    // Start is called before the first frame update XDXD
    public int danio;
    public float velocidadDeDisparo, propagacion, rango, tiempoDeRecarga, tiempoEntreBalas;
    public int tamanioCargador, balasPorAtaque;
    //public bool armaAutomatica;
    int balasRestantes, balasDisparadas;

    //Mas weas booleanas
    bool disparando, listoParaDisparar, recargando;

    //Referencia
    //public Camera fpsCam;
    public Transform enemigo;
    public Transform puntoDeAtaque;
    private Vector3 PosicionJugador;
    //Intento de Casquillo
    //public Transform salidaCasquillo;
    public RaycastHit rayHit;
    public LayerMask esJugador;

    //Graficas


    //efectos de disparo uwu
    public GameObject flashDisparo, agujeroDeBala; //casquilloBala;
    public AudioSource plomazo;
    private GameObject referenciaFlashDisparo, referenciaAgujeroBala;// referenciaCasquilloBala;

    // ver las balas en texto

    private void Awake()
    {
        balasDisparadas = balasPorAtaque;
        balasRestantes = tamanioCargador;
        listoParaDisparar = true;
        plomazo = GetComponent<AudioSource>();
        recargando = false;
    }
    /*
    private void Update()
    {
        MisInputs();
    }
    */
    
    /*
    void MisInputs()
    {
        if (armaAutomatica) disparando = Input.GetKey(KeyCode.Mouse0);
        else disparando = Input.GetKeyDown(KeyCode.Mouse0);
        //Recargar 
        if(Input.GetKeyDown(KeyCode.R) && balasRestantes < tamanioCargador && !recargando) recargar();

        //Disparar
        if(listoParaDisparar && disparando && !recargando && balasRestantes > 0)
        {
            balasDisparadas = balasPorAtaque;
            disparar();
        }
    }
    */
    public void condicionesDisparo(Vector3 RefPosicionJugador)
    {
        PosicionJugador = RefPosicionJugador;
        if(listoParaDisparar && !recargando && balasRestantes > 0)
        {
            balasDisparadas = balasPorAtaque;
            disparar();
        }
        if(balasRestantes <= 0)
        {
            if(balasRestantes == tamanioCargador)
            {
                return;
            }
            recargarArmaEnemiga();
        } 
    }

    private void disparar()
    {


        listoParaDisparar = false;

        //Recoil Disparar
        float x = Random.Range(-propagacion, propagacion);
        float y = Random.Range(-propagacion, propagacion);

        //Calculamos la dirección de la propagación
        //Vector3 direccion = enemigo.forward + new Vector3(x,y -0.55f,0f);
        Vector3 direccion = PosicionJugador - puntoDeAtaque.position;
        //Vector3 direccion = puntoDeAtaque.position - (jugador.position);// + new Vector3(x,y -0.5f,0f); //+ new Vector3(x,y -0.5f,0f);

        //Proceso: RAYCAST
        //public static bool Raycast(Vector3 origen, Vector3 direccion, 
        //float distancia maxima, int layerMask = DefaultRaycastLayers, 
        //QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);

        if (Physics.Raycast(puntoDeAtaque.position, direccion, out rayHit, rango, esJugador))
        {
            //Debug.Log(rayHit.collider.name);
            //dañar a los enemigos
            if(rayHit.collider.CompareTag("Jugador"))
            {
                rayHit.collider.GetComponent<ScriptJugador>().recibirDanio(danio);
            }
            
        }
        //Graficas: Para poner los efectos uwu:
        referenciaFlashDisparo = Instantiate(flashDisparo, puntoDeAtaque.position, Quaternion.identity);
       
        plomazo.Play();
        referenciaAgujeroBala = Instantiate(agujeroDeBala, rayHit.point, Quaternion.Euler(0,180,0));
        
        //Instantiate(referenciaFlashDisparo);
        //Instantiate(referenciaAgujeroBala);
        Destroy(referenciaAgujeroBala, 10f);
        Destroy(referenciaFlashDisparo, 0.1f);
        //INtento de Casiquillo
        //Instantiate(casquilloBala, salidaCasquillo.position, Quaternion.identity);

        //Intento de destrucción de casquillos:  XD

        //referenciaCasquilloBala = Instantiate(casquilloBala, salidaCasquillo.position, Quaternion.identity);//.GetComponent<Rigidbody>();

        //Destroy(referenciaCasquilloBala, 20f);
        
        balasRestantes = balasRestantes - 1;
        //Debug.Log("Balas en el cargador: " + balasRestantes);

        balasDisparadas = balasDisparadas -1;
        Invoke("reestablecerDisparo", velocidadDeDisparo);

        //Multiples balas (Escopeta) o Rafaga
        if(balasDisparadas > 0 && balasRestantes > 0)
        Invoke("disparar", tiempoEntreBalas*Time.deltaTime);
        
    }

    private void reestablecerDisparo()
    {
        //Debug.Log("REstableciendo Diparo");
        listoParaDisparar = true;
    }


    private void recargarArmaEnemiga()
    {
        recargando = true;
        Invoke("recargaTerminada", tiempoDeRecarga);

    }

    private void recargaTerminada()
    {
        balasRestantes = tamanioCargador;
        recargando = false;
    }
}

