using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manage the quality of the game.
public class QualitySettingsSystem : System<QualitySettingsSystem>
{
    //Get all the quality levels.
    string QualityString
    {
        get { return QualitySettings.names[QualitySettings.GetQualityLevel()]; }
    }

    [SerializeField] private string _qualitySetting = "";

    protected override void OnInit()
    {
        _qualitySetting = QualityString;
    }

    protected override void OnShutdown()
    {
    }

    //Get current quality.
    public int GetQuality()
    {
        return QualitySettings.GetQualityLevel();
    }

    //Increase current quality.
    public void IncreaseQuality(bool pApplyExpenisve = false)
    {
        if (pApplyExpenisve)
        {
            QualitySettings.IncreaseLevel(true);
        }
        else
        {
            QualitySettings.IncreaseLevel();
        }

        _qualitySetting = QualityString;
    }

    //Decrease current quality.
    public void DecreseQuality(bool pApplyExpenisve = false)
    {
        if (pApplyExpenisve)
        {
            QualitySettings.DecreaseLevel(true);
        }
        else
        {
            QualitySettings.DecreaseLevel();
        }

        _qualitySetting = QualityString;
    }

    //Set quality directly.
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        _qualitySetting = QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
}