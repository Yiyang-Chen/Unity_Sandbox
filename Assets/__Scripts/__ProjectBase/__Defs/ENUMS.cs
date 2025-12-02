#region events

public enum EGlobalEvent
{
    Example_MonsterDead,
    Example_JumpPressed,
    Example_UpdateProgressBar,
    Default,
    OnShutdown,
};

public enum EEntityEvent
{
    Example1,
    Example2,
    
    OnCreated,
    
    OnTagChanged,
    EntityBeginMove,
    EntityStopMove,
}

#endregion

//overall game state

#region gamestate

public enum EGameState
{
    GameDefaultState,
    GameStateA,
    GameStateB
};

#endregion

#region input

public enum EInputState
{
    Default,
    DisableWithDebug,
    Disable,
    Inputting,
}

#endregion

#region UI

public enum EUILayer
{
    Bot,
    Mid,
    Top,
    Popup,
    System,
    Dialogue
}

#endregion

#region data

// streaming Ҫ����build��Ķ����� persistent ������̨�����ϵ�һЩ�ļ� temporary ��ʱcache DataPath editor��runtime���᲻һ��
public enum EDataPath
{
    Default,
    Streaming,
    Persistent,
    Temporary
}

#endregion

#region AttackDirection

[System.Flags]
public enum EFlagExample
{
    None = 0,
    Up = 1,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    UpLeft = 1 << 4,
    DownLeft = 1 << 5,
    UpRight = 1 << 6,
    DownRight = 1 << 7
}

#endregion