using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class RSUtils
{
    public static TextMesh CreateWorldText(string text, Transform parent = null,
        Vector3 localPostion = default(Vector3),
        int fontSize = 40, Color color = default(Color), TextAnchor anchor = TextAnchor.MiddleCenter,
        TextAlignment textAlignment = TextAlignment.Center, int order = 0)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPostion, fontSize, color, anchor, textAlignment, order);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPostion, int fontSize,
        Color color,
        TextAnchor anchor, TextAlignment textAlignment, int order)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPostion;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = anchor;
        textMesh.alignment = textAlignment;
        textMesh.color = color;
        textMesh.fontSize = fontSize;
        textMesh.text = text;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = order;
        return textMesh;
    }

    public static Vector3 GetMouseWorldPos(bool withZ = false, Camera worldCamera = null)
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(Input.mousePosition);
        if (!withZ)
        {
            worldPosition.z = 0;
        }

        return worldPosition;
    }

    public static GameObject FindInChildren(Transform parent, string name)
    {
        return parent.Cast<Transform>().FirstOrDefault(t => t.name == name)?.gameObject;
    }
}