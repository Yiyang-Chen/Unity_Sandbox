using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogComponent : BaseComponent
{
    protected override void InitComponent()
    {
        base.InitComponent();
    }

    protected override void DisposeComponent()
    {
        base.DisposeComponent();
    }

    protected string LogEntityId
    {
        get
        {
            return $"Entity{entity.id.ToString()}";
        }
    }
    
    public void Debug(string message)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug($"{LogEntityId}: {message}");
    }

    public void Info(string message)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Info($"{LogEntityId}: {message}");
    }

    public void Warn(string message)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Warn($"{LogEntityId}: {message}");
    }

    public void Error(string message)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"{LogEntityId}: {message}");
    }
    
    public void Fatal(string message)
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Fatal($"{LogEntityId}: {message}");
    }
}