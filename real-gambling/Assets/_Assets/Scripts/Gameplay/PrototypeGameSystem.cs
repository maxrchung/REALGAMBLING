using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeGameSystem : MonoBehaviour
{
    [SerializeField] private UICounter moneyCounter;
    [SerializeField] private UICounter fingerCounter;

    [Space]
    [SerializeField] private UIReelSpinButton reelSpinButtonOne;
    [SerializeField] private UIReelSpinButton reelSpinButtonTwo;
    [SerializeField] private UIReelSpinButton reelSpinButtonThree;
    [SerializeField] private UIReelSpinButton reelSpinButtonFour;
    [SerializeField] private UIReelSpinButton reelSpinButtonFive;

    [Space]
    [SerializeField] private UIReel reelOne;
    [SerializeField] private UIReel reelTwo;
    [SerializeField] private UIReel reelThree;
    [SerializeField] private UIReel reelFour;
    [SerializeField] private UIReel reelFive;

    [Space]
    [SerializeField] private UIBetField betField;
    
    [Space]
    [SerializeField] private Button tradeFingerButton;
    [SerializeField] private Button playButton;

    [Space]
    [SerializeField] private TextBox textBox;

    [Space]
    [SerializeField] private ReelIconValuesSO iconSO;

    private int moneyAmount;
    private int fingerAmount;

    private Vector2 screenCenter = new Vector2(960, 510);
    
    private static PrototypeGameSystem instance;
    
    public int MoneyAmount
    {
        get => moneyAmount;
        set
        {
            moneyAmount = value;
            moneyCounter.SetAmountText("$" + moneyAmount.ToString());
        }
    }

    public int FingerAmount
    {
        get => fingerAmount;
        set
        {
            fingerAmount = value;
            fingerCounter.SetAmountText(fingerAmount.ToString());
        }
    }
    
    public static PrototypeGameSystem Instance => instance;
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Start()
    {
        MoneyAmount = 0;
        FingerAmount = 5;

        reelSpinButtonOne.Initialize();
        reelSpinButtonTwo.Initialize();
        reelSpinButtonThree.Initialize();
        reelSpinButtonFour.Initialize();
        reelSpinButtonFive.Initialize();
        betField.ResetText();
        
        AfterPlayerAction();
        reelOne.DisplayIcons(new Sprite[]
        {
            iconSO.ReelValues[ReelIcons.Bread].iconSprite,
            iconSO.ReelValues[ReelIcons.Fish].iconSprite,
            iconSO.ReelValues[ReelIcons.Snake].iconSprite,
            iconSO.ReelValues[ReelIcons.PepperMan].iconSprite,
            iconSO.ReelValues[ReelIcons.CarKey].iconSprite,
        });
    }
    
    public void AfterPlayerAction()
    {
        if (moneyAmount <= 0)
        { // edge case, out of money
            playButton.interactable = false;
            
            if (fingerAmount == 0)
            {
                textBox.SetText("Yer outta money and fingers.\n\nGAME OVER!");
                textBox.MoveBox(screenCenter);
                textBox.ToggleVisibility(true);
            }
            else
            {
                textBox.SetText("Yer outta money.\n\nGive me a finger fer more!");
                textBox.MoveBox(screenCenter);
                textBox.ToggleVisibility(true);
                tradeFingerButton.interactable = true;
            }

            return;
        }

        tradeFingerButton.interactable = false;
        Debug.Log($"reelSpinButtonThree.IsLocked: {reelSpinButtonThree.IsLocked}");
        if (reelSpinButtonThree.IsLocked)
        { // edge case, game start -- need third reel
            textBox.SetText("Ya need at least three reels to play.\n\nBuy one now!");
            textBox.MoveBox(screenCenter + new Vector2(150, 300));
            textBox.ToggleVisibility(true);
            playButton.interactable = false;
            return;
        }
        
        // Main gameplay
        textBox.ToggleVisibility(false);
        playButton.interactable = true;
    }

    public void OnTradeFingerButtonPressed()
    {
        if (!TrySubtractFingers(1))
            return;
        
        MoneyAmount += 10;
        AfterPlayerAction();
    }
    
    public bool TrySubtractMoney(int difference)
    {
        if (moneyAmount > difference)
        {
            MoneyAmount -= difference;
            return true;
        }
        
        return false;
    }

    public bool TrySubtractFingers(int difference)
    {
        if (fingerAmount > difference)
        {
            FingerAmount -= difference;
            return true;
        }
        
        return false;
    }
}
