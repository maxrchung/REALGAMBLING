using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Button upgradeIconButton;
    [SerializeField] private TMP_Text unlockIconText;
    [SerializeField] private Image reelSpinActiveImage;
    [Space]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;


    public bool IsLocked => isLocked;

    public void Initialize()
    {
        Reset();
    }

    public void Reset()
    {
        ToggleActive(isLocked);
    }

    public void OnUnlockButtonClicked()
    {
        if (PrototypeGameSystem.Instance.TrySubtractMoney(unlockAmount))
        {
            isLocked = false;
            ToggleActive(isLocked);
            PrototypeGameSystem.Instance.CreateWheel();
            PrototypeGameSystem.Instance.AfterPlayerAction();
        }
    }

    public void OnUpgradeReelButtonClicked()
    {
        
    }

    public void OnUpgradeIconButtonClicked()
    {
        
    }

    private void ToggleActive(bool locked)
    {
        isLocked = locked;

        if (!isLocked)
        {
            reelSpinActiveImage.color = activeColor;
            
            unlockReelButton.gameObject.SetActive(false);
            upgradeReelButton.gameObject.SetActive(true);
            upgradeIconButton.gameObject.SetActive(true);
        }
        else
        {
            reelSpinActiveImage.color = inactiveColor;
            
            unlockReelButton.gameObject.SetActive(true);
            unlockReelText.text = $"UNLOCK (${unlockAmount})";
            upgradeReelButton.gameObject.SetActive(false);
            upgradeIconButton.gameObject.SetActive(false);
        }
    }
}
