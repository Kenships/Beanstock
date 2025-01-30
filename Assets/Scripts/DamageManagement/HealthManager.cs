using DamageManagement;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    
    public void Damage(float damage)
    {
        if (health <= 0f)
        {
            health = 0f;
            Die();
        }
        
        health -= damage;
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }
}
