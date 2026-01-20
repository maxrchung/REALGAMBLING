using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReel : MonoBehaviour
{
    public MaxIsReel maxIsReel;

    public void SetIcons(List<ReelIcons> icons)
    {
        maxIsReel.SetIcons(icons);
    }

    public void Spin(int steps)
    {
        maxIsReel.Spin(steps);
    }
}
