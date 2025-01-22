using UnityEngine;
public class CharacterLogicController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float rotationSpeed = 5f; 
    public float attackRange = 5f; 
    public float health = 100f; 
    public float maxHealth = 100f; 
    public GameObject projectilePrefab; 
    public Transform firePoint;
    [SerializeField]
    private Transform projectileOwner;
    
    public LayerMask enemyLayer;

    public LayerMask itemLayer; 

    private Vector2 movementDirection; 
    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged HealthChanged;

    [SerializeField]
    InventoryController inventoryController;

    public delegate void OnItemPickedUp(GameObject item);
    public event OnItemPickedUp ItemPickedUp;

    private GameObject closestEnemy;

    [SerializeField]
    private float BulletSpeed = 5f;

    Rigidbody2D rb;
    public void SetMovementDirection(Vector2 direction)
    {
        movementDirection = direction;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        HandleShooting();
        HandleMovement();
        HandleRotation();
        HandleAiming();
    }

    private void HandleRotation()
    {
        Vector2 direction;
        if (closestEnemy != null)
        {
             direction = closestEnemy.transform.position - transform.position;
        }
        else
        {
            if(movementDirection.magnitude <= 0.01f)
            {
                return;
            }
            direction = movementDirection;
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HandleShooting()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }
    }

    void HandleMovement()
    {
        if (movementDirection.magnitude > 0)
        {
            Vector2 moveDirection = new Vector2(movementDirection.x, movementDirection.y);
            rb.AddForce(moveDirection.normalized * moveSpeed);
        }
        else
            rb.velocity = Vector2.zero;
    }

    void HandleAiming()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        if (enemiesInRange.Length > 0)
        {
            closestEnemy = enemiesInRange[0].gameObject;

            Vector3 direction = closestEnemy.transform.position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        }
    }
    public void Shoot()
    {
        if(closestEnemy!=null)
            ShootAtEnemy(closestEnemy);
        else
        {
            Vector2 shootDirection = firePoint.transform.position - transform.position;
            ShootAtPosition(shootDirection + new Vector2( firePoint.transform.position.x, firePoint.transform.position.y));
        }
    }
    public InventoryController GetInventoryController()
    {
        return inventoryController;
    }
    private void ShootAtPosition(Vector2 position)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, projectileOwner);
            var direction = position - new Vector2(firePoint.transform.position.x, firePoint.transform.position.y);
            projectile.GetComponent<Rigidbody2D>().AddForce(direction.normalized * BulletSpeed);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(projectile,5f);
        }
    }
    void ShootAtEnemy(GameObject enemy)
    {
        if(enemy == null)
        {
            Debug.Log("No target to shoot");
            return;
        }
        ShootAtPosition(enemy.transform.position);
    }

    

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        HealthChanged?.Invoke(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Персонаж умер!");
    }

    //internal bool TryPickupItem(ItemDetails itemDetails)
    //{
    //    if (inventoryController.HaveSpace(itemDetails))
    //    {
    //        PickupItem(itemDetails);
    //        return true;
    //    }
    //    else
    //        return false;
    //}
    //private void PickupItem(ItemDetails itemDetails)
    //{
    //    inventoryController.AddItem(itemDetails);
    //}
}
