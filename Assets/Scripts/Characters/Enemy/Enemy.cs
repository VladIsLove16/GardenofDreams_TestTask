using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using static CharacterLogicController;

public class Enemy  : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float timeBetweenDamageTicks = 1f;
    [SerializeField] private int spawnChance;
    [SerializeField] private ItemGO SpawnOnDeath;
    private Transform target;
    private Vector2 targetPos;
    public Vector2 SpawnPoint;
    private Rigidbody2D rb;
    private bool IsCollidingWithPlayer;
    private float LastTimeDamaged;
    private HealthComponent PlayerHealthComponent ;
    private void Awake()
    {
        GetComponent<HealthComponent>().Died += OnDied;
        SpawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        SetTargetPosition();
        Move();
        HandleRotation();
        HandlePlayerDamage();
    }

    private void SetTargetPosition()
    {
        if (target == null)
            targetPos = SpawnPoint;
        else
            targetPos = target.position;
    }

    private void HandlePlayerDamage()
    {
        if(LastTimeDamaged + timeBetweenDamageTicks > Time.time && IsCollidingWithPlayer)
        {
            LastTimeDamaged = Time.time;
            PlayerHealthComponent?.TakeDamage(damage);
        }
    }

    private void Move()
    {
        Vector2 moveDirection = targetPos - new Vector2( transform.position.x, transform.position.y);
        
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
        else
            rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + moveDirection.normalized * Time.fixedDeltaTime * moveSpeed);
    }
    private void HandleRotation()
    {
        Vector2 direction = targetPos - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnDied()
    {
        if (UnityEngine.Random.Range(0, 100) < spawnChance)
        {
            Instantiate(SpawnOnDeath, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterLogicController characterLogicController = collision.collider.GetComponent<CharacterLogicController>();
        if(characterLogicController!=null)
        {
            HealthComponent  healthComponent =   characterLogicController.GetComponent<HealthComponent>();
            if (healthComponent != null)
            { 
                IsCollidingWithPlayer = true;
                PlayerHealthComponent = healthComponent;
            }

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        CharacterLogicController characterLogicController = collision.collider.GetComponent<CharacterLogicController>();
        if (characterLogicController != null)
        {
            HealthComponent healthComponent = characterLogicController.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                IsCollidingWithPlayer = false;
            }
        }
    }

    internal void SetTarget(Transform transform)
    {
        target = transform;
    }

    internal void LoseTarget(Transform transform)
    {
        target =  null;
    }
}
