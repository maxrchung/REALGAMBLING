using System.Collections;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    public SkinnedMeshRenderer Renderer;
    public Material Transparent;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Pull()
    {
        animator.SetTrigger("Pull");
    }

    public void Return()
    {
        animator.SetTrigger("Return");
    }

    public void View()
    {
        animator.SetTrigger("View");
    }

    public void Unview()
    {
        animator.SetTrigger("Unview");
    }



    private int fingers = 5;

    public void Cut()
    {
        StartCoroutine(CutCoroutine());
    }

    public IEnumerator CutCoroutine()
    {
        animator.SetTrigger("View");

        yield return new WaitForSeconds(2);

        var materials = Renderer.materials;
        if (fingers == 5)
        {
            materials[5] = Transparent;
        }
        else if (fingers == 4)
        {
            materials[4] = Transparent;
        }
        else if (fingers == 3)
        {
            materials[3] = Transparent;
        }
        else if (fingers == 2)
        {
            materials[2] = Transparent;
        }
        else if (fingers == 1)
        {
            // Lose
            materials[1] = Transparent;
        }

        Renderer.materials = materials;

        fingers = fingers - 1;

        yield return new WaitForSeconds(1);
        animator.SetTrigger("Unview");
    }


    // Update is called once per frame
    void Update()
    {

    }
}
