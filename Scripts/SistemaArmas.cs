using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SistemaArmas : MonoBehaviour
{
    //Stats del Arma
    public int danio;
    public float velocidadDeDisparo, propagacion, rango, tiempoDeRecarga, tiempoEntreBalas;
    public int tamanioCargador, balasPorClic;
    public bool armaAutomatica;
    int balasRestantes, balasDisparadas;

    //LASER
    public LineRenderer miraLaser;

    //Mas weas booleanas
    bool disparando, listoParaDisparar, recargando;

    //Referencia
    //public Camera fpsCam;
    public Transform arma;
    public Transform puntoDeAtaque;
    //Intento de Casquillo
    public Transform salidaCasquillo;
    public RaycastHit rayHit;
    public LayerMask esEnemigo;

    //Graficas
    //Agitar la camara cuando disparas
    //public AgitarCamara agitarCamara;
    //public float magnitudAgitarCamara, duracionAgitarCamara;
    //texto pa las balas
    public Text texto;
    //efectos de disparo uwu
    public GameObject flashDisparo, agujeroDeBala, casquilloBala;
    public AudioSource plomazo;
    private GameObject referenciaFlashDisparo, referenciaAgujeroBala, referenciaCasquilloBala;

    // ver las balas en texto

    private void Awake()
    {
        balasRestantes = tamanioCargador;
        listoParaDisparar = true;
        plomazo = GetComponent<AudioSource>();
    }

    private void Update()
    {
        MisInputs();
        mostrarMiraLaser();
        //Colocamos el texto de las balas
        //texto.SetText(balasRestantes + " / " + tamanioCargador);
        texto.text = balasRestantes + "/" + tamanioCargador;
    }


    void MisInputs()
    {
        if (armaAutomatica) disparando = Input.GetKey(KeyCode.Mouse0);
        else disparando = Input.GetKeyDown(KeyCode.Mouse0);
        //Recargar 
        if(Input.GetKeyDown(KeyCode.R) && balasRestantes < tamanioCargador && !recargando) recargar();

        //Disparar
        if(listoParaDisparar && disparando && !recargando && balasRestantes > 0)
        {
            balasDisparadas = balasPorClic;
            disparar();
        }
    }

    private void disparar()
    {
        listoParaDisparar = false;

        //Recoil Disparar
        float x = Random.Range(-propagacion, propagacion);
        float y = Random.Range(-propagacion, propagacion);

        //Calculamos la dirección de la propagación
        Vector3 direccion = arma.forward + new Vector3(x,y,0);

        /* Aumento de Recoil cuando se esta moviendo
        if(rigid.velocity.magnitude > 0)
        {
            propagacion = propagacion * 1.5f;

        }else propagacion;
        */

        //Proceso: RAYCAST
        //public static bool Raycast(Vector3 origen, Vector3 direccion, 
        //float distancia maxima, int layerMask = DefaultRaycastLayers, 
        //QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);
        if (Physics.Raycast(arma.position, direccion, out rayHit, rango, esEnemigo))
        {
            //Debug.Log(rayHit.collider.name);
            //dañar a los enemigos

            
            if(rayHit.collider.CompareTag("Enemigo"))
            {
                rayHit.collider.GetComponent<IA_Enemigos>().recibirDanio(danio);
            }
            
        }
        // Llamar al metodo que aigta la camara:
        //agitarCamara.Agitar(duracionAgitarCamara, magnitudAgitarCamara);

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

        //Intento de destrucción de casquillos: 

        referenciaCasquilloBala = Instantiate(casquilloBala, salidaCasquillo.position, Quaternion.identity);//.GetComponent<Rigidbody>();
        //referenciaCasquilloBala.AddForce(transform.forward * -0.5f, ForceMode.Impulse);
        //referenciaCasquilloBala.AddForce(transform.up * 0.5f, ForceMode.Impulse);

        Destroy(referenciaCasquilloBala, 20f);
        
        balasRestantes = balasRestantes - 1;
        balasDisparadas = balasDisparadas -1;
        Invoke("reestablecerDisparo", velocidadDeDisparo);

        //Multiples balas (Escopeta) o Rafaga
        if(balasDisparadas > 0 && balasRestantes > 0)
        Invoke("disparar", tiempoEntreBalas);

    }

    public void mostrarMiraLaser()
    {
        RaycastHit laserInfo;
        if(Physics.Raycast(puntoDeAtaque.position, puntoDeAtaque.forward, out laserInfo, rango)){
            if(laserInfo.collider){
                    miraLaser.SetPosition(1, new Vector3(0, 0, laserInfo.distance));
            }
        }else{
                miraLaser.SetPosition(1, new Vector3(0, 0, rango));
        }
    }
    private void reestablecerDisparo()
    {
        listoParaDisparar = true;


    }

    private void recargar()
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