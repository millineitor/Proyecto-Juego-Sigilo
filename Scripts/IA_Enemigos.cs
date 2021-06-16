using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System; //Libreria estandar de unity, para usar ConvertToInt32
using Random=UnityEngine.Random; //Existen 2 clases 'Random' en System y UnityEngine
using Object=UnityEngine.Object; //Existen 2 clases 'Object' en System y UnityEngine
//Establecermos la clase Random que sea la que implementa UnityEngine
public class IA_Enemigos : MonoBehaviour
{   
    //Componente de NavMesh
    public NavMeshAgent agente;
    //Vector del Jugador
    public Transform jugador; 
    //Layers para la NavMesh
    public LayerMask esTerreno, esJugador;
    //Atributo de Vida para el enemigo
    public float vida;
    public int estadoActual;

    //-----------BUSQUEDA------------------
    //Una posicion para un punto de busqueda
    public Vector3 puntoPatrulla;
    bool puntoPatrullaFijado;
    public float rangoPuntoPatrulla;
    //public Transform puntoDeBusqueda;

    //-----------PATRULLA -----------------
    public Transform[] puntosDePatrulla;
    private int puntoDestino = 0;
    //-----------ATACAR--------------------
    bool yaAtaco;
    
    //public RaycastHit lineaDeVision; //rayHit;
    public RaycastHit infoVision;
    public SistemaArmasEnemigos armaEnemiga; //Arma enemiga de la clase Arma

    //ANIMACIONES
    public Animator animador;
    public int estaAtacandoHash;
    public int estaMuriendoHash;
    public int estaSiguiendoHash;
    public Quaternion rotacionActual;


    //CONDICIONES
    public float rangoDeVision, rangoDeAtaque; //Rangos de Visión y Ataque
    public bool jugadorEnVisionEnemiga, jugadorEnRangoAtaque, jugadorEnLineaDeVision,jugadorEnConoDeVision; //jugadorAvistado; // Detectores
    public float anguloDeVision;
    //CONDICIONES PARA BUSQUEDA:
    public bool busquedaEnCurso, localizacionCompletada;
    static public bool jugadorAvistado;
    static public Vector3 ultimaPosicionConocidaJugador;
    public bool rayoVision;

    private void Awake()
    {
        //jugadorAvistado = RadioEnemigo.Instancia.jugadorAvistado;
        //
        estaAtacandoHash = Animator.StringToHash("estaAtacando");
        estaMuriendoHash = Animator.StringToHash("estaMuriendo");
        estaSiguiendoHash = Animator.StringToHash("estaSiguiendo");
        animador = GetComponent<Animator>();
        agente.autoBraking = false;
        localizacionCompletada = false;
        ultimaPosicionConocidaJugador = new Vector3(0f, 0f, 0f);//jugador.position;
        busquedaEnCurso = false;
        jugadorAvistado = false;
        jugador = GameObject.Find("Jugador3raPersona").transform;
        agente = GetComponent<NavMeshAgent>();
        siguientePuntoPatrulla();
    }

    private void Update()
    {
        
        bool estaAtacando = animador.GetBool(estaAtacandoHash);
        bool estaMuriendo = animador.GetBool(estaMuriendoHash);
        bool estaSiguiendo = animador.GetBool(estaSiguiendoHash);
        //Debug.Log(agente.name + " Jugador Avistado: " + jugadorAvistado);
        Vector3 direccionDelJugador = jugador.position - transform.position; //La dirección del jugador dada en un Vector3
        direccionDelJugador.Normalize();
        //float anguloHaciaElJugador = Vector3.Dot(transform.forward, direccionDelJugador);
        float anguloHaciaElJugador = (Vector3.Angle(direccionDelJugador, transform.forward)); // El angulo entre el vector del jugador y el enemigo
        //----------------------------CONDICIONES -----------------------------------

        //CONO DE VISION
        //Checamos que el jugador este dentro de un cono de 70 grados
        // > 0.707f
        /*
        if(anguloHaciaElJugador > anguloDeVision )
        {
            jugadorEnConoDeVision = true;
        }else jugadorEnConoDeVision = false;
        */
        
        if(anguloHaciaElJugador >= -anguloDeVision && anguloHaciaElJugador <= anguloDeVision)
        {
            jugadorEnConoDeVision = true;
        }else jugadorEnConoDeVision = false;
        

        //Casteamos un rayo que al final o al colisionar, castea una esfera que da informacion sobre colisiones
        // Un rayo que: Sale desde el enemigo, y se dirige hacia el jugador, que tiene un rango = rango de vision, con radio pequeño
        rayoVision = Physics.SphereCast(transform.position, transform.lossyScale.x/2, direccionDelJugador, out infoVision, rangoDeVision);

        //LINEA DE VISION
        //Checamos el collider de la esfera, si esta impacta al jugador, entonces tiene linea de visión.
        //Debug.Log(infoVision.collider);
        if(infoVision.collider != null)
        {
            if(infoVision.collider.CompareTag("Jugador"))
            {
                jugadorEnLineaDeVision = true;
            }else jugadorEnLineaDeVision = false;
        }else jugadorEnLineaDeVision = false;


        //VISION
        //Si el jugador esta en el cono y en línea de visión, entonces esta en rango de visión.
        if ((jugadorEnConoDeVision && jugadorEnLineaDeVision) || Physics.CheckSphere(transform.position, 2f, esJugador))
        {
            jugadorEnVisionEnemiga = true;
        }else jugadorEnVisionEnemiga = false;

        //Checamos si el jugador esta en rango de ataque
        jugadorEnRangoAtaque = (Physics.CheckSphere(transform.position,rangoDeAtaque, esJugador));
        //Debug.Log("VALOR DE VISION ENEMIGA: " + Convert.ToInt32(jugadorEnVisionEnemiga)*4);
        //Debug.Log("VALOR DE RANGO DE ATAQUE:" + Convert.ToInt32(jugadorEnRangoAtaque)*2);
        estadoActual = (Convert.ToInt32(jugadorEnVisionEnemiga)*4 + Convert.ToInt32(jugadorEnRangoAtaque)*2 + Convert.ToInt32(jugadorAvistado));
        //Debug.Log(agente.name + " -> " + estadoActual);
        //Debug.Log(estadoActual);
        /*----------------------- MAQUINA DE ESTADOS----------------------------------
        
        /* TABLA DE ESTADOS
        POTENCIAS:      2                   1                       0
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |Jugador En Vision      |Jugador en Rango        |Jugador Avistado        |Estado                  | Salida en Numero       /
        |Enemiga                |de Ataque               |                        |                        | Decimal                /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |0                      |0                       |0                       |Patrullando             | 0                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |0                      |0                       |1                       |BuscarJugador           | 1                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |0                      |1                       |0                       |Patrullando             | 2                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |0                      |1                       |1                       |BuscarJugador           | 3                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |1                      |0                       |0                       |SeguirJugador           | 4                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |1                      |0                       |1                       |SeguirJugador           | 5                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |1                      |1                       |0                       |AtacarJugador           | 6                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        |1                      |1                       |1                       |AtacarJugador           | 7                      /
        +-----------------------+------------------------+------------------------+------------------------+------------------------+
        */

        if (jugadorAvistado && jugadorEnVisionEnemiga)
        {
            //Usamos esta condicion para que la corutina no se llame cada frame
            if(!localizacionCompletada)
            {
                StartCoroutine(establecerPosicionConocidaJugador());
            }
            
        }
        /*
        if (jugador.position == ultimaPosicionConocidaJugador)
        {
            //Debug.Log("Posición del Jugador Revelada");
        }*/
        //MAQUINA DE ESTADOS
        switch (estadoActual)
        {
            //CASOS DE PATRULLA
            case int estadoActual when (estadoActual == 0 || estadoActual == 2):
                //Debug.Log(agente.name + " Estado: " + estadoActual + " Patrullando");
                //agente.stoppingDistance = 0;
                animador.SetBool(estaAtacandoHash, false);
                animador.SetBool(estaSiguiendoHash, false);
                agente.speed = 6;
                patrullar();
                break;
            //CASOS DE BUSQUEDA
            case int estadoActual when (estadoActual == 1 || estadoActual == 3):
                //Debug.Log(agente.name + " Estado: " + estadoActual + " Buscando");
                //agente.stoppingDistance = 0;
                animador.SetBool(estaAtacandoHash, false);
                animador.SetBool(estaSiguiendoHash, true);
                agente.speed = 10;
                iniciarBusqueda();
                break;
            //CASOS DE SEGUIR
            case int estadoActual when (estadoActual > 3 && estadoActual < 6):
                //Debug.Log(agente.name + " Estado: " + estadoActual + " Siguiendo");
                //agente.stoppingDistance = 0;
                animador.SetBool(estaAtacandoHash, false);
                animador.SetBool(estaSiguiendoHash, true);
                jugadorAvistado = true;
                agente.speed = 10;
                seguirJugador();
                break;
            //CASOS DE ATAQUE
            case int estadoActual when (estadoActual > 5):
                //Debug.Log(agente.name + " Estado: " + estadoActual + " Atacando");
                //agente.stoppingDistance = rangoDeAtaque - 1;
                jugadorAvistado = true;
                animador.SetBool(estaSiguiendoHash, false);
                agente.speed = 6;
                atacarJugador();
                break;
        }
        //Debug.Log("ESTA PARTE DEL CODIGO SI SE EJECUTA XD");
    }
    //-----------------------PATRULLA---------------------------------------
     private void patrullar()
    {
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
            siguientePuntoPatrulla();
        

    }
    private void siguientePuntoPatrulla()
    {
        agente.destination = puntosDePatrulla[puntoDestino].position;
        puntoDestino = (puntoDestino + 1) % puntosDePatrulla.Length;
    }
    //-----------------------PATRULLA---------------------------------------

    //-----------------------SEGUIR JUGADOR---------------------------------
    private void seguirJugador(){
        agente.SetDestination(jugador.position);
        //mirarJugador();
    }
    //-----------------------SEGUIR JUGADOR---------------------------------

    //-----------------------ATACAR JUGADOR---------------------------------
    private void atacarJugador(){
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        //float distanciaEnemigoJugador = Vector3.Distance(transform.position, jugador.position);
        

        //Vector3 posicionAtaque = new Vector3(transform.position.x -(rangoDeAtaque),0,transform.position.z - (rangoDeAtaque));

        //agente.SetDestination(posicionAtaque);
        agente.SetDestination(jugador.position);
        //mirarJugador();
        transform.LookAt(jugador);
        transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);
        animador.SetBool(estaAtacandoHash, true);

        if (!yaAtaco)
        {  
            armaEnemiga.GetComponent<SistemaArmasEnemigos>().condicionesDisparo(jugador.position);
            //Resetear Ataque
            yaAtaco = true;
            Invoke(nameof(reiniciarAtaque), 0f);
        }
    }
    
    private void reiniciarAtaque(){
        yaAtaco = false;
    }
    //-----------------------ATACAR JUGADOR---------------------------------

    //Recibir daño:
    public void recibirDanio(int danio)
    {
        vida = vida - danio;
        if(vida <= 0)
        {
            //agente.ResetPath();
            animador.SetBool(estaMuriendoHash, true);
            this.gameObject.GetComponent<IA_Enemigos>().enabled = false;
            agente.speed = 0;
            Invoke(nameof(destruirEnemigo), 2.7f);
        } 
    }
    //Destruir al Jugador
    private void destruirEnemigo()
    {
        
        Destroy(gameObject);
    }

    //----------------Busqueda de Jugador------------------------------------
    private void iniciarBusqueda()
    {
        //Debug.Log(agente.name + "Jugador Avistado: " + jugadorAvistado);
        if (!busquedaEnCurso)
        {
            agente.SetDestination(ultimaPosicionConocidaJugador);
            //Debug.Log("Dirijiendose a posición de busqueda inicial");
        }
        
        //¿Hay una ruta en proceso de cálculo, pero aún no está lista? y La distancia entre la posición del agente y el destino en la ruta actual
        if (!agente.pathPending && agente.remainingDistance < 0.5f && !busquedaEnCurso) StartCoroutine(buscarJugador());
        
    }
    
    IEnumerator buscarJugador()
    {
        busquedaEnCurso = true;
        //Debug.Log(agente.name + " Iniciando Busqueda...");
        // This will wait 1 second like Invoke could do, remove this if you don't need it
        //yield return new WaitForSeconds(1);
        float tiempoTranscurrido = 0;
        while (tiempoTranscurrido < 15 && jugadorAvistado)
        {
            if (jugadorEnVisionEnemiga)
            {
                agente.ResetPath();
                busquedaEnCurso = false;
                yield break;
            }
            // Code to go left here
            //CODIGO PARA "BUSQUEDA DE JUGADOR"
            if (!puntoPatrullaFijado) StartCoroutine(buscarPuntoPatrulla());
        
            if (puntoPatrullaFijado)
            {
                //Nos dirijimos al punto aleaotrio generado por buscarPuntoPatrulla()
                agente.SetDestination(puntoPatrulla);
                //Debug.Log("Buscando...");
            }

            Vector3 distanciaAlPuntoPatrulla = transform.position - puntoPatrulla;
            //Cuando estemos llegando al punto, ponemos false puntoPatrulla para que se reinicie la corutina
            if(distanciaAlPuntoPatrulla.magnitude < 1f)
            {
                puntoPatrullaFijado = false;
            }
            tiempoTranscurrido += Time.deltaTime;
    
            yield return null;
        }
        //Debug.Log(agente.name + " Busqueda Concluida");
        busquedaEnCurso = false;
        jugadorAvistado = false;
        
    }

    IEnumerator buscarPuntoPatrulla()
    {
        yield return new WaitForSeconds(0.5f);
        //Creamos un punto aleatorio usando el rango que le dimos en el inspector
        float randomZ = Random.Range(-rangoPuntoPatrulla, rangoPuntoPatrulla);
        float randomX = Random.Range(-rangoPuntoPatrulla, rangoPuntoPatrulla);

        puntoPatrulla = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        //Casteamos a un rayo para comprobar que el punto que colocamos sea parte del terreno
        if (Physics.Raycast(puntoPatrulla, -transform.up, 2f, esTerreno))
            puntoPatrullaFijado = true;
    }

    IEnumerator establecerPosicionConocidaJugador()
    {
        //Establecemos la ultima posicion del jugador
        ultimaPosicionConocidaJugador = jugador.position;
        //Debug.Log("NUEVA POSICION ESTABELCIDA :)");
        //Seteamos esta variable en TRUE para que no se llame la corutina pasado 1 segundo
        localizacionCompletada = true;
        float tiempoTranscurrido = 0;
        //Esperamos un segundo
        while (tiempoTranscurrido < 1)
        {
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        //Una vez pasado el segundo reestabelcemos la variable para que se 
        //Vuelva a llamar la corrutina
        //Debug.Log("Cooldown Reestblecido");
        localizacionCompletada = false;
    }

    public void mirarJugador()
    {
        Vector3 direccionVistaEnemiga = jugador.position - transform.position;
        direccionVistaEnemiga.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(direccionVistaEnemiga);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
    }

    //----------------------- DIBUJOS PARA REFERENCIA XD -------------------------
    public void OnDrawGizmosSelected()
        {
            //Dibujar rango de ataque
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangoDeAtaque);

            //dibujar rango de vision
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, rangoDeVision);

            // DIbujar Cono de vision
            Gizmos.color = Color.cyan;
            float totalFOV = 90.0f;
            float rayRange = rangoDeVision;
            float halfFOV = totalFOV/2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
            Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
            Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );

            // Dibujar Linea de vision (SphereCast)
            RaycastHit hit;
            Vector3 direccionDelJugador2 = jugador.position - transform.position;
            bool isHit = Physics.SphereCast(transform.position, transform.lossyScale.x / 2, direccionDelJugador2, out hit, rangoDeVision);
            if (isHit)
            {   
                //Si el rayo golpea una pared, sera verde
                if (hit.collider.CompareTag("Pared"))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(transform.position, direccionDelJugador2);
                    //Gizmos.DrawWireSphere(transform.position + direccionDelJugador2, transform.lossyScale.x / 2);

                }else if(hit.collider.CompareTag("Jugador"))
                {
                    //Si el rayo golpea al jugador, sera rojo
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, direccionDelJugador2);
                    Gizmos.DrawWireSphere(transform.position + direccionDelJugador2, transform.lossyScale.x / 2);
                }
            }
            else
            {
                //Si el rayo golpea al jugador, sera rojo
                Gizmos.color = Color.white;
                Gizmos.DrawRay(transform.position, direccionDelJugador2);
            }
        }
}