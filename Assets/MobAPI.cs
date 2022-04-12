using System;
using Cinemachine.Utility;
using DefaultNamespace;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class MobAPI : MonoBehaviour, Creature
{
    // Variables
    [SerializeField] private Vector3 _offset = Vector3.up;
    [SerializeField] private float turningDuration = 7f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpHeight = 1f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2? _landingDirection;
    private Quaternion lookAngle = Quaternion.identity;
    private bool isGrounded = false;
    private int jumpState = 0;
    private Vector3 _previousDirection = Vector3.zero;
    private Vector3 finalDirection = Vector3.zero;

    // References
    private CharacterController controller;
    private Animator animator;
    [CanBeNull] private GameObject _spirit;

    // Properties
    public GameObject spirit
    {
        get => _spirit;
        set
        {
            _spirit = value;
            lastOccupied = DateTime.Now;
            if (value)
            {
                var scale = transform.localScale;
                var sequence = DOTween.Sequence();
                sequence.Append(transform.DOScale(scale * 1.5f, 0.5f).SetEase(Ease.InQuart));
                sequence.Append(transform.DOScale(scale, 1.6f).SetEase(Ease.InQuart));
            }

            moveDirection = Vector3.zero;
            finalDirection = Vector3.zero;
        }
    }

    // Animation keys
    private readonly int animationIdSpeed = Animator.StringToHash("Speed");
    private readonly int animationIdAttack = Animator.StringToHash("Attack");

    public Vector3 offset => _offset;
    public DateTime lastOccupied { get; private set; }

    public void Move(Vector2 direction)
    {
        if (!isGrounded)
        {
            _landingDirection = direction;
            return;
        }

        moveDirection = new Vector3(direction.x, moveDirection.y, direction.y);
        EvaluateDirection();
    }

    public void MoveTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.Euler(direction);
        finalDirection = direction;
    }

    public void Look(Quaternion lookAngle)
    {
        this.lookAngle = lookAngle;
        EvaluateDirection();
    }

    public void Attack()
    {
        animator.SetTrigger(animationIdAttack);
    }

    private void HandleSpeedChanges()
    {
        if ((finalDirection.ProjectOntoPlane(Vector3.up).magnitude <= 0.1f) !=
            (_previousDirection.ProjectOntoPlane(Vector3.up).magnitude <= 0.1f))
            animator.SetFloat(animationIdSpeed,
                finalDirection.ProjectOntoPlane(Vector3.up).magnitude != 0f ? 0.25f : 0f);
        _previousDirection = finalDirection;
    }

    private void EvaluateDirection()
    {
        var eulerAnglesY = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg
                           + lookAngle.eulerAngles.y;
        var vector = Vector3.forward;
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            transform.DORotateQuaternion(Quaternion.Euler(0f, eulerAnglesY, 0f), turningDuration);
            vector = Quaternion.Euler(0f, eulerAnglesY, 0f) * Vector3.forward;
        }
        else
            vector = Vector3.zero;

        finalDirection = vector.normalized * speed;
    }

    public void MoveVertically(float direction)
    {
        if (direction <= 0f)
            return;
        if (!isGrounded)
            return;
        _landingDirection = new Vector2(moveDirection.x, moveDirection.z);
        finalDirection.y = Mathf.Sqrt(jumpHeight * -3f * Physics.gravity.y);
        jumpState = 1;
    }

    private void Movement()
    {
        HandleSpeedChanges();

        if (isGrounded && jumpState == -1)
        {
            if (_landingDirection != null)
            {
                Move(_landingDirection.Value);
                _landingDirection = null;
            }

            finalDirection.y = 0f;
            jumpState = 0;
        }

        if (finalDirection.y > 0)
        {
            controller.Move(finalDirection.y * transform.up * Time.deltaTime);
            finalDirection += Physics.gravity * Time.deltaTime;
        }
        else if (jumpState == 1)
        {
            jumpState = -1;
            finalDirection.y = 0;
        }

        controller.SimpleMove(finalDirection);
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        Movement();
    }

    private bool CheckIfGrounded() => controller.isGrounded || Physics.Raycast(
        transform.position,
        -transform.up,
        controller.height / 2f + 0.2f
    );

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.AddForce(pushDir * speed, ForceMode.Impulse);
    }
}