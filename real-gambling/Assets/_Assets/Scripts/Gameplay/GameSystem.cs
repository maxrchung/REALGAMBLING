using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private TextMeshProUGUI moneyCounter;

    [Space]
    [SerializeField] private List<UIReelSpinButton> reelSpinButtons;

    [FormerlySerializedAs("reels")]
    [Space]
    [SerializeField] private List<UIReel> uiReels;

    [Space]
    [SerializeField] private Button tradeFingerButton;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playCostText;

    public HandScript hand;

    private int moneyAmount;
    private int fingerAmount;
    private int costToPlay;

    private Vector2 screenCenter = new Vector2(960, 510);

    private List<Reel> reelInstances;
    private ReelIcons[,] reelsAsBoard;

    private static GameSystem instance;

    public int MoneyAmount
    {
        get => moneyAmount;
        set
        {
            moneyAmount = value;
            moneyCounter.text = moneyAmount.ToString() + "$";
        }
    }

    public int FingerAmount
    {
        get => fingerAmount;
        set
        {
            fingerAmount = value;
        }
    }

    public int CostToPlay
    {
        get => costToPlay;
        set
        {
            costToPlay = value;
            playCostText.text = costToPlay.ToString() + "$";
        }
    }

    public static GameSystem Instance => instance;

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
        CostToPlay = 1;
        reelInstances = new List<Reel>();

        for (int i = 0; i < reelSpinButtons.Count; i++)
        {
            reelSpinButtons[i].Initialize(i);
        }

        reelInstances.Add(CreateReel());
        uiReels[0].SetIcons(reelInstances[0].IconsOnReel);

        reelInstances.Add(CreateReel());
        uiReels[1].SetIcons(reelInstances[1].IconsOnReel);

        AfterPlayerAction();
    }

    public Reel CreateReel()
    {
        Reel newReel = new Reel(12, 10);
        Debug.Log(newReel.ToString());
        return newReel;
    }

    public void AfterPlayerAction()
    {
        playCostText.text = $"{costToPlay}$";
        if (moneyAmount < costToPlay)
        { // edge case, out of money
            playButton.interactable = false;

            if (fingerAmount > 0)
            {
                hand.Cut();
            }
            else
            {
                // Game Over -- Handled by HandScript
            }

            return;
        }

        //tradeFingerButton.interactable = false;
        //if (reelSpinButtons[2].IsLocked)
        //{
        //    // TODO: edge case, game start -- need third reel
        //    playButton.interactable = false;
        //    return;
        //}

        // Main gameplay
        playButton.interactable = true;
    }

    public void OnPlayButtonPressed()
    {
        // 1: Check player's money and subtract if enough
        if (moneyAmount < costToPlay)
        {
            return;
        }

        MoneyAmount -= costToPlay;


        hand.Pull();


        // 2: Spin the reel
        reelsAsBoard = new ReelIcons[5, reelInstances.Count];
        for (int i = 0; i < reelInstances.Count; i++)
        {
            int iconSteps = Random.Range(100, 150);
            reelInstances[i].SpinReel(iconSteps);

            // 3: Display results of reel
            // List<ReelIcons> reelResults = reelInstances[i].GetIcons(5);
            // uiReels[i].DisplayIcons(reelResults);
            uiReels[i].SetIcons(reelInstances[i].IconsOnReel);
            uiReels[i].Spin(iconSteps);
            List<ReelIcons> reelResults = reelInstances[i].GetIcons(5);

            for (int y = 0; y < reelResults.Count; y++)
            {
                reelsAsBoard[y, i] = reelResults[y];
            }
        }

        // 2.5: Delay until spin finishes
        // TODO: use async await
        StartCoroutine(SpinReelDelay(5));
    }

    public void OnUnlockReelButtonPressed(int reelIndex)
    {
        reelInstances.Add(CreateReel());
        uiReels[reelIndex].SetIcons(reelInstances[reelIndex].IconsOnReel);
        AfterPlayerAction();
    }

    public void OnUpgradeReelButtonPressed(int reelIndex)
    {
        Debug.Log($"Changing reel for {reelIndex}");
        reelInstances[reelIndex].UpgradeReelValue(5);
        print(reelInstances[reelIndex]);
        uiReels[reelIndex].SetIcons(reelInstances[reelIndex].IconsOnReel);
        AfterPlayerAction();
    }

    private List<Match> CheckMatches(List<WinningCombinationSO> combinationsToCheck)
    {
        List<Match> matchList = new List<Match>();
        List<Vector2Int> matchPositions = new List<Vector2Int>();

        // iterate across board
        int boardHeight = reelsAsBoard.GetLength(0);
        int boardWidth = reelsAsBoard.GetLength(1);
        for (int boardRow = 0; boardRow < boardHeight; boardRow++)
        {
            for (int boardCol = 0; boardCol < boardWidth; boardCol++)
            {
                // iterate through all combos
                foreach (WinningCombinationSO combo in combinationsToCheck)
                {
                    matchPositions.Clear();
                    // if there isn't enough space to check this combo, skip
                    if (boardRow + combo.Height > boardHeight || boardCol + combo.Width > boardWidth)
                        continue;

                    bool isValid = true;

                    // iterate through all of pattern's squares
                    for (int comboRow = 0; comboRow < combo.Height; comboRow++)
                    {
                        for (int comboCol = 0; comboCol < combo.Width; comboCol++)
                        {

                            // continue to next square if there is no icon set
                            // in the inspector, the axes are switched
                            if (!combo.Pattern[comboCol, comboRow])
                                continue;

                            // if the board doesn't have an icon, exit loop early
                            // [y, x] = board's top left corner; add u and v to iterate with pattern
                            if (reelsAsBoard[boardRow + comboRow, boardCol + comboCol] == ReelIcons.None)
                            {
                                isValid = false;
                                break;
                            }

                            matchPositions.Add(new Vector2Int(boardRow + comboRow, boardCol + comboCol));
                        }

                        // leave pattern early if the pattern was broken
                        if (!isValid)
                            break;
                    }

                    // if the entire pattern was iterated through and remained valid, count
                    if (isValid)
                    {
                        // int[,] positions =
                        Vector2Int[] positions = new Vector2Int[matchPositions.Count];
                        matchPositions.CopyTo(positions);
                        matchList.Add(new Match(combo, positions));
                    }
                }
            }
        }

        return matchList;
    }

    private int ScoreMatches(List<Match> matches)
    {
        int totalScore = 0;

        foreach (var match in matches)
        {
            // TODO: Give the patterns multipliers/bonus score
            // TODO: Upgrade the reel
            // for now, we're doing linear addition
            // could spice it up with:
            // amount of icons in one match -- the more icons in a match, the higher it scales
            // amount of matches in one spin -- the more matches, the higher it scales
            // difficulty of pattern -- the rarer the pattern, the bigger the multiplier

            Debug.Log(match.pattern.name);
            foreach (var position in match.matchPositions)
            {
                Debug.Log($"position: {position}");
                ReelIcons icon = reelsAsBoard[position.x, position.y];
                totalScore += SOReferences.Instance.Icons.Values[icon].PointAmount;
                Debug.Log($"{icon}, {SOReferences.Instance.Icons.Values[icon].PointAmount}");
            }
            Debug.Log($"score after combo: {totalScore}");
        }
        Debug.Log($"{matches.Count} matches made, {totalScore} money earned");

        return totalScore;
    }

    public void OnTradeFingerButtonPressed()
    {
        hand.Cut(); // Money added in HandScript

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

    private IEnumerator SpinReelDelay(float timeInSeconds)
    {
        playButton.interactable = false;
        yield return new WaitForSeconds(timeInSeconds);

        playButton.interactable = true;

        // 3: Check for winning combinations
        List<WinningCombinationSO> combinationsToCheck;

        switch (reelInstances.Count)
        {
            case 3:
                combinationsToCheck = SOReferences.Instance.Combinations.ThreeReelCombinations;
                break;
            case 4:
                combinationsToCheck = SOReferences.Instance.Combinations.FourReelCombinations;
                break;
            case 5:
                combinationsToCheck = SOReferences.Instance.Combinations.FiveReelCombinations;
                break;
            default:
                Debug.LogError($"{reelInstances.Count} not supported for checking");
                yield break;
        }

        List<Match> matches = CheckMatches(combinationsToCheck);

        // 4: Score matches
        // - Count the icons that matched?
        // - Check icon attributes
        // - Add back to money
        int spinScore = ScoreMatches(matches);
        MoneyAmount += spinScore;

        AfterPlayerAction();
    }
}