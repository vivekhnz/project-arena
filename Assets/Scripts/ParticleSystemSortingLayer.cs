using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemSortingLayer : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Renderer renderer = particleSystem.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = "Particles";
            renderer.sortingOrder = 2;
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
