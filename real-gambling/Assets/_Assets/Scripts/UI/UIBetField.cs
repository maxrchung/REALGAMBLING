using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBetField : MonoBehaviour
{
    [SerializeField] private TMP_Text betHeaderText;
    [SerializeField] private TMP_InputField betInputField;

    public int GetInput()
    {
        return int.Parse(betInputField.text);
    }

    public void ResetText()
    {
        betInputField.text = "";
    }
}
