using System.Collections;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        var animator = GetComponent<Animator>();

        yield return new WaitForSeconds(1);
        animator.SetTrigger("Pull");

        yield return new WaitForSeconds(5);
        animator.SetTrigger("Return");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
