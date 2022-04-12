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
    [SerializeReference] private AudioSource spiritMoveInSound;
    [SerializeReference] private ParticleSystem spiritMoveInEffect;

    // Properties
    private bool isSpirit => body == null;

    public GameObject physicalForm
    {
        get => isSpirit ? spirit : body;
        set
        {
            if (value == null)
                MoveOut();
            else
                MoveIn(value);
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
    
    private void MoveIn(GameObject newBody)
    {
        if (!isSpirit || !newBody.IsAvailable()) 
            return;
        creature.spirit = null;
        body = newBody;
        creature.spirit = gameObject;
        UpdateParent();
        spiritMoveInSound.Play();
        spiritMoveInEffect.Play();
    }

    private void MoveOut()
    {
        if (isSpirit)
            return;
        transform.position += creature.offset * 2f;
        creature.spirit = null;
        body = null;
        spirit.transform.position = transform.position;
        UpdateParent();
        creature.spirit = gameObject;
    }
    
    private void UpdateParent()
    {
        gameObject.transform.SetParent(physicalForm.gameObject.transform);
        gameObject.transform.position = gameObject.transform.parent.position + creature.offset;
    }

    private void OnEnable()
    {
        spiritControls.creatureContact += MoveIn;
    }
    
    private void OnDisable()
    {
        spiritControls.creatureContact -= MoveIn;
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