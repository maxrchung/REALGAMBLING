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

    private int moneyAmount;
    private int fingerAmount;

    private static PrototypeGameSystem instance;

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
        moneyAmount = 0;
        fingerAmount = 5;
    }

    private void RoundStart()
    {
        if (moneyAmount <= 0)
        {
            playButton.interactable = false;
            
            if (fingerAmount == 0)
            {
                Debug.Log("No more money and fingers, game over!");
            }
            else
            {
                tradeFingerButton.interactable = true;
            }
        }
        else
        {
            playButton.interactable = true;
            tradeFingerButton.interactable = false;
            
            
        }
    }
    
    public bool SubtractMoney(int difference)
    {
        if (moneyAmount > difference)
        {
            moneyAmount -= difference;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SubtractFingers(int difference)
    {
        if (fingerAmount > difference)
        {
            fingerAmount -= difference;
            return true;
        }
        else
        {
            return false;
        }
    }
}
