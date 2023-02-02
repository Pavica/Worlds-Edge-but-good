
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float maxHealth;
    float health;
    public float damage;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Rigidbody rb;


    //AI motion
    [SerializeField] private int speed;

    Vector3 velocity = Vector3.zero;

    public HealthBar healthBar;

    private void Start()
    {
        player = GameObject.Find("RigidBodyFPSController").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.updatePosition = false;

        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (agent.speed == 0)
        {
            agent.speed = speed;
        }
        agent.SetDestination(player.transform.position);


        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            ///Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        //Destroy previous obect
        GameObject projectileShot = GameObject.Find("rock(Clone)");
        Destroy(projectileShot);
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
        healthBar.SetHealth(health);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            Debug.Log("Player hit the enemey");
            //add knockback somehow (very hard)
            StartCoroutine(changeColorAfterHit(0.25f));
            TakeDamage(player.GetComponent<RigidbodyFirstPersonController>().damage);
            Debug.Log("Enemy HP:" + health);
        }
    }

    IEnumerator changeColorAfterHit(float time)
    {
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSecondsRealtime(time);
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //enemies hit each other
        if (collision.gameObject.tag == "rock")
        {
            Debug.Log("Enemy hit another enemy");
            TakeDamage(damage);

            Debug.Log("Enemy HP:" + health);
        }
    }
}
