using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [Header("UI Objects")] [SerializeField]
    private TextMeshProUGUI moneyCounter;

    [Space] [SerializeField] private List<UIReelSpinButton> reelSpinButtons;

    [FormerlySerializedAs("reels")] [Space] [SerializeField]
    private List<UIReel> uiReels;

    [Space] [SerializeField] private Button tradeFingerButton;
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

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public GameObject appleSpinny;

    public ParticleManager particleManager;
    public float difficulty_scaling;
    private int pull_count = 0;
    public SoundManager soundManager;

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
        set { fingerAmount = value; }
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

        reelInstances.Add(CreateReel());
        uiReels[2].SetIcons(reelInstances[2].IconsOnReel);

        hand.Cut();
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
        {
            // edge case, out of money

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

        MoneyAmount -= costToPlay;
        pull_count++;
        if (pull_count % 2 == 0)
        {
            costToPlay = (int)Mathf.Ceil(costToPlay * difficulty_scaling);
        }

        hand.Pull();


        // 2: Spin the reel
        reelsAsBoard = new ReelIcons[5, reelInstances.Count];
        // Debug.Log("================== SPIN ==================");
        for (int i = 0; i < reelInstances.Count; i++)
        {
            int iconSteps = Random.Range(100, 150);
            reelInstances[i].SpinReel(iconSteps);

            // 3: Display results of reel
            // List<ReelIcons> reelResults = reelInstances[i].GetIcons(5);
            // uiReels[i].DisplayIcons(reelResults);
            // uiReels[i].SetIcons(reelInstances[i].IconsOnReel);
            // Debug.Log($"Reel {i}: {reelInstances[i].ToString()}");
            uiReels[i].Spin(iconSteps);
            List<ReelIcons> reelResults = reelInstances[i].GetIcons(5);
            uiReels[i].SetDebugImages(reelResults);
            for (int y = 0; y < reelResults.Count; y++)
            {
                reelsAsBoard[y, i] = reelResults[y];
            }
        }

        //
        foreach (var spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }

        spawnedObjects.Clear();

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

    private void RenderMatches(List<Match> matches)
    {
        foreach (var match in matches)
        {
            string posString =
                string.Join(",", match.matchPositions.Select(matchPos => $"({matchPos.x}, {matchPos.y})"));
            Debug.Log($"{match.pattern.name} matched at position: {posString}");
            foreach (var position in match.matchPositions)
            {
                var reelVerticalOffset = MaxIsReel.iconWidth * (position.x - 2);
                var reelWorldPos = uiReels[position.y].transform.position;
                RectTransform rt = uiReels[position.y].maxIsReel.mask;
                if (rt != null)
                {
                    reelWorldPos = rt.TransformPoint(rt.localPosition - new Vector3(0, reelVerticalOffset, 0));
                }

                spawnedObjects.Add(Instantiate(appleSpinny, reelWorldPos, Quaternion.identity));
            }
        }
    }

    public void OnUpgradeReelButtonPressed(int reelIndex)
    {
        Debug.Log($"Changing reel for {reelIndex}");
        reelInstances[reelIndex].UpgradeReelValue(7);
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

    public bool TryToTryToSubtractMoney(int difference)
    {
        if (moneyAmount >= difference)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        bool kaching = false;
        foreach (Match m in matches)
        {
            kaching = true;
            foreach (Vector2Int pos in m.matchPositions)
            {
                switch (reelsAsBoard[pos.x, pos.y])
                {
                    case ReelIcons.Worm:
                        particleManager.BurstParticles(0);
                        goto case ReelIcons.SmallFry;
                    case ReelIcons.SmallFry:
                        particleManager.BurstParticles(1);
                        goto case ReelIcons.Hook;
                    case ReelIcons.Hook:
                        particleManager.BurstParticles(2);
                        goto case ReelIcons.Fish;
                    case ReelIcons.Fish:
                        particleManager.BurstParticles(3);
                        goto case ReelIcons.Bass;
                    case ReelIcons.Bass:
                        particleManager.BurstParticles(4);
                        goto case ReelIcons.Mackerel;
                    case ReelIcons.Mackerel:
                        particleManager.BurstParticles(5);
                        goto case ReelIcons.Chef;
                    case ReelIcons.Chef:
                        particleManager.BurstParticles(6);
                        goto case ReelIcons.Sashimi;
                    case ReelIcons.Sashimi:
                        particleManager.BurstParticles(7);
                        goto case ReelIcons.MaxFish;
                    case ReelIcons.MaxFish:
                        particleManager.BurstParticles(8);
                        break;
                }
            }
        }

        if (kaching)
        {
            soundManager.PlaySound(8);
        }

        // 4: Score matches
        // - Count the icons that matched?
        // - Check icon attributes
        // - Add back to money
        int spinScore = ScoreMatches(matches);
        MoneyAmount += spinScore;

        RenderMatches(matches);
        AfterPlayerAction();
    }

    public int HowManyReelsDoWeHaveInterrobang()
    {
        return reelInstances.Count;
    }
}