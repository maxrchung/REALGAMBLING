using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReel : MonoBehaviour
{
    public MaxIsReel maxIsReel;

    public List<Image> debugImages;

    public void SetDebugImages(List<ReelIcons> icons)
    {
        if (!debugImages[0].gameObject.activeSelf)
        {
            return;
        }
        
        if (icons.Count != 5)
        {
            Debug.LogError($"Need exactly 5 images, {icons.Count} passed in");
            return;
        }

        for (int i = 0; i < icons.Count; i++)
        {
            debugImages[i].sprite = SOReferences.Instance.Icons.Values[icons[i]].iconSprite;
        }
    }
    
    public void SetIcons(List<ReelIcons> icons)
    {
        maxIsReel.SetIcons(icons);
    }

    public void Spin(int steps)
    {
        maxIsReel.Spin(steps);
    }
}
