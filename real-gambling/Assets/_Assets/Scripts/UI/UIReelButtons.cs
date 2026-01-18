using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIReelSpinButton : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    [SerializeField, ShowIf("isLocked")] private int unlockAmount;
    [Space]
    [SerializeField] private Button unlockReelButton;
    [SerializeField] private Button upgradeReelButton;
    [SerializeField] private Button upgradeIconButton;
    [SerializeField] private Image reelSpinActiveImage;
    [Space]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    private bool reelActive;

    public bool ReelActive => reelActive;

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
        if (PrototypeGameSystem.Instance.SubtractMoney(unlockAmount))
        {
            ToggleActive(!reelActive);   
        }
    }

    public void OnUpgradeReelButtonClicked()
    {
        
    }

    public void OnUpgradeIconButtonClicked()
    {
        
    }

    private void ToggleActive(bool isActive)
    {
        reelActive = isActive;

        if (reelActive)
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
            upgradeReelButton.gameObject.SetActive(false);
            upgradeIconButton.gameObject.SetActive(false);
        }
    }
}
