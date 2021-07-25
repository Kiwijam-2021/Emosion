using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
    private PostProcessVolume _volume;
    private ChromaticAberration _chromaticAberration;
    private bool _isIncreasing = true;
    private float _deltaTime = 0;

    public float intensity = 0.5f;
    public int frequency = 200;

    public bool isExploding = false;

    public static PostProcessing Instance { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(this);
        }

        ;

        Instance = this;
    }

    private void Start()
    {
        _volume = GetComponent<PostProcessVolume>();
        _chromaticAberration = _volume.profile.GetSetting<ChromaticAberration>();
    }

    private void Update()
    {
        if (isExploding) return;

        var duration = (float) 1 / frequency;
        _deltaTime += Time.deltaTime;

        if ((_deltaTime / duration) > 1)
        {
            _isIncreasing = !_isIncreasing;
            _deltaTime = 0;
        }

        var value = Mathf.Lerp(0, intensity, _deltaTime / duration);

        if (_isIncreasing)
        {
            _chromaticAberration.intensity.value = value;
        }
        else
        {
            _chromaticAberration.intensity.value = intensity - value;
        }
    }

    public void PlayExplosion(float initialStrength, float strength)
    {
        StartCoroutine(Explode(initialStrength, strength));
    }
    //
    // IEnumerator Explode(float initialStrength, float strength)
    // {
    //     Debug.Log("Explode");
    //     if (isExploding)
    //     {
    //         yield return new WaitForSeconds(0);
    //     }
    //
    //     isExploding = true;
    //     _deltaTime = 0;
    //     var duration = (float) 1 / 3;
    //
    //     while (true)
    //     {
    //         _deltaTime += Time.deltaTime;
    //
    //         if ((_deltaTime / duration) > 1)
    //         {
    //             if (_isIncreasing)
    //             {
    //                 _isIncreasing = false;
    //                 _deltaTime = 0;
    //             }
    //             else
    //             {
    //                 break;
    //             }
    //         }
    //
    //         yield return new WaitForSeconds(duration);
    //
    //         var value = Mathf.Lerp(initialStrength, strength, _deltaTime / duration);
    //
    //         if (_isIncreasing)
    //         {
    //             _chromaticAberration.intensity.value = value;
    //         }
    //         else
    //         {
    //             _chromaticAberration.intensity.value = intensity - value;
    //         }
    //     }
    //
    //     isExploding = false;
    // }

    IEnumerator Explode(float initialStrength, float strength)
    {
        Debug.Log("Explode");
        if (isExploding)
        {
            yield return new WaitForSeconds(0);
        }

        isExploding = true;

        _chromaticAberration.intensity.value = initialStrength;
        yield return new WaitForSeconds(0.05f);

        _chromaticAberration.intensity.value = ((strength - initialStrength) / 2) + initialStrength;
        yield return new WaitForSeconds(0.025f);

        _chromaticAberration.intensity.value = strength;
        yield return new WaitForSeconds(0.0125f);

        _chromaticAberration.intensity.value = 0;

        isExploding = false;
    }
}