using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : UnityController
{
    [Header("Mouse Cursor Settings")] 
    [SerializeField]
    public BindableProperty<EInputState> currentInputState = new();
    [SerializeField]
    public BindableProperty<bool> cursorLocked = new();
    [SerializeField]
    public BindableProperty<bool> showCursor = new();

    private InputSystem _inputSystem;

    protected override void OnInit()
    {
        _inputSystem = MinimalEnvironment.Instance.GetSystem<InputSystem>();
        _inputSystem.RegisterController(this);

        OnEnable();
    }

    private void OnEnable()
    {
        if (_inputSystem == null)
        {
            return;
        }
        
        // 启用所有输入
        _inputSystem.SwitchInputState(EInputState.Default);
    }

    private void OnDisable()
    {
        if (_inputSystem == null)
        {
            return;
        }
        
        _inputSystem.SwitchInputState(EInputState.Disable);
    }

    private void Update()
    {
        #region SampleCode

        // Pause Menu Panel
        if (_inputSystem.jump.WasPressedThisFrame())
        {
            MinimalEnvironment.Instance.GetSystem<EventSystem>().EventTrigger(EGlobalEvent.Example_JumpPressed);
        }

        #endregion
    }

    protected override void OnShutdown()
    {
        OnDisable();
        // 清理
        _inputSystem = null;
    }

    protected override void BindRuntimeData()
    {
        // Input 的绑定需要通过 System 的方法触发副作用，不能用 AutoBindProperties
        // Label 会在 Awake 时自动设置，不需要手动调用 WithLabel
        currentInputState
            .BindTo(
                () => _inputSystem.GetInputState(),
                value => _inputSystem.SwitchInputState(value)
            )
            .BelongsTo(typeof(InputModel));
        
        cursorLocked
            .BindTo(
                () => _inputSystem.CursorLocked,
                value => _inputSystem.CursorLocked = value
            )
            .BelongsTo(typeof(InputModel));
        
        showCursor
            .BindTo(
                () => _inputSystem.ShowCursor,
                value => _inputSystem.ShowCursor = value
            )
            .BelongsTo(typeof(InputModel));
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        //SetCursorState(cursorLocked);
    }
}