using UnityEngine;

public class Enemy   : MonoBehaviour
{
    [SerializeField] private int damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterLogicController characterLogicController = collision.collider.GetComponent<CharacterLogicController>();
        if(characterLogicController!=null)
        {
            characterLogicController.TakeDamage(damage);
        }
    }
    public void TakeDamage(int damage)
    {
        Destroy(gameObject);
    }
}