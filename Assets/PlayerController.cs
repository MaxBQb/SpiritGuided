using System;
using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Variables
    private bool isUp = false;
    private bool isDown = false;
   

    // References
    [SerializeReference] private new Transform camera;
    [CanBeNull] private GameObject body;
    [SerializeReference] private GameObject spirit;

    // Properties
    private bool isSpirit => body == null;

    public GameObject physicalForm
    {
        get => isSpirit ? spirit : body;
        set
        {
            if (value == null && !isSpirit)
            {
                creature.spirit = null;
                body = null;
                spirit.transform.position = transform.position;
                creature.spirit = gameObject;
                UpdateParent();
                return;
            }

            if (isSpirit && value.IsAvailable())
            {
                creature.spirit = null;
                body = value;
                creature.spirit = gameObject;
            }

            UpdateParent();
        }
    }

    private Creature creature => physicalForm.GetControls();
    private SpiritControls spiritControls => spirit.GetComponent<SpiritControls>();
    
    // Catch user input
    public void OnMove(InputAction.CallbackContext context)
        => creature.Move(context.ReadValue<Vector2>());

    public void OnJump(InputAction.CallbackContext context)
    {
        isUp = context.ReadValueAsButton();
        UpdateVerticalMovement();
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        isDown = context.ReadValueAsButton();
        if (isDown && !isSpirit)
            physicalForm = null;
        UpdateVerticalMovement();
    }

    private void UpdateVerticalMovement() 
        => creature.MoveVertically(isUp == isDown ? 0f : (isUp ? 1f : -1f));
    
    public void OnLook(InputAction.CallbackContext context) 
        => creature.Look(camera.rotation);

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
            creature.Attack();
    }

    // Logic

    private void UpdateParent()
    {
        gameObject.transform.SetParent(physicalForm.gameObject.transform);
        gameObject.transform.position = gameObject.transform.parent.position + creature.offset;
    }

    private void Awake()
    {
        spiritControls.creatureContact += (other) => physicalForm = other; 
    }

    private void Start()
    {
        UpdateParent();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        
    }
}