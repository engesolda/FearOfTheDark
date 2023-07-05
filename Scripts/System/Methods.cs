using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class Methods
{
    public struct SceneLoaderPlayerPosition
    {
        public Vector2 playerPosition;
        public string previwsMap;
        public string currentMap;

        public SceneLoaderPlayerPosition(Vector2 _playerPos, string _previusMap, string _currentMap) { playerPosition = _playerPos; previwsMap = _previusMap; currentMap = _currentMap; }
    }

    public static int ConvertBoolToInt(bool _value)
    {
        int i = _value ? 1 : 0;
        return i;
    }

    public static bool ConvertIntToBool(int _value)
    {
        bool i = _value == 1 ? true : false;
        return i;
    }

    public static bool ConvertStringToBool(string _value)
    {
        bool i = _value == "1" ? true : false;
        return i;
    }

    public static string ConvertBoolToString(bool _value)
    {
        string i = _value == true ? "1" : "0";
        return i;
    }

    public static Vector3 ConvertStringToVector3(string _value)
    {
        _value = _value.Replace("(", "");
        _value = _value.Replace(")", "");
        _value = _value.Replace(",", "/");
        _value = _value.Replace(".", ",");

        string[] strings = _value.Split("/"[0]);

        float x = float.Parse(strings[0]);
        float y = float.Parse(strings[1]);
        float z = float.Parse(strings[2]);

        return new Vector3(x, y, z);
    }

    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;// = new Sprite();
        Texture2D SpriteTexture = LoadTexture(FilePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }

    public static Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
