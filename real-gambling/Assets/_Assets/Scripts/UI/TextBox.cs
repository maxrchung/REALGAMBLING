using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBox : MonoBehaviour
{
    [SerializeField] private Image textBoxBorderImage;
    [SerializeField] private Image textBoxBGImage;
    [SerializeField] private TMP_Text textBoxText;

    public void ToggleVisibility(bool visibility)
    {
        gameObject.SetActive(visibility);
    }

    public void SetText(string text)
    {
        textBoxText.text = text;
    }

    public void MoveBox(Vector2 goPosition)
    {
        gameObject.transform.position = goPosition;
    }
}
