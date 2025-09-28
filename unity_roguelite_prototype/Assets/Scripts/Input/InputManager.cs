using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Settings")]
    public PlayerInputActions playerInputActions;

    // Input values
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool DashPressed { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupInputActions()
    {
#if ENABLE_INPUT_SYSTEM
        if (playerInputActions == null)
            playerInputActions = new PlayerInputActions();

        if (playerInputActions.Player.Move != null)
        {
            // Bind movement
            playerInputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            playerInputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        }

        if (playerInputActions.Player.Jump != null)
        {
            // Bind jump
            playerInputActions.Player.Jump.performed += ctx => JumpPressed = true;
            playerInputActions.Player.Jump.canceled += ctx => JumpHeld = false;
            playerInputActions.Player.Jump.started += ctx => JumpHeld = true;
        }

        if (playerInputActions.Player.Attack != null)
        {
            // Bind attack
            playerInputActions.Player.Attack.performed += ctx => AttackPressed = true;
        }

        if (playerInputActions.Player.Dash != null)
        {
            // Bind dash
            playerInputActions.Player.Dash.performed += ctx => DashPressed = true;
        }
#endif
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        playerInputActions?.Enable();
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        playerInputActions?.Disable();
#endif
    }

    void LateUpdate()
    {
        // Reset single-frame inputs
        JumpPressed = false;
        AttackPressed = false;
        DashPressed = false;
    }

    public void OnDestroy()
    {
#if ENABLE_INPUT_SYSTEM
        playerInputActions?.Dispose();
#endif
    }
}

[System.Serializable]
public class PlayerInputActions
{
    public PlayerActions Player;

    public PlayerInputActions()
    {
        Player = new PlayerActions();
    }

    public void Enable()
    {
        Player.Enable();
    }

    public void Disable()
    {
        Player.Disable();
    }

    public void Dispose()
    {
        Player?.Dispose();
    }
}

[System.Serializable]
public class PlayerActions
{
#if ENABLE_INPUT_SYSTEM
    public InputAction Move;
    public InputAction Jump;
    public InputAction Attack;
    public InputAction Dash;
#endif

    public PlayerActions()
    {
#if ENABLE_INPUT_SYSTEM
        // Setup default input bindings
        Move = new InputAction("Move", binding: "<Gamepad>/leftStick", type: InputActionType.Value);
        Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        Jump = new InputAction("Jump", binding: "<Keyboard>/space", type: InputActionType.Button);
        Jump.AddBinding("<Gamepad>/buttonSouth");

        Attack = new InputAction("Attack", binding: "<Mouse>/leftButton", type: InputActionType.Button);
        Attack.AddBinding("<Keyboard>/leftCtrl");
        Attack.AddBinding("<Gamepad>/buttonWest");

        Dash = new InputAction("Dash", binding: "<Keyboard>/leftShift", type: InputActionType.Button);
        Dash.AddBinding("<Gamepad>/buttonEast");
#endif
    }

    public void Enable()
    {
#if ENABLE_INPUT_SYSTEM
        Move?.Enable();
        Jump?.Enable();
        Attack?.Enable();
        Dash?.Enable();
#endif
    }

    public void Disable()
    {
#if ENABLE_INPUT_SYSTEM
        Move?.Disable();
        Jump?.Disable();
        Attack?.Disable();
        Dash?.Disable();
#endif
    }

    public void Dispose()
    {
#if ENABLE_INPUT_SYSTEM
        Move?.Dispose();
        Jump?.Dispose();
        Attack?.Dispose();
        Dash?.Dispose();
#endif
    }
}