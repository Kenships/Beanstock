using System;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundPulse : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float pulseSpeed = 5f;
    private SpriteRenderer sr;
    private SpriteRenderer parentsr;
    private Vector3 originalScale;
    private void Awake()
    {
        originalScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        if (transform.parent.TryGetComponent(out SpriteRenderer parentsr))
        {
            this.parentsr = parentsr;
            sr.sprite = parentsr.sprite;
            sr.sortingOrder = parentsr.sortingOrder - 1;
            sr.color = Color.white;
        }
        else
        {
            Debug.LogWarning("BackgroundPulse parent component doesn't have parent SpriteRenderer");
        }
    }

    private void Update()
    {
        Pulse();
    }

    private void Pulse()
    {
        sr.sprite = parentsr.sprite != sr.sprite ? parentsr.sprite : sr.sprite;
        sr.flipX = parentsr.flipX;
        sr.flipY = parentsr.flipY;
        float scaleFactor = (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2; // Normalized oscillation [0,1]
        //float dynamicScale = Mathf.Lerp(1.0f, scaleMultiplier, scaleFactor);
        float dynamicScale = scaleMultiplier;
        transform.localScale = originalScale * dynamicScale;
    }
}