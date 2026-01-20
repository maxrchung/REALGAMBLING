using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxIsReel : MonoBehaviour
{
    public List<ReelIcons> icons = new();
    public RectTransform mask;
    public RectTransform content;

    private List<Image> images = new();

    public float scrollSpeed = 200;
    public float iconWidth = 100;
    public int steps = 0;

    private float distance = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetIcons(icons);
        Spin(steps);
    }

    // Update is called once per frame
    void Update()
    {
        var delta = scrollSpeed * Time.deltaTime;
        if (distance <= 0)
        {
            return;
        }

        distance -= delta;
        Vector3 down = delta * Vector3.down;
        float bottomY = -mask.rect.height / 2f - iconWidth / 2f;

        // Move each icon individually
        foreach (var image in images)
        {
            RectTransform rt = image.rectTransform;
            rt.localPosition += down;

            // Wrap when icon goes below mask
            if (rt.localPosition.y < bottomY)
            {
                rt.localPosition += Vector3.up * iconWidth * images.Count;
            }
        }
    }

    public void SetIcons(List<ReelIcons> icons)
    {
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
            images.Clear();
        }
        
        for (int i = 0; i < icons.Count; i++)
        {
            var sprite = SOReferences.Instance.Icons.Values[icons[i]].iconSprite;

            GameObject go = new GameObject("Icon_" + i, typeof(Image));
            go.transform.SetParent(content, false); // 'false' keeps local scale/rotation

            Image image = go.GetComponent<Image>();
            image.sprite = sprite;

            // Set RectTransform size
            RectTransform rt = image.rectTransform;
            rt.sizeDelta = new Vector2(iconWidth, iconWidth);

            // Position vertically
            float yPos = -i * iconWidth + 2 * iconWidth;

            if (i >= 5)
            {
                yPos = yPos + icons.Count * iconWidth;
            }

            rt.localPosition = new Vector3(0, yPos, 0);

            images.Add(image);
        }
    }

    public void Spin(int steps)
    {
        distance = steps * iconWidth;
    }
}
