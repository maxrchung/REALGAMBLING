using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Reel
{
    private int currentIndex;
    private List<ReelIcons> iconsOnReel;
    private int multiplier;

    public int CurrentIndex => currentIndex;
    public List<ReelIcons> IconsOnReel => iconsOnReel;
    public int Multiplier => multiplier;
    private int reelValue;
    private int maxReelValue;
    private int reelSize;
    private int reelItems;

    public Reel(int minIcons, int maxIcons, int reelSize)
    {
        currentIndex = 0;
        iconsOnReel = new List<ReelIcons>();
        multiplier = 1;

        for (int i = 0; i < reelSize; i++)
        {
            iconsOnReel.Add(ReelIcons.None);
        }

        AddIconsRandomly(Random.Range(minIcons, maxIcons));
    }

    public Reel(int newReelSize, int newReelValue)
    {
        Debug.Log("creating new reel");
        maxReelValue = newReelValue;
        reelValue = 0;
        reelSize = newReelSize;
        reelItems = 0;
        currentIndex = 0;
        iconsOnReel = new List<ReelIcons>();
        for (int i = 0; i < reelSize; i++)
        {
            iconsOnReel.Add(ReelIcons.None);
        }
        AddIconsUntilValue();
    }

    private void AddIconsRandomly(int remainingIcons)
    {
        if (!iconsOnReel.Contains(ReelIcons.None))
        {
            Debug.LogError("No more space for new icons!");
            return;
        }

        int i = 0;

        while (remainingIcons > 0 && iconsOnReel.Contains(ReelIcons.None))
        {
            // if current index has no icon and roll is successful
            if (iconsOnReel[i] == ReelIcons.None && Random.Range(0, 2) == 1)
            {
                iconsOnReel[i] = ReelIcons.Snake;
                remainingIcons--;
            }

            i++;
            i %= iconsOnReel.Count;
        }
    }

    private void AddIconsUntilValue()
    {
        while (reelValue < maxReelValue)
        {
            int tryIndex = Random.Range(0, reelSize);
            if (reelItems < iconsOnReel.Count)
            {
                if (iconsOnReel[tryIndex] == ReelIcons.None)
                {
                    int tryNum = Random.Range(1, 7);
                    if (reelValue + SOReferences.Instance.Icons.Values[SOReferences.Instance.Icons.Ranks[tryNum]].PointAmount <= maxReelValue)
                    {
                        iconsOnReel[tryIndex] = SOReferences.Instance.Icons.Ranks[tryNum];
                        reelValue += SOReferences.Instance.Icons.Values[SOReferences.Instance.Icons.Ranks[tryNum]].PointAmount;
                        reelItems += 1;
                    }
                }
            }
            else
            {
                int tryNum = Random.Range(1, 7);
                if (reelValue + SOReferences.Instance.Icons.Values[SOReferences.Instance.Icons.Ranks[tryNum]].PointAmount <= maxReelValue)
                {
                    reelValue -= SOReferences.Instance.Icons.Values[iconsOnReel[tryNum]].PointAmount;
                    iconsOnReel[tryIndex] = SOReferences.Instance.Icons.Ranks[tryNum];
                    reelValue += SOReferences.Instance.Icons.Values[SOReferences.Instance.Icons.Ranks[tryNum]].PointAmount;
                }
            }
        }
    }

    public void UpgradeReelValue(int upgradeVal)
    {
        for (int i = 0; i < reelSize; i++)
        {
            iconsOnReel[i] = ReelIcons.None;
        }
        maxReelValue += upgradeVal;
        reelValue = 0;
        reelItems = 0;
        AddIconsUntilValue();
    }

    public List<ReelIcons> GetIcons(int size)
    {
        List<ReelIcons> icons = new List<ReelIcons>();

        int i = currentIndex;
        while (icons.Count < size)
        {
            icons.Add(iconsOnReel[i]);

            i++;
            i %= iconsOnReel.Count;
        }

        return icons;
    }

    public List<ReelIcons> GetAllIcons()
    {
        return iconsOnReel;
    }

    public void SpinReel(int spinSteps)
    {
        // currentIndex += spinSteps;
        // currentIndex %= iconsOnReel.Count;

        // decrement, because the Reel spins down
        // take the number of steps and modulo it; on a reel size of 4, moving down 5 is the same as moving down 1
        int actualSteps = spinSteps % iconsOnReel.Count;
        currentIndex -= actualSteps;
        if (currentIndex < 0)
        {
            currentIndex += iconsOnReel.Count;
        }
    }

    public void UpgradeReelSize(int extraSize)
    {
        for (int i = 0; i < extraSize; i++)
        {
            iconsOnReel.Add(ReelIcons.None);
        }
    }

    public void UpgradeReelIcon()
    {
        HashSet<int> selectedIndices = new HashSet<int>();
        int index = Random.Range(0, iconsOnReel.Count);

        List<ReelIcons> ranks = SOReferences.Instance.Icons.Ranks;

        // while not all indices have been checked, and current index points at no icon or max rank icon
        while
        (
            selectedIndices.Count < iconsOnReel.Count &&
            (iconsOnReel[index] == ReelIcons.None || iconsOnReel[index] == ranks[^1])
        )
        {
            selectedIndices.Add(index);

            while (selectedIndices.Contains(index))
                index = Random.Range(0, iconsOnReel.Count);
        }

        if (selectedIndices.Count == iconsOnReel.Count)
        {
            Debug.LogError("Can't upgrade any more icons!");
            return;
        }

        int currentRank = ranks.IndexOf(iconsOnReel[index]);
        iconsOnReel[index] = ranks[currentRank + 1];
    }

    public void UpgradeReelMultiplier()
    {
        multiplier++;
    }

    public override string ToString()
    {
        string iconString = "";

        foreach (var icon in IconsOnReel)
        {
            iconString += icon;
        }

        return iconString;
    }
}