using System.Numerics;
using JetBrains.Annotations;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public static class Utils
{

    public const int sortingOrderDefault = 5000; 

    
    public static TextMesh CreateWorldText(Transform parent, string text, UnityEngine.Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
        
    }

    public static UnityEngine.Vector3 GetMouseWorldPosition()
    {
        UnityEngine.Vector3 vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vector.z = 0f;
        return vector;
    }
    public static UnityEngine.Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static UnityEngine.Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static UnityEngine.Vector3 GetMouseWorldPositionWithZ(UnityEngine.Vector3 screenPosition, Camera worldCamera)
    {
        UnityEngine.Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }


}