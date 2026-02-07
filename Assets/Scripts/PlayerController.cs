using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public Animator animator;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        move = Quaternion.Euler(0, -90, 0) * move;

        if (move.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(move);

        controller.Move(move * moveSpeed * Time.deltaTime);

        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool("Running", isMoving);
    }
}
