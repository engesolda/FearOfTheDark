using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText popupText;
    private static GameObject canva;

    public static void Initialize()
    {
        canva = GameObject.Find("Canvas");

        if (popupText == null)
        {
            popupText = Resources.Load<FloatingText>("FloatingText");
        }        
    }
    
    public static void CreateFloatingText(Vector2 _position, string _text, Color _color, int _fontSize)
    {
        FloatingText instance = Instantiate(popupText);
        instance.transform.SetParent(canva.transform, false);
        instance.transform.position = _position;
        instance.ConfigureInstance(_text, _color, _fontSize);
    }
}
