using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    private Material dissolveMaterial;
    public float delayTime;

    private void Start()
    {
        dissolveMaterial = GetComponent<ParticleSystemRenderer>().material;
        Invoke("StartLerp", delayTime);
    }

    void StartLerp()
    {
        StartCoroutine(LerpMaterial());
    }

    IEnumerator LerpMaterial()
    {
        float t = -1;

        while(t <= 1)
        {
            dissolveMaterial.SetFloat("_Dissolve", t += Time.deltaTime);

            yield return null;
        }
    }
}
