using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class CharacterLogicController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float rotationSpeed = 5f; 
    public float attackRange = 5f; 
  
    public GameObject projectilePrefab; 
    public Transform firePoint;
    [SerializeField]
    private Transform projectileOwner;
    
    public LayerMask enemyLayer;

    public LayerMask itemLayer; 

    private Vector2 movementDirection;

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
        transform.rotation = Quaternion .Euler (0, 0, angle);
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
            rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + movementDirection.normalized * Time.fixedDeltaTime * moveSpeed);
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
            Vector2 direction = closestEnemy.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

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
