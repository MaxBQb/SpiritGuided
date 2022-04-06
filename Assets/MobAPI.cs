using System;
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
    [SerializeField] private float jumpSpeed = 1f;
    private Vector3 moveDirection = Vector3.zero;
    private float _verticalMove = 0f;
    private Quaternion lookAngle = Quaternion.identity;
    private Vector3 jumpDirection = Vector3.zero;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool inAir = false;
    private float airControl = 0.5f;
    private bool jumpClimax = false;
    // private ContactPoint _contact;
    private float turnSmoothVelocity = 0f;
    private Vector3 _previousDirection = Vector3.zero;
    private Vector3 finalDirection = Vector3.zero;
    
    // References
    private CharacterController controller;
    private Rigidbody _rigidbody;
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

    void OnCollisionStay(Collision collisionInfo)
    {
        // _contact = collisionInfo.contacts[0];
        // if (inAir || isJumping)
        //     _rigidbody.AddForceAtPosition(-_rigidbody.velocity, _contact.point);
    }

    private void HandleSpeedChanges()
    {
        if ((finalDirection.magnitude <= 0.1f) != (_previousDirection.magnitude <= 0.1f))
            animator.SetFloat(animationIdSpeed, finalDirection.magnitude != 0f ? 0.25f : 0f);
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
        finalDirection = vector.normalized;
    }

    public void MoveVertically(float direction)
    {
        if (direction < 0f)
            return;
        if (!isGrounded) 
            return;
        isJumping = true;
        jumpDirection = moveDirection;
        // _rigidbody.AddForce(transform.up * jumpSpeed * _rigidbody.mass);
        EvaluateDirection();
    }


    private void Movement()
    {
        HandleSpeedChanges();
        controller.SimpleMove(finalDirection * speed);
        // if (isGrounded)
        // else if (isJumping || inAir)
        //     transform.Translate((jumpDirection * speed * airControl) * Time.deltaTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        // animator.speed = 1f;
        // animator.SetFloat(animationIdSpeed,  0.4f);
        // _collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        // if (!isGrounded)
        // {
        //     if (Physics.Raycast(transform.position, -transform.up, _collider.height / 2 + 0.2f))
        //     {
        //         isGrounded = true;
        //         isJumping = false;
        //         inAir = false;
        //         jumpClimax = false;
        //     }
        //     else if (!inAir)
        //     {
        //         inAir = true;
        //         jumpDirection = moveDirection;
        //     }
        //     else if (inAir && _rigidbody.velocity.y == 0.0) 
        //     {
        //         jumpClimax = true;
        //     }
        // }

        Movement();
    }
    
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

    void OnCollisionExit(Collision collisionInfo)
    {
        isGrounded = false;
    }
}
