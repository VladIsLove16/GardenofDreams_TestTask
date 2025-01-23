using UnityEngine;

public class EnemyVisionZone : MonoBehaviour
{
    Enemy Enemy;
    private void Awake()
    {
        Enemy = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterLogicController characterLogicController = collision.GetComponent<CharacterLogicController>();
        if(characterLogicController!=null)
        {
            Enemy.SetTarget(characterLogicController.transform);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        CharacterLogicController characterLogicController = collision.GetComponent<CharacterLogicController>();
        if (characterLogicController != null)
        {
            Enemy.LoseTarget(characterLogicController.transform);
        }
    }
}