using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //references
    private Transform playerTransform;
    private NavMeshAgent agent;
    private PlayerHeath playerHealth;
    private float playerLookupTimer;

    //values
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    private float attackTimer;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float chaseRange = 20f;
    [SerializeField] private float speed = 1.5f;

    [SerializeField] Animator animator;

    //patrolling
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    //states
    private enum aiStates
    {
        Patrol,
        Chase,
        Attack
    }
    //start-metode finder referencerne til player og navmeshagenten
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length); // start ved et tilfældigt patrol point
        }

        TryFindPlayer();
    }

    void Update()
    {
        playerLookupTimer -= Time.deltaTime;
        if ((playerTransform == null || playerHealth == null) && playerLookupTimer <= 0f)
        {
            TryFindPlayer();
            playerLookupTimer = 0.5f;
        }

        //returner hvis spilleren ikke er fundet
        if (playerTransform == null) return;

        //tæller ned
        attackTimer -= Time.deltaTime;


        // *FORBEREDELSE TIL UDREGNING* //
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position); // afstand til spilleren

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized; // retning til spilleren via retningsvektor

        //prik-produkt = 1: spilleren er direkte foran fjenden,
        //prik-produkt = 0: spilleren er til siden,
        //prik-produkt = -1: spilleren er direkte bag fjenden.
        float dotAngleToPlayer = Vector3.Dot(transform.forward, directionToPlayer); // vinkel mellem fjendens fremadgående retning og retningen til spilleren 


        // *UDREGNING* //
        float attackScore = 0f;
        float chaseScore = 0f;
        float patrolScore = 1f;

        if(distanceToPlayer <= attackRange) // hvis spilleren er inden for attack-range, tilføres attackScore en høj værdi baseret på afstanden og vinklen til spilleren
        {
            attackScore = (attackRange - distanceToPlayer) + (100f * dotAngleToPlayer);
        }

        if(distanceToPlayer <= chaseRange) // hvis spilleren er inden for chase-range, tilføres chaseScore en værdi baseret på afstanden og vinklen til spilleren
        {
            chaseScore = (chaseRange - distanceToPlayer) + (5f * dotAngleToPlayer);
        }
        
        if(distanceToPlayer > chaseRange) // hvis spilleren er uden for chase-range, tilføres patrolScore en høj værdi 
        {
            patrolScore = 100f;
        }

        //bedste score findes ved at sammeligne og finde den højeste værdi;
        int bestScore = (int)(Mathf.Max(attackScore, chaseScore, patrolScore));

        //Debug.Log("Attack Score: "+ attackScore + " | Chase Score: " + chaseScore + " | Patrol Score: " + patrolScore + " | Best Score: " + bestScore);

        // *BESLUTNING* //
        if (bestScore == (int)attackScore) // ATTACK
        {
            AttackPlayer();
        }
        else if(bestScore == (int)chaseScore) // CHASE
        {
            ChasePlayer();
        }
        else // PATROL
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if(patrolPoints.Length == 0) return; // returner hvis der ikke er nogen patrol points

        // sæt destination til det nuværende patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        //Debug.Log(gameObject.name + ": patrolling to point " + currentPatrolIndex);

        // afstand til det nuværende patrol point
        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);

        //er fjenden tæt nok på det nuværende patrol point, gå videre til det næste
        if (distanceToPatrolPoint < 0.75f)
        {
            currentPatrolIndex++;

            // hvis det sidste patrol point er nået, start forfra
            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }
        }
    }

    private void ChasePlayer()
    {
        //sæt destination/orientering til spilleren
        agent.SetDestination(playerTransform.position);
        agent.speed = speed;
        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        //Debug.Log(gameObject.name + ": chasing player");
    }

    private void AttackPlayer()
    {
        //fjenden angriber spilleren statisk og retter sig imod dem
        agent.SetDestination(playerTransform.position + new Vector3(1.5f,1.5f,1.5f));
        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (attackTimer <= 0f) // hvis fjendens cooldowm er overstået kan de angribe
        {
            //ATTACK LOGIC HER (reducere spillerens health)
            Debug.Log(gameObject.name + ": attacking player!");
            if (playerHealth != null)
            {
                animator.SetBool("IsAttacking", true);
                playerHealth.TakeDamage(attackDamage, transform.position); // eksempel på at reducere spillerens health med 10
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }

            attackTimer = attackCooldown; // reset attackTimer til cooldown
        }
    }

    private void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            playerTransform = null;
            playerHealth = null;
            return;
        }

        playerTransform = player.transform;
        playerHealth = player.GetComponent<PlayerHeath>();
    }
}
