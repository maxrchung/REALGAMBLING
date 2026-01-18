using TMPro;
using UnityEngine;

public class UICounter : MonoBehaviour
{
    [SerializeField] private TMP_Text counterHeaderText;
    [SerializeField] private TMP_Text counterAmountText;

    public void SetHeaderText(string headerString)
    {
        counterHeaderText.text = headerString;
    }

    public void SetAmountText(string amountString)
    {
        counterAmountText.text = amountString;
    }
}
