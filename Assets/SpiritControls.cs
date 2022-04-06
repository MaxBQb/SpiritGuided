using System;
using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;

public class SpiritControls : MonoBehaviour, Creature
{
    // Variables
    [SerializeField] private float speed = 120.0f;
    [SerializeField] private float lifeAttractionRadius = 4.35f;
    [SerializeField] private float lifeAttractionSpeed = 0.5f;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion lookAngle = Quaternion.identity;
    private Vector3 finalDirection = Vector3.zero;
    
    // References
    private CharacterController controller;
    [CanBeNull] private GameObject _spirit;

    // Properties
    public Rigidbody _rigidbody;
    public Vector3 offset => Vector3.zero;
    public DateTime lastOccupied { get; private set; }
    
    // Events
    public event Action<GameObject> creatureContact;
    
    public void Move(Vector2 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.y);
        EvaluateDirection();
    }

    public void Look(Quaternion angle)
    {
        lookAngle = angle;
        EvaluateDirection();
    }

    public void Attack()
    {
    }

    public void MoveVertically(float direction)
    {
        moveDirection.y = direction;
        EvaluateDirection();
    }

    private void EvaluateDirection()
    {
        var eulerAnglesY = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg
                           + lookAngle.eulerAngles.y;
        var vector = Vector3.forward;
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            transform.rotation = Quaternion.Euler(0f, eulerAnglesY, 0f);
            vector = transform.rotation * Vector3.forward;
        }
        else 
            vector = Vector3.zero;

        finalDirection = (vector + Vector3.up * moveDirection.y).normalized;
    }

    private void Movement()
    {
        controller.Move(finalDirection * speed * Time.deltaTime);
        // if (isGrounded)
        // else if (isJumping || inAir)
        //     transform.Translate((jumpDirection * speed * airControl) * Time.deltaTime);
    }

    public GameObject spirit
    {
        get => _spirit;
        set
        {
            _spirit = value;
            bool enabled = value != null;
            gameObject.SetActive(enabled);
            gameObject.GetComponent<Collider>().enabled = enabled;
            if (!enabled)
                lastOccupied = DateTime.Now;
            moveDirection = Vector3.zero;
            finalDirection = Vector3.zero;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        LifeAttractionApply();
        Movement();
    }
    
    private void LifeAttractionApply()
    {
        foreach (var collider in Physics.OverlapSphere(transform.position, lifeAttractionRadius))
            if (collider.gameObject.IsAvailable())
                controller.Move((collider.transform.position - transform.position) * lifeAttractionSpeed * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.IsAvailable())
            creatureContact?.Invoke(hit.gameObject);
    }

    void OnCollisionExit(Collision collisionInfo)
    {
    }
}