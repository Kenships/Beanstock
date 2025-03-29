using DamageManagement;
using Events.Channels;
using Events.Listeners;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthVIsual : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] HealthManager healthManager;
    void Start()
    {
        healthManager.OnHealthSet.onEventRaised += SetHealth;
        healthManager.OnDie.onEventRaised += Hide;
    }

    private void Hide(GameObject arg0)
    {
        gameObject.SetActive(false);
    }

    private void SetHealth(float health)
    {
        // Ensure the value is clamped between 0 and 1.
        
        Debug.Log(health);
        float healthPercent = healthManager.GetHealth()/healthManager.GetMaxHealth();
        healthPercent = Mathf.Clamp01(healthPercent);
        fillImage.fillAmount = healthPercent;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
