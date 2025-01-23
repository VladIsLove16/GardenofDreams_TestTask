using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    bool DestoryOnTrigger;
    public int Damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy Enemy = collision.GetComponent<Enemy>();
        if (Enemy != null)
        {
            HealthComponent healthComponent = Enemy.GetComponent<HealthComponent>();
            if (healthComponent != null)
            { 
                healthComponent.TakeDamage(Damage);
                if (DestoryOnTrigger)
                    Destroy(gameObject);
            }
        }
    }
}
