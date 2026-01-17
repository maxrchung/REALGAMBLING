using System.Collections.Generic;
using UnityEngine;

public class SlotMachineLogic : MonoBehaviour
{
    public List<Reel> activeReels;
    public List<ReelIcons> spinResult;
    
    private void Start()
    {
        spinResult = new List<ReelIcons>();
        activeReels = new List<Reel>();

        Reel reelOne = new Reel();
        Reel reelTwo = new Reel();
        Reel reelThree = new Reel();

        activeReels = new List<Reel>()
        {
            reelOne, reelTwo, reelThree
        };

        reelOne.reelIcons = new List<ReelIcons>()
        {
            ReelIcons.Bread, ReelIcons.None, ReelIcons.Fish, ReelIcons.None, ReelIcons.Snake, ReelIcons.None
        };

        reelTwo.reelIcons = new List<ReelIcons>()
        {
            ReelIcons.Bread, ReelIcons.Fish, ReelIcons.Fish, ReelIcons.None, ReelIcons.Snake, ReelIcons.Bread
        };

        reelThree.reelIcons = new List<ReelIcons>()
        {
            ReelIcons.Fish, ReelIcons.None, ReelIcons.Snake, ReelIcons.SixSeven, ReelIcons.Bread, ReelIcons.Bread
        };
    }

    public void Spin()
    {
        spinResult.Clear();

        foreach (Reel reel in activeReels)
        {
            int reelIconSize = reel.reelIcons.Count;
            int resultIndex = Random.Range(0, reelIconSize);
            
            while (reel.reelIcons[resultIndex] == ReelIcons.None)
            {
                resultIndex++;
                resultIndex %= reelIconSize;
            }
            
            spinResult.Add(reel.reelIcons[resultIndex]);
        }

        PrintResult();
    }

    public void PrintResult()
    {
        string result = "Result: | ";
        Debug.Log(spinResult.Count);
        foreach (ReelIcons icon in spinResult)
        {
            result += icon + " | ";
        }
        Debug.Log(result);
    }
}
