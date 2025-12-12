using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Vector3 offset;
    public Transform target;

    private Color highHealthColor = Color.green;
    private Color mediumHealthColor = Color.yellow;
    private Color lowHealthColor = Color.red;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target != null && mainCamera != null)
        {
            Vector3 worldPosition = target.position + offset;
            
            transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            UpdateHealthBarColor();
        }
    }

    public void SetHealth(int currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            UpdateHealthBarColor();
        }
    }

    private void UpdateHealthBarColor()
    {
        if (fillImage == null || healthSlider == null)
        {
            return;
        }

        float healthPercent = healthSlider.value / healthSlider.maxValue;

        if (healthPercent > 0.6f)
        {
            fillImage.color = highHealthColor;
        }
        else if (healthPercent > 0.3f)
        {
            fillImage.color = mediumHealthColor;
        }
        else
        {
            fillImage.color = lowHealthColor;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
