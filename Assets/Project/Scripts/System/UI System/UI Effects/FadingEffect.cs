using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FadingEffect : MonoBehaviour
{
    [SerializeField] private float _fadingSpeed = 1.0f;
    [SerializeField] private Color _fadeColor = Color.black;
    [SerializeField] private AnimationCurve _fadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, 1.5f, 1.5f), new Keyframe(1, 0));

    private float _alpha = 0;
    private Texture2D _texture;
    private int _direction = 0;
    private float _time;
   // private Coroutine _fadeCoroutine;
    private void Start()
    {
        _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
        _texture.Apply();
    }
    public void FadeIn()
    {
       // StopFadeEffect();
        _alpha = 1f;
        _time = 0;
        _direction = 1;
       // _fadeCoroutine = StartCoroutine(StartFadeEffect());
    }
    public void FadeOut()
    {
       // StopFadeEffect();
        _alpha = 0f;
        _time = 1;
        _direction = -1;
        //_fadeCoroutine = StartCoroutine(StartFadeEffect());
    }
    //private IEnumerator StartFadeEffect()
    //{
    //    while (_direction != 0)
    //    {
    //        _time += Time.deltaTime * _direction * _fadingSpeed;
    //        _alpha = _fadeCurve.Evaluate(_time);
    //        _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
    //        _texture.Apply();
    //        if (_alpha <= 0f || _alpha >= 1f) _direction = 0;
    //    }
    //    yield return null;
    //}
    //private void StopFadeEffect()
    //{
    //    if (_fadeCoroutine != null)
    //    {
    //        StopCoroutine(_fadeCoroutine);
    //        _fadeCoroutine = null;
    //    }
    //}
    public void OnGUI()
    {
        if (_alpha > 0) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        if (_direction != 0)
        {
            _time += Time.deltaTime * _direction * _fadingSpeed;
            _alpha = _fadeCurve.Evaluate(_time);
            _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
            _texture.Apply();
            if (_alpha <= 0f || _alpha >= 1f) _direction = 0;
        }
    }
}
