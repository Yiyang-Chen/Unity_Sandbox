using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtils
{
    //TODO: 没有小数时，显示1.0k好看还是1k好看？
    public static string ReadableCount(float count, int decimalPlaces = 1)
    {
        bool isNegative = count < 0;
        float absCount = Mathf.Abs(count);
        
        string[] units = { "", "K", "M" };
        float[] unitScale = { 1f, 1000f, 1000000f };
        int idx = 0;
        
        while (idx < units.Length - 1)
        {
            if (absCount < unitScale[idx + 1] || (idx == 0 && absCount < 1000))
                break;
            idx++;
        }
        
        string valueStr;
        if (decimalPlaces == 0)
        {
            valueStr = Mathf.Floor(absCount / unitScale[idx]).ToString();
        }
        else
        {
            string format = "0." + new string('0', decimalPlaces);
            valueStr = (absCount / unitScale[idx]).ToString(format);
            if (valueStr.EndsWith("." + new string('0', decimalPlaces)))
                valueStr = valueStr.Substring(0, valueStr.IndexOf('.'));
        }
        
        return $"{(isNegative ? "-" : "")}{valueStr}{units[idx]}";
    }
}
