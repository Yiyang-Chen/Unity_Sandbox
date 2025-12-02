using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

/// <summary>
/// old input handler & new input system
/// </summary>
public class InputSystem : System<InputSystem>
{
    #region inputsystemparameter
    //New Input system
    public PlayerControls _inputControls;
    public InputAction move;
    public InputAction jump;

    #endregion
    
    private InputModel _mModel;
    public bool CursorLocked
    {
        get => _mModel.cursorLocked; 
        set => _mModel.cursorLocked = value;
    }

    public bool ShowCursor
    {
        get => _mModel.showCursor;
        set
        {
            _mModel.showCursor = value;
            if (_mModel.showCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    protected override void OnInit()
    {
        _mModel = new InputModel();
        _inputControls = new PlayerControls();
        
        move = _inputControls.Player.Move;
        jump = _inputControls.Player.Jump;
        
        this.GetSystem<MonoSystem>()?.AddUpdateListener(Update);
    }

    protected override void OnShutdown()
    {
        this.GetSystem<MonoSystem>()?.RemoveUpdateListener(Update);
        
        _mModel = null;
        // 清理输入控制
        if (_inputControls != null)
        {
            _inputControls.Dispose();
            _inputControls = null;
            move = null;
            jump = null;
        }
    }

    private void Update()
    {
        //MouseRaycastCheck();
    }

    private void MouseRaycastCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug(hit.transform.name);
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("hit");
        }
    }

    public EInputState GetInputState() => _mModel.currentInputState;
    
    // Function to enable certain input functionalities in a particular state
    public void SwitchInputState(EInputState inputState)
    {
        if (!Initialized)
        {
            return;
        }
        
        switch (inputState)
        {
            case EInputState.Default:
                _mModel.currentInputState = EInputState.Default;
                EnableMouse();
                EnableKeyBoard();
                EnableDebug();
                break;
            case EInputState.DisableWithDebug:
                _mModel.currentInputState = EInputState.DisableWithDebug;
                DisableMouse();
                DisableKeyBoard();
                EnableDebug();
                break;
            case EInputState.Disable:
            case EInputState.Inputting:
                _mModel.currentInputState = EInputState.Inputting;
                DisableMouse();
                DisableKeyBoard();
                DisableDebug();
                break;
        }
    }


    #region InputState

    public void DisableMouse()
    {

    }

    public void EnableMouse()
    {

    }

    public void DisableKeyBoard()
    {
        jump.Disable();
        move.Disable();
    }

    public void EnableKeyBoard()
    {
        jump.Enable();
        move.Enable();
    }

    public void DisableDebug()
    {
    }

    public void EnableDebug()
    {
    }
    #endregion
}