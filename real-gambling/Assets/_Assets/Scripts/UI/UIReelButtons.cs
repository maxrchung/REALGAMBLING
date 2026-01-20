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
    [SerializeField] private TextMeshProUGUI unlockReelText;
    [SerializeField] private Button upgradeReelButton;
    [SerializeField] private TextMeshProUGUI upgradeReelText;
    [SerializeField] private Image reelSpinActiveImage;
    [Space]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    private int reelIndex;

    public bool IsLocked => isLocked;
    private int base_upgrade_cost = 5;
    public int upgrade_scaling = 3;

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
        // Hardcode don't allow 5th column unlock until 4th
        if (GameSystem.Instance.HowManyReelsDoWeHaveInterrobang() != 4 && reelIndex == 4)
        {
            return;
        }


        if (GameSystem.Instance.TrySubtractMoney(unlockAmount))
        {
            isLocked = false;
            ToggleActive(isLocked);
            GameSystem.Instance.OnUnlockReelButtonPressed(reelIndex);
        }
    }

    public void OnChangeReelButtonClicked()
    {
        if (GameSystem.Instance.TrySubtractMoney(base_upgrade_cost))
        {
            GameSystem.Instance.OnUpgradeReelButtonPressed(reelIndex);
            base_upgrade_cost += upgrade_scaling;
            upgradeReelText.text = base_upgrade_cost.ToString() + "$";
        }
    }

    private void ToggleActive(bool locked)
    {
        isLocked = locked;

        if (!isLocked)
        {
            //reelSpinActiveImage.color = activeColor;

            unlockReelButton.gameObject.SetActive(false);
            upgradeReelButton.gameObject.SetActive(true);
        }
        else
        {
            //reelSpinActiveImage.color = inactiveColor;

            unlockReelButton.gameObject.SetActive(true);
            unlockReelText.text = $"{unlockAmount}$";
            upgradeReelButton.gameObject.SetActive(false);
        }
    }
}
