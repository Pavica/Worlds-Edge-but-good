
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public float baseMaxHealth;
    float maxHealth;
    float health;
    public float baseDamage;
    [HideInInspector]
    public float damage;
    public int baseXp;
    int xp;
    int level;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    public enum AttackType {range, close};
    public AttackType type;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Rigidbody rb;

    public Animator attack;

    Vector3 velocity = Vector3.zero;

    //Player and Enemy Health / Level
    public HealthBar healthBar;
    public PauseMenu pauseMenu;

    public HealthBar healthBarPlayer;
    public XpBar xpBarPlayer;
    public TextMeshProUGUI levelText;

    public GameObject quest;

    private void Start()
    {
        player = GameObject.Find("RigidBodyFPSController").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.updatePosition = false;
        DistanceLevelScale levelRange = GameObject.Find("DistanceBar").GetComponent<Distance>().currentLevelRange;

        if(levelRange != null)
        {
            level = UnityEngine.Random.Range(levelRange.getStartValue(), levelRange.getEndValue() + 1);
        }
        
        setStats(level);
        health = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    private void FixedUpdate()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void setStats(int level)
    {
        levelText.SetText("Lvl. " + level);
        maxHealth = (float)Math.Round( Mathf.Log(5 + level-1, 5) * baseMaxHealth,1);
        health = maxHealth;
        damage = (float)Math.Round(Mathf.Log(5 + level - 1, 5) * baseDamage,1);
        xp = Mathf.RoundToInt(Mathf.Log(8 + level - 1,8) * baseXp);
    }

    private void Patroling()
    {
        //if (!walkPointSet) SearchWalkPoint();

        // if (walkPointSet)
        //    agent.SetDestination(walkPoint);

        // Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        // if (distanceToWalkPoint.magnitude < 1f)
        //    walkPointSet = false;

        //disable everything agent related
        if (GetComponent<NavMeshAgent>().isActiveAndEnabled && GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            GetComponent<NavMeshAgent>().updatePosition = false;
            GetComponent<NavMeshAgent>().updateRotation = false;
            //this is the most important part; turn the agent off:
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<NavMeshAgent>().updatePosition = true;
        GetComponent<NavMeshAgent>().updateRotation = true;

        
        if (GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            agent.SetDestination(player.transform.position);
        }

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
            if(type == AttackType.close)
            {
                closeAttack();
            }
            else
            {
                rangeAttack();
            }
            
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void rangeAttack()
    {
        Rigidbody rb = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        player.GetComponent<RigidbodyFirstPersonController>().attackingEnemyDamage = damage;
    }

    private void closeAttack()
    {
        attack.SetTrigger("attack");
        player.GetComponent<RigidbodyFirstPersonController>().TakeDamage(damage);
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
        healthBar.setHealth(health);
    }
    private void DestroyEnemy()
    {
        //Quest 
        Quest questScript = quest.GetComponent<Quest>();

        if(gameObject.name == "EnemyLegs(Clone)" && questScript.currentQuest.getEnemyType() == "Legs" &&  level >= questScript.currentQuest.getLevel()  ||
            gameObject.name == "EnemyShark(Clone)" && questScript.currentQuest.getEnemyType() == "Sharks" && level >= questScript.currentQuest.getLevel() ||
            gameObject.name == "EnemySlime(Clone)" && questScript.currentQuest.getEnemyType() == "Slimes" && level >= questScript.currentQuest.getLevel()
            )
        {
            questScript.currentKills++;
            questScript.slider.value = questScript.currentKills;
            questScript.questAmountText.SetText( questScript.currentKills + " / " + questScript.currentQuest.getAmountOfKills());
        }

        //Destroy
        Destroy(gameObject);
        xpBarPlayer.addXP(xp);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void getHit()
    {
        Debug.Log("Player hit the enemey");
        StartCoroutine(changeColorAfterHit(0.25f));
        TakeDamage(player.GetComponent<RigidbodyFirstPersonController>().damage);
        Debug.Log("Enemy HP:" + health);
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
