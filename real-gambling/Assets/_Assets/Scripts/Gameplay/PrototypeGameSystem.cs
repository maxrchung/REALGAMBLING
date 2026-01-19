using DG.Tweening.Core.Easing;
using GambleCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GambleCore.Interface;
using GambleCore.Util;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class PrototypeGameSystem : MonoBehaviour
{
    [SerializeField] private UICounter moneyCounter;
    [SerializeField] private UICounter fingerCounter;

    [Space]
    [SerializeField] private List<UIReelSpinButton> reelSpinButtons;

    [Space]
    [SerializeField] private List<UIReel> reels;

    [Space] [SerializeField] private UIBetField betField;

    [Space] [SerializeField] private Button tradeFingerButton;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playButtonText;

    [Space] [SerializeField] private TextBox textBox;

    [Space] [SerializeField] private ReelIconValuesSO iconSO;

    private int moneyAmount;
    private int fingerAmount;
    private int costToPlay;

    // START BLOCK -- gambling controller fields
    private GamblingController _gamblingController;
    private IGamblingBoard _board;
    private List<GamblingWheelController> wheels;

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
        _gamblingController = new GamblingController();
        _board = _gamblingController.CreateBoard(0);
        wheels = new List<GamblingWheelController>();
        
        MoneyAmount = 0;
        FingerAmount = 5;
        costToPlay = 1;

        foreach (var reelSpinButton in reelSpinButtons)
        {
            reelSpinButton.Initialize();
        }
        betField.ResetText();
        
        CreateWheel();
        CreateWheel();
        AfterPlayerAction();
    }

    public void CreateWheel()
    {
        string seedName = "GamblingWheel" + wheels.Count;
        var rng = DeterministicRng.CreateStream(0, seedName);
        wheels.Add(new GamblingWheelController());
        wheels[^1].RandomizeSymbols(rng);
        _board.AddWheel(wheels[^1]);
    }
    
    public void AfterPlayerAction()
    {
        playButtonText.text = $"PLAY\n(${costToPlay})";
        if (moneyAmount < costToPlay)
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
        if (reelSpinButtons[2].IsLocked)
        {
            // edge case, game start -- need third reel
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

    public void OnPlayButtonPressed()
    {
        if (moneyAmount < costToPlay)
        {
            return;    
        }

        MoneyAmount -= costToPlay;
        
        // stub
        var steps = _board.GetRandomSteps();
        _board.PerformSteps(steps);

        for (int i = 0; i < wheels.Count; i++)
        {
            var wheelSprites = wheels[i].ShownSymbols.Select(symbol => iconSO.ReelValues[((ReelIconAdapter)symbol).Value].iconSprite);
            reels[i].DisplayIcons(wheelSprites.ToArray());
        }

        AfterPlayerAction();
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
        if (moneyAmount >= difference)
        {
            MoneyAmount -= difference;
            return true;
        }

        return false;
    }

    public bool TrySubtractFingers(int difference)
    {
        if (fingerAmount >= difference)
        {
            FingerAmount -= difference;
            return true;
        }

        return false;
    }
}