using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour // TODO: encapsulate more than just paricles
{                                 // Sound, and other stuff; Should get renamed then
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
