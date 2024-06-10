using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string _name;
    public SpriteRenderer _image;
    [TextArea(5, 10)] public string[] _text;
}