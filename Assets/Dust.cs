using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour
{
    private ParticleSystem dust = null;

    void Start()
    {
        dust = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        dust.Play();
    }

}
