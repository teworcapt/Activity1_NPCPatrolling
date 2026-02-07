using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class PatrolController : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;
    public Transform player;
    public Transform eyePoint;
    public TextMeshProUGUI chaseTimerText;
    public TextMeshProUGUI detectionText;

    [Header("Vision Settings")]
    public float viewRadius = 6f;
    public float viewAngle = 90f;

    [Header("Chase Settings")]
    public float loseSightDuration = 2f;

    [Header("Patrol Settings")]
    public float waitTimeAtPoint = 2f;

    private float loseSightTimer = 0f;
    private bool chasing = false;
    private int currentPointIndex = 0;
    private Transform[] patrolPoints;

    private float viewThreshold;
    private float waitTimer = 0f;
    private bool waiting = false;

    void Start()
    {
        viewThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        patrolPoints = new Transform[] { pointA, pointB, pointC, pointD };

        agent.SetDestination(patrolPoints[0].position);

        chaseTimerText.text = "";
        detectionText.text = "";
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            chasing = true;
            loseSightTimer = loseSightDuration;
            agent.SetDestination(player.position);

            chaseTimerText.text = loseSightTimer.ToString("F1");
            detectionText.text = "Detected!";
        }
        else if (chasing)
        {
            loseSightTimer -= Time.deltaTime;

            if (loseSightTimer <= 0f)
            {
                chasing = false;
                chaseTimerText.text = "";
                detectionText.text = "";
                GoToNextPoint();
            }
            else
            {
                agent.SetDestination(player.position);
                chaseTimerText.text = loseSightTimer.ToString("F1");
                detectionText.text = "Detected!";
            }
        }
        else
        {
            PatrolLogic();
            detectionText.text = "";
        }
    }


    void PatrolLogic()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                GoToNextPoint();
            }
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (agent.velocity.sqrMagnitude == 0f && !agent.hasPath && !agent.pathPending)
            {
                waiting = true;
                waitTimer = waitTimeAtPoint;
            }
        }
    }

    void GoToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    bool CanSeePlayer()
    {
        Collider col = player.GetComponent<Collider>();
        Vector3 targetPoint = col.bounds.center;
        Vector3 dir = (targetPoint - eyePoint.position);
        float dist = dir.magnitude;

        if (dist > viewRadius)
            return false;

        Vector3 dirNorm = dir.normalized;
        float dot = Vector3.Dot(transform.forward, dirNorm);

        if (dot < viewThreshold)
            return false;

        if (Physics.Raycast(eyePoint.position, dirNorm, out RaycastHit hit, viewRadius)) 
        { if (hit.collider == col) 
                return true; 
        }

        return false;
    }


    void OnDrawGizmos()
    {
        if (eyePoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePoint.position, viewRadius);

        Vector3 forward = transform.forward;
        float halfAngle = viewAngle * 0.5f;

        Quaternion leftRot = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Vector3 leftDir = leftRot * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(eyePoint.position, leftDir * viewRadius);

        Quaternion rightRot = Quaternion.AngleAxis(halfAngle, Vector3.up);
        Vector3 rightDir = rightRot * forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(eyePoint.position, rightDir * viewRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(eyePoint.position, forward * viewRadius);
    }


}
