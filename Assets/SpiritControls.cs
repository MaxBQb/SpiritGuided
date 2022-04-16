using System;
using DefaultNamespace;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpiritControls : MonoBehaviour, Creature, IOnEventCallback
{
    // Variables
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float lifeAttractionRadius = 5f;
    [SerializeField] private float lifeAttractionPower = 10f;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion lookAngle = Quaternion.identity;
    private Vector3 finalDirection = Vector3.zero;
    private bool newEnabled = true;
    
    // References
    private CharacterController controller;
    [CanBeNull] private GameObject _spirit;
    private PhotonView view;

    // Properties
    public Rigidbody _rigidbody;
    public Vector3 offset => Vector3.zero;
    public DateTime lastOccupied { get; private set; }
    
    // Events
    public event Action<GameObject> creatureContact;
    private const int ON_MOVE_IN = 42;
    
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
    }

    public GameObject spirit
    {
        get => _spirit;
        set
        {
            _spirit = value;
            newEnabled = value != null;
            if (view.IsMine && !newEnabled)
                PhotonNetwork.RaiseEvent(ON_MOVE_IN, transform.position, new RaiseEventOptions()
                {
                    Receivers = ReceiverGroup.Others
                }, SendOptions.SendReliable);
            gameObject.SetActive(newEnabled);
            gameObject.GetComponent<Collider>().enabled = newEnabled;
            if (!newEnabled)
                lastOccupied = DateTime.Now;
            moveDirection = Vector3.zero;
            finalDirection = Vector3.zero;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        view = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (!enabled && !view.IsMine)
            return;
        LifeAttractionApply();
        Movement();
    }
    
    private void LifeAttractionApply()
    {
        foreach (var collider in Physics.OverlapSphere(transform.position, lifeAttractionRadius))
            if (collider.gameObject.IsAvailable())
                Magnetize(collider.transform.position
                          + collider.gameObject.GetControls().offset
                          - transform.position, lifeAttractionPower);
    }
    
    void OnDrawGizmosSelected()
    {   
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lifeAttractionRadius);
    }
    
    private void Magnetize(Vector3 direction, float power)
    {
        controller.Move(direction.normalized * power/(direction.magnitude*direction.magnitude) * Time.deltaTime);
        Debug.DrawRay(transform.position, direction, Color.green);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.IsAvailable())
            creatureContact?.Invoke(hit.gameObject);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case ON_MOVE_IN:
                OnMoveIn((Vector3) photonEvent.CustomData);
                break;
        }
    }

    private void OnMoveIn(Vector3 pos) => transform.position = pos;
    
}