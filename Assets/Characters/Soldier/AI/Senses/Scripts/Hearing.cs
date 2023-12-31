using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hearing : MonoBehaviour, INoiseReceiver
{
    private LayerMask obstacleMask = ~(1 << 9);

    [System.Serializable]
    public class HeardNoise
    {
        public float timeOfEmission;
        public NoiseEmitter noiseEmitter;
    }

    public List<HeardNoise> noiseEmittersBeingHeard = new List<HeardNoise>(); // Should not be public, but offer the noises through a loop in a getter method.
    [HideInInspector] public UnityEvent<NoiseEmitter> onHeardNoiseEmitter;
    [HideInInspector] public UnityEvent onForgotNoiseEmitter;
    [SerializeField] float forgetFrequency = 5f;
    [SerializeField] float noiseLifeSpan = 1f;


    Coroutine forgetNoisesCoroutine;
    void OnEnable()
    {
        forgetNoisesCoroutine = StartCoroutine(ForgetNoises());
    }

    void OnDisable() 
    { 
        StopCoroutine(forgetNoisesCoroutine);
    }

    void INoiseReceiver.NotifyNoise(NoiseEmitter noiseEmitter)
    {
        HeardNoise heardNoise = noiseEmittersBeingHeard.Find(x => x.noiseEmitter == noiseEmitter);

        // TODO: have enemies not "hear" the player when a wall stands between them.
        //RaycastHit hit;
        //Physics.Raycast(transform.position, (noiseEmitter.transform.position - transform.position).normalized, out hit, 5f, obstacleMask);
        //if (!hit.collider.CompareTag("Player")) { return; }

        if(heardNoise == null)
        {
            heardNoise = new HeardNoise();
            heardNoise.timeOfEmission = Time.time;
            heardNoise.noiseEmitter = noiseEmitter;
            noiseEmittersBeingHeard.Add(heardNoise);
        }
        else
        {
            heardNoise.timeOfEmission = Time.time;
        }
        onHeardNoiseEmitter.Invoke(noiseEmitter);
    }

    IEnumerator ForgetNoises()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / forgetFrequency);
            noiseEmittersBeingHeard.RemoveAll(x => (Time.time - x.timeOfEmission) > noiseLifeSpan);
            onForgotNoiseEmitter.Invoke();
        }
    }
}
