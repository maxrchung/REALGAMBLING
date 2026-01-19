using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandScript : MonoBehaviour
{
    public SkinnedMeshRenderer Renderer;
    public Material Transparent;
    public Image RedPanel;
    public Image EndPanel;
    public Text[] EndTexts;

    public TextMeshProUGUI MoneyText;

    private Animator animator;

    private int money = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        MoneyText.text = money.ToString() + "$";
    }

    public void Pull()
    {
        money = money - 1;
        MoneyText.text = money + "$";
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

    public void Die()
    {
        fingers = 1;
        StartCoroutine(CutCoroutine());
    }

    public IEnumerator CutCoroutine()
    {
        animator.SetTrigger("View");

        // Wait for hand to come into view
        yield return new WaitForSeconds(2);

        var materials = Renderer.materials;
        // Has to match material slots
        materials[fingers] = Transparent;
        Renderer.materials = materials;

        fingers = fingers - 1;

        // End game
        if (fingers == 0)
        {
            StartCoroutine(EndGame());
        }
        else
        {
            StartCoroutine(FlashRed());

            // After red flash, return hand back
            yield return new WaitForSeconds(1);

            animator.SetTrigger("Unview");
        }
    }

    private IEnumerator FlashRed()
    {
        // Flash red
        Color color = RedPanel.color;
        color.a = 0.9f; // Fully visible
        RedPanel.color = color;
        var time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / 1f; // Fade over one second
            color.a = Mathf.Lerp(0.9f, 0, time);
            RedPanel.color = color;
            yield return null;
        }
        color.a = 0;
        RedPanel.color = color;
    }

    private IEnumerator EndGame()
    {
        // Flash red
        Color color = EndPanel.color;
        var time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / 1f;
            color.a = Mathf.Lerp(0, 1f, time);
            EndPanel.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(1);

        color = EndTexts[0].color;
        color.a = 1;
        EndTexts[0].color = color;

        yield return new WaitForSeconds(2);

        time = 0f;
        color = EndTexts[1].color;
        while (time < 1f)
        {
            time += Time.deltaTime / 0.67f;
            color.a = Mathf.Lerp(0, 1f, time);
            EndTexts[1].color = color;
            yield return null;
        }

        time = 0f;
        color = EndTexts[2].color;
        while (time < 1f)
        {
            time += Time.deltaTime / 0.69f;
            color.a = Mathf.Lerp(0, 1f, time);
            EndTexts[2].color = color;
            yield return null;
        }
    }

    public void Buy()
    {
        money = money - 1;
        MoneyText.text = money.ToString() + "$";
    }

    public void Gain()
    {
        money = money + 2;
        MoneyText.text = money.ToString() + "$";
    }


    // Update is called once per frame
    void Update()
    {

    }
}
