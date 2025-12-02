using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : UnityController
{
    [SerializeField]
    private BindableProperty<EGameState> currentGameState = new();

    GameStateSystem _gameStateSystem;
    
    protected override void OnInit()
    {
        _gameStateSystem = MinimalEnvironment.Instance.GetSystem<GameStateSystem>();
        _gameStateSystem.RegisterController(this);
    }

    protected override void OnShutdown()
    {
        
    }

    protected override void BindRuntimeData()
    {
        // 使用自动绑定，字段名与 GameStateModel 一致
        this.AutoBindProperties(_gameStateSystem.Model);
    }
}
