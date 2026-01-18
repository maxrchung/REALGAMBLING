using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ItemSelect : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{


    private const float ROTATION_SPEED = 670f;

    private const float SCALE_MULT = 1.1f;

    private Action<GameObject> clickCallback;

    private Transform cameraTransform;

    private Quaternion originalRotation;

    private Vector3 originalScale;

    private Boolean shouldUpdate;

    public void setClickCallback(Action<GameObject> clickCallback)
    {
        this.clickCallback = clickCallback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickCallback == null)
        {
            Debug.LogWarning("No click callback registered.");
            return;
        }
        // delegate
        this.clickCallback(this.gameObject);
    }

    void Start()
    {
        Debug.Log("Starting");
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found.");
            return;
        }
        this.cameraTransform = mainCamera.transform;
    }

    void Update()
    {
        if (shouldUpdate)
        {
            transform.Rotate(-cameraTransform.right, ROTATION_SPEED * Time.deltaTime, Space.World);
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        shouldUpdate = true;
        transform.localScale *= SCALE_MULT;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shouldUpdate = false;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }
}
