using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //references
    private Transform playerTransform;
    private NavMeshAgent agent;

    //values
    private float attackRange = 0.8f;
    private float attackTimer;
    private float attackCooldown = 1f;
    private float chaseRange = 5f;

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
    [SerializeField] private aiStates currentState = aiStates.Patrol;


    //start-metode finder referencerne til player og navmeshagenten
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
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
        float patrolScore = 0.1f;

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
        float bestScore = Mathf.Max(attackScore, chaseScore, patrolScore);


        // *BESLUTNING* //
        if (bestScore == attackScore || attackScore > chaseScore && attackScore > patrolScore) // ATTACK
        {
            AttackPlayer();
            currentState = aiStates.Attack;
        }
        else if(bestScore == chaseScore || chaseScore > attackScore && chaseScore > patrolScore) // CHASE
        {
            ChasePlayer();
            currentState = aiStates.Chase;
        }
        else // PATROL
        {
            Patrol();
            currentState = aiStates.Patrol;
        }
    }

    private void Patrol()
    {
        if(patrolPoints.Length == 0) return; // returner hvis der ikke er nogen patrol points

        // sæt destination til det nuværende patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        Debug.Log(gameObject.name + ": patrolling to point " + currentPatrolIndex);

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
        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (Vector3.Distance(transform.position, playerTransform.position) <= 0.5f)
        {
            agent.SetDestination(transform.position); // stop fjenden hvis den er tæt nok på spilleren
        }

        Debug.Log(gameObject.name + ": chasing player");
    }

    private void AttackPlayer()
    {
        //fjenden angriber spilleren statisk og retter sig imod dem
        agent.SetDestination(transform.position);
        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (attackTimer <= 0f) // hvis fjendens cooldowm er overstået kan de angribe
        {
            //ATTACK LOGIC HER (reducere spillerens health)
            Debug.Log(gameObject.name + ": attacking player!");

            attackTimer = attackCooldown; // reset attackTimer til cooldown
        }
    }
}
