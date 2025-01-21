using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy Enemy = collision.collider.GetComponent<Enemy>();
        if (Enemy != null)
        {
            Enemy.TakeDamage(Damage);
        }
    }
}
