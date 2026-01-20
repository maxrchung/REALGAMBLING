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
    public Image WinPanel;
    public Text[] EndTexts;
    public SoundManager soundManager;
    public GameObject door;
    public TextMeshProUGUI YouLeftText;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Pull()
    {
        animator.SetTrigger("Pull");
        soundManager.PlaySound(10);
        soundManager.PlayIntroAndLoop();
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

    public void Cut()
    {
        soundManager.StopSong();
        StartCoroutine(CutCoroutine());
    }

    public void Die()
    {
        GameSystem.Instance.FingerAmount = 1;
        StartCoroutine(CutCoroutine());
    }

    public IEnumerator CutCoroutine()
    {
        animator.SetTrigger("View");

        // Wait for hand to come into view
        yield return new WaitForSeconds(2);

        var materials = Renderer.materials;
        // Has to match material slots
        materials[GameSystem.Instance.FingerAmount] = Transparent;
        Renderer.materials = materials;

        GameSystem.Instance.FingerAmount--;

        // End game
        if (GameSystem.Instance.FingerAmount == 0)
        {
            StartCoroutine(EndGame());
        }
        else
        {
            GameSystem.Instance.MoneyAmount += 10;
            StartCoroutine(FlashRed());

            // After red flash, return hand back
            yield return new WaitForSeconds(1);

            animator.SetTrigger("Unview");
            GameSystem.Instance.AfterPlayerAction();
        }
    }

    private IEnumerator FlashRed()
    {
        // Flash red
        Color color = RedPanel.color;
        color.a = 0.9f; // Fully visible
        RedPanel.color = color;
        soundManager.PlaySound(9);
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
        GameSystem.Instance.MoneyAmount -= 1;
    }

    public void Gain()
    {
        GameSystem.Instance.MoneyAmount += 2;
    }

    public void Win(int takeAway)
    {
        StartCoroutine(WinCoroutine(takeAway));
    }

    private IEnumerator WinCoroutine(int takeAway)
    {

        Color color = WinPanel.color;
        var time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime / 1f;
            color.a = Mathf.Lerp(0, 1f, time);
            WinPanel.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(2);

        color = EndTexts[3].color;
        color.a = 1;
        EndTexts[3].color = color;

        YouLeftText.gameObject.SetActive(true);
        YouLeftText.text = takeAway.ToString() + "$";

        yield return new WaitForSeconds(2);

        door.SetActive(true);
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
}
