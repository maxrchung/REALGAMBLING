using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ReelBehavior : MonoBehaviour
{
    [SerializeField] private int iconsOnReel;
    [Space]
    [SerializeField] private GameObject reelGO;
    [SerializeField] private GameObject reelIconGOPrefab;

    private void Start()
    {
        // 360 is the degrees around the circle
        // 360 / iconsOnReel = degrees per icon, starting from 0
        // we need to use sohcahtoa
        // probably need to change scale based on how many icons on reel
        
        float radius = transform.localScale.x * .25f; // based on X scale; incrementing by 1 increases radius by .25f
        float degreesPerIcon = 360f / iconsOnReel;
        
        for (int i = 0; i < iconsOnReel; i++)
        {
            float currentDegrees = degreesPerIcon * i;
            float degreesToRadian = currentDegrees * (Mathf.PI / 180);
            
            float xPos = Mathf.Sin(degreesToRadian);
            float zPos = Mathf.Cos(degreesToRadian);
            
            Debug.Log($"currentDegrees: {currentDegrees}, degreesToRadian: {degreesToRadian}, xPos: {xPos}, zPos: {zPos}");
            
            Vector3 iconPosition = new Vector3(xPos, 0, zPos);
            Quaternion iconRotation = Quaternion.Euler(new Vector3(0, currentDegrees, 0));
            Debug.Log($"reelPosition: {iconPosition}");
            GameObject reelIconGO = Instantiate(reelIconGOPrefab, iconPosition, iconRotation, reelGO.transform);
        }
        
        Debug.Log(reelGO.transform.position);
        Debug.Log(reelGO.transform.localPosition);
    }
    
    // private void Update()
    // {
    //     transform.Rotate(0, -180 * Time.deltaTime, 0);
    // }
}
