using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Musica : MonoBehaviour
{
    public AudioSource musicaJuegoTerminado;

    void Awake()
    {
        musicaJuegoTerminado = GetComponent<AudioSource>();
    }

    public void apagarMusica()
    {
        gameObject.SetActive(false);
    }

    public void encenderMusica()
    {
        gameObject.SetActive(true);
    }
}
