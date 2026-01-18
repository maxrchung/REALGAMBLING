using System.Collections;
using UnityEngine;

public class MaxIsHandScript : MonoBehaviour
{
    public GameObject lever;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        var animator = GetComponent<Animator>();

        yield return new WaitForSeconds(1);
        animator.SetTrigger("Pull");

        StartCoroutine(rotateLever());


        yield return new WaitForSeconds(5);
        animator.SetTrigger("Return");
    }

    private IEnumerator rotateLever()
    {
        // Idk some slight pause to delay for animation???
        yield return new WaitForSeconds(0.25f);


        float duration = 1f;
        float elapsed = 0f;

        Quaternion start = lever.transform.rotation;
        Quaternion end = start * Quaternion.Euler(-45f, 0f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            lever.transform.rotation = Quaternion.Lerp(start, end, t);
            yield return null;
        }

        lever.transform.rotation = end; // snap exact

        // Go back
        yield return new WaitForSeconds(2);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            lever.transform.rotation = Quaternion.Lerp(end, start, t);
            yield return null;
        }

        lever.transform.rotation = start; // snap exact

    }

    // Update is called once per frame
    void Update()
    {

    }
}
