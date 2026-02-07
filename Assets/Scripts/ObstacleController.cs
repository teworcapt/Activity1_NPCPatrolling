using UnityEngine;
using UnityEngine.InputSystem;

public class ObstacleController : MonoBehaviour
{
    public float moveDistance = 4f;
    public float moveSpeed = 3f;

    public enum TriggerAction
    {
        MoveLeftObstacle,
        MoveUpObstacle,
        MoveRightObstacle
    }

    public TriggerAction actionToUse;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool moved;
    private bool moving;

    private PlayerControls controls;

    void Awake()
    {
        startPosition = transform.position;
        targetPosition = startPosition;

        controls = new PlayerControls();

        switch (actionToUse)
        {
            case TriggerAction.MoveLeftObstacle:
                controls.Player.MoveLeft.performed += _ => ToggleMove();
                break;

            case TriggerAction.MoveUpObstacle:
                controls.Player.MoveUp.performed += _ => ToggleMove();
                break;

            case TriggerAction.MoveRightObstacle:
                controls.Player.MoveRight.performed += _ => ToggleMove();
                break;
        }
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            moving = false;
        }
    }

    void ToggleMove()
    {
        if (moving) return;

        if (!moved)
        {
            targetPosition = startPosition + transform.right * moveDistance;
            moved = true;
        }
        else
        {
            targetPosition = startPosition;
            moved = false;
        }

        moving = true;
    }
}
