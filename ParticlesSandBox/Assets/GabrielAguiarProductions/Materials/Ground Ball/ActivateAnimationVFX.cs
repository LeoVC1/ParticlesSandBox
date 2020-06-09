using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimationVFX : MonoBehaviour
{
    public GameObject[] handVFX;
    public GameObject[] spellVFX;
    public Animator anim;

    private void Update()
    {
        if (Input.anyKey)
        {
            int index = Convert.ToInt32(Input.inputString);

            ActivateVFX(index);
        }
    }

    public void ActivateVFX(int index)
    {
        foreach(GameObject g in handVFX)
        {
            g.SetActive(false);
        }

        foreach (GameObject g in spellVFX)
        {
            g.SetActive(false);
        }

         
        handVFX[index * 2].SetActive(true);
        handVFX[index * 2 + 1].SetActive(true);        spellVFX[index].SetActive(true);
        anim.SetTrigger("animate");
    }
}
