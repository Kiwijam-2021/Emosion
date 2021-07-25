using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BlobPreview : MonoBehaviour
{
    public BlobType type;
    private SpriteRenderer _spriteRenderer;
    [ReadOnly(true)] public Sprite[] blobSprites = new Sprite[6];
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (_spriteRenderer.sprite != blobSprites[(int)type])
        {
            _spriteRenderer.sprite = blobSprites[(int)type];
        }
    }

}