using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public PlayerMovement movement;
    public PlayerJumping jumping;
    public PlayerCollisionDetector collisionDetector;
    public PlayerInputBuffer inputBuffer;
    public PlayerInput playerInput;
    
    private PlayerStateMachine stateMachine;
    private bool isInitialized = false;
    
    public PlayerStateMachine StateMachine => stateMachine;
    
    void Awake()
    {
        RefreshComponentReferences();
    }
    
    void Start()
    {
        InitializeIfReady();
    }
    
    void Update()
    {
        if (!isInitialized)
        {
            InitializeIfReady();
            return;
        }
        
        stateMachine?.Update();
    }
    
    void FixedUpdate()
    {
        if (!isInitialized) return;
        stateMachine?.FixedUpdate();
    }
    
    public void RefreshComponentReferences()
    {
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (jumping == null) jumping = GetComponent<PlayerJumping>();
        if (collisionDetector == null) collisionDetector = GetComponent<PlayerCollisionDetector>();
        if (inputBuffer == null) inputBuffer = GetComponent<PlayerInputBuffer>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
    }
    
    private void InitializeIfReady()
    {
        RefreshComponentReferences();
        
        if (AreAllComponentsReady())
        {
            if (stateMachine == null)
            {
                stateMachine = new PlayerStateMachine(this);
                stateMachine.ChangeState(PlayerState.Grounded);
                isInitialized = true;
                Debug.Log("PlayerController initialized successfully!");
            }
        }
    }
    
    private bool AreAllComponentsReady()
    {
        return movement != null && 
               jumping != null && 
               collisionDetector != null && 
               inputBuffer != null && 
               playerInput != null;
    }
    
    [ContextMenu("Force Initialize")]
    public void ForceInitialize()
    {
        isInitialized = false;
        InitializeIfReady();
    }
}