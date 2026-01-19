using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIReelSpinButton : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    [SerializeField, ShowIf("isLocked")] private int unlockAmount;
    [Space]
    [SerializeField] private Button unlockReelButton;
    [SerializeField] private TMP_Text unlockReelText;
    [SerializeField] private Button upgradeReelButton;
    [SerializeField] private TMP_Text upgradeReelText;
    [SerializeField] private Button changeReelButton;
    [SerializeField] private TMP_Text changeReelText;
    [SerializeField] private Image reelSpinActiveImage;
    [Space]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    private int reelIndex;

    public bool IsLocked => isLocked;

    public void Initialize(int index)
    {
        reelIndex = index;
        Reset();
    }

    public void Reset()
    {
        ToggleActive(isLocked);
    }

    public void OnUnlockButtonClicked()
    {
        if (GameSystem.Instance.TrySubtractMoney(unlockAmount))
        {
            isLocked = false;
            ToggleActive(isLocked);
            GameSystem.Instance.OnUnlockReelButtonPressed(reelIndex);
        }
    }

    public void OnUpgradeReelButtonClicked()
    {
        
    }

    public void OnChangeReelButtonClicked()
    {
        if (GameSystem.Instance.TrySubtractMoney(5))
        {
            GameSystem.Instance.OnChangeReelButtonPressed(reelIndex);
        }
    }

    private void ToggleActive(bool locked)
    {
        isLocked = locked;

        if (!isLocked)
        {
            reelSpinActiveImage.color = activeColor;
            
            unlockReelButton.gameObject.SetActive(false);
            upgradeReelButton.gameObject.SetActive(true);
            changeReelButton.gameObject.SetActive(true);
        }
        else
        {
            reelSpinActiveImage.color = inactiveColor;
            
            unlockReelButton.gameObject.SetActive(true);
            unlockReelText.text = $"UNLOCK (${unlockAmount})";
            upgradeReelButton.gameObject.SetActive(false);
            changeReelButton.gameObject.SetActive(false);
        }
    }
}
