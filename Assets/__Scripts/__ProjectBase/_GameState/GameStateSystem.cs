using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSystem : System<GameStateSystem>
{
    private GameStateModel _mModel;
    
    /// <summary>
    /// 访问 Model，用于 Controller 的自动绑定
    /// </summary>
    public GameStateModel Model => _mModel;

    protected override void OnInit()
    {
        _mModel = new GameStateModel();
    }

    protected override void OnShutdown()
    {
        _mModel = null;
    }

    //Set game state to a state
    public void SetGameState(EGameState s)
    {
        _mModel.SetGameState(s);
    }

    //Get current game state
    public EGameState GetGameState()
    {
        return _mModel.currentGameState;
    }
}