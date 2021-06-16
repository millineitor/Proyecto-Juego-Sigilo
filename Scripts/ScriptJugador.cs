using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptJugador : MonoBehaviour
{
    /*
    Animator animacion;
    void Awake() => animacion = GetComponent<Animator>();
    */
    //JUGADOR SIGUIENTE NIVEL
   // public static ScriptJugador Instancia;// { get; private set; }
    //NIVEL
    public bool terminoNivel;
    //MOVIMIENTO
    public CharacterController control;
    public bool agachado;
    public bool pechoTierra;
    public Transform camara;
    public Collider colliderJugador;
    //GRAVEDAD
    public float gravedad = -9.81f;
    public Vector3 rapidez;
    public Transform groundCheck;
    public float distanciaSuelo;
    public LayerMask esTerreno;
    bool estaEnSuelo;
    //BARRA DE VIDA
    public Slider barraDeVida;
    //ANIMACION
    public Animator animador;
    public int estaCaminandoHash;
    public int estaAgachadoHash;
    public int estaPechoTierraHash;
    public int estaMuertoHash;
    private float alturaOriginalCollider = 3f;
    private Vector3 posicionOriginalCollider = new Vector3(0f, -0.3f, 0.3f);
    //MOVIMIENTO
    public float velocidad;
    public float tiempoVueltaSuave = 0.1f;
    private float velocidadVueltaSuave;
    //Prubea de Sonido RARA XD
    public AudioSource sonidoDeMuerte;
    //STATS
    public float vida;

    void Start()
    {
        //animador = GetComponent<Animator>();
        //DontDestroyOnLoad(gameObject);
        /*
        if (Instancia == null){
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        } */
        estaMuertoHash = Animator.StringToHash("estaMuerto");
        estaCaminandoHash = Animator.StringToHash("estaCaminando");
        estaPechoTierraHash = Animator.StringToHash("estaPechoTierra");
        estaAgachadoHash = Animator.StringToHash("estaAgachado");
        barraDeVida.value = vida;
        sonidoDeMuerte = GetComponent<AudioSource>();
        colliderJugador = GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {   
        estaEnSuelo = Physics.CheckSphere(groundCheck.position, distanciaSuelo, esTerreno);

        if (estaEnSuelo && rapidez.y < 0)
        {
            rapidez.y = -2f;
        }

        bool estaCaminando = animador.GetBool(estaCaminandoHash);
        bool estaAgachado = animador.GetBool(estaAgachadoHash);
        bool estaPechoTierra = animador.GetBool(estaPechoTierraHash);
        //Debug.Log(estaMuerto);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direccion = new Vector3(horizontal, 0f, vertical).normalized;

        if(direccion.magnitude >= 0.1f){
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + camara.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloObjetivo, ref velocidadVueltaSuave, tiempoVueltaSuave);
            transform.rotation = Quaternion.Euler(0f, anguloObjetivo, 0f);
            Vector3 direccionMovimiento = Quaternion.Euler(0f, anguloObjetivo, 0f) * Vector3.forward;
            control.Move(direccionMovimiento * velocidad * Time.deltaTime);
            animador.SetBool(estaCaminandoHash, true);
        }else{
            animador.SetBool(estaCaminandoHash, false);
        }

        rapidez.y += gravedad * Time.deltaTime;
        control.Move(rapidez * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            pechoTierra = false;
            agachado = !agachado;
            
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            agachado = false;
            pechoTierra = !pechoTierra;
           
        }

        if (agachado){
            Debug.Log("AGACHADO");
            velocidad = 5f;
            control.height = 2.5f;
            control.center = new Vector3(0f, -0.8f, 0.3f);
            animador.SetBool(estaAgachadoHash, true);
        }else{
            animador.SetBool(estaAgachadoHash, false);
        }

        if (pechoTierra){
            velocidad = 2.5f;
            control.height = 1.75f;
            control.center = new Vector3(0f, -1.2f, 0.3f);
            animador.SetBool(estaPechoTierraHash, true);
        }else{

            animador.SetBool(estaPechoTierraHash, false);
        }

        if (!pechoTierra && !agachado)
        {
            control.center = posicionOriginalCollider;
            velocidad = 10f;
            control.height = alturaOriginalCollider;
        }
    }
    
    public void recibirDanio(int danio)
    {
        bool estaMuerto = animador.GetBool(estaMuertoHash);
        barraDeVida.value -= danio;
        vida = vida - danio;
        if(vida <= 0)
        {
            animador.SetBool(estaMuertoHash, true);
           // Debug.Log(estaMuertoHash);
            sonidoDeMuerte.Play();
           // Debug.Log("Desactivando Jugador...");
            desactivarJugador();
            
        }
    }
    public void OnTriggerEnter(Collider other){
        Debug.Log(other.name);
        if(other.gameObject.tag == "finDelNivel"){
            terminoNivel = true;
            Debug.Log("XD");
        }
    }

    public void desactivarJugador(){
        colliderJugador.enabled = false;
        this.gameObject.GetComponent<ScriptJugador>().enabled = false;
        GameObject.Find("1911").GetComponent<SistemaArmas>().enabled = false;
        
    }
    
    public void reiniciarJugador(){
        colliderJugador.enabled = true;
        this.gameObject.GetComponent<ScriptJugador>().enabled = true;
        GameObject.Find("1911").GetComponent<SistemaArmas>().enabled = true;
    }

    public void reestablecerPosicionJugador(Vector3 posicion){
        transform.position = posicion;
    }

}