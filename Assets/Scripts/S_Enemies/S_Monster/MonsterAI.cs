using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    //references
    private Transform playerTransform;
    private UnityEngine.AI.NavMeshAgent agent;
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

    //attacks
    [Header("Attacks")]
    [SerializeField] private MonsterAttackHitbox leftArmHitbox;
    [SerializeField] private MonsterAttackHitbox rightArmHitbox;
    [SerializeField] private MonsterAttackHitbox bodyHitbox;
    [SerializeField] private float smashDuration = 1.2f;      // hele animationens længde
    [SerializeField] private float smashHitDelay = 0.4f;      // tid før armen begynder at skade
    [SerializeField] private float smashHitWindow = 0.4f;     // hvor længe armen skader
    [SerializeField] private float rollDuration = 2f;
    [SerializeField] private float rollSpeed = 6f;
    [SerializeField] private float pauseAfterAttack = 3f;   // tid monsteret står stille efter et angreb
    private bool isAttacking;
    private Coroutine attackRoutine;

    //patrolling
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Coroutine stunRoutine;

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
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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

        //under et angreb styrer attack-coroutinen bevægelsen
        if (isAttacking) return;


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
        //fjenden står stille og retter sig imod spilleren
        agent.ResetPath();
        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (attackTimer <= 0f && attackRoutine == null) // pausen er overstået, vælg et tilfældigt angreb
        {
            attackRoutine = StartCoroutine(AttackRoutine(Random.Range(0, 3)));
        }
    }

    private System.Collections.IEnumerator AttackRoutine(int attackType)
    {
        isAttacking = true;
        agent.ResetPath();

        if (attackType == 0 || attackType == 1)
        {
            bool left = attackType == 0;
            MonsterAttackHitbox hitbox = left ? leftArmHitbox : rightArmHitbox;
            animator.SetTrigger(left ? "SmashLeft" : "SmashRight");

            yield return new WaitForSeconds(smashHitDelay);
            if (hitbox != null) hitbox.Activate(attackDamage, transform);
            yield return new WaitForSeconds(smashHitWindow);
            if (hitbox != null) hitbox.Deactivate();
            yield return new WaitForSeconds(Mathf.Max(0f, smashDuration - smashHitDelay - smashHitWindow));
        }
        else
        {
            animator.SetBool("IsRolling", true);
            if (bodyHitbox != null) bodyHitbox.Activate(attackDamage, transform);

            float oldSpeed = agent.speed;
            agent.speed = rollSpeed;
            float t = 0f;
            while (t < rollDuration)
            {
                if (playerTransform != null) agent.SetDestination(playerTransform.position);
                t += Time.deltaTime;
                yield return null;
            }

            agent.speed = oldSpeed;
            agent.ResetPath();
            if (bodyHitbox != null) bodyHitbox.Deactivate();
            animator.SetBool("IsRolling", false);
        }

        // står stille (isAttacking blokerer chase/patrol) før næste angreb kan vælges
        agent.ResetPath();
        yield return new WaitForSeconds(pauseAfterAttack);

        isAttacking = false;
        attackRoutine = null;
    }

    private void OnDisable()
    {
        if (leftArmHitbox != null) leftArmHitbox.Deactivate();
        if (rightArmHitbox != null) rightArmHitbox.Deactivate();
        if (bodyHitbox != null) bodyHitbox.Deactivate();
        isAttacking = false;
        attackRoutine = null;
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

    public void Stun(float duration)
    {
        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    private System.Collections.IEnumerator StunRoutine(float duration)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
        stunRoutine = null;
    }
}
