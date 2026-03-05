using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Inspector States
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 7f;
    [Tooltip("Qué tan rápido acelera")]
    [SerializeField] float acceleration = 8f;
    [Tooltip("Qué tan rápido desacelera")]
    [SerializeField] float deceleration = 10f;

    #endregion

    #region Private States
    Vector2 movement;

    float currentSpeed;
    float targetSpeed;
    bool isRunning;
    #endregion

    #region References
    Rigidbody2D rb;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetSpeed = walkSpeed;
        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        InputManager.OnMoveEvent += SetMovement;
        InputManager.OnRunEvent += SetRunState;
    }

    private void OnDisable()
    {
        InputManager.OnMoveEvent -= SetMovement;
        InputManager.OnRunEvent -= SetRunState;
    }

    private void FixedUpdate()
    {
        HandleSpeed();
        ApplyMovement();
    }

    #region Movement Logic
    void HandleSpeed()
    {
        float rate = isRunning ? acceleration : deceleration;

        float speedTarget = movement == Vector2.zero ? 0f : targetSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, speedTarget, rate * Time.fixedDeltaTime);
    }

    void ApplyMovement()
    {
        rb.linearVelocity = movement * currentSpeed;
    }

    void SetMovement (Vector2 input)
    {
        movement = input.normalized;
    }
    void SetRunState (bool runing)
    {
        isRunning = runing;
        targetSpeed = isRunning ? runSpeed : walkSpeed;
    }
    #endregion
}
