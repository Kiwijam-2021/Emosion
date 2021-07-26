using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Blob : MonoBehaviour
{
    private static readonly Vector3 BaseSize = new(1, 1, 1);

    private static readonly Dictionary<BlobType, int> BlobScores = new()
    {
        {BlobType.VeryHappy, 1},
        {BlobType.Happy, 1},
        {BlobType.SlightlyHappy, 1},
        {BlobType.SlightlyUnhappy, 1},
        {BlobType.Unhappy, 1},
        {BlobType.VeryUnhappy, 1},
    };

    private const int MaxSize = 3;
    private const float SmoothTime = 0.1F;

    private DateTime _createdAt;
    private Vector3 _from;
    private Vector3 _to;
    private Vector3 _velocity = Vector3.zero;
    private bool _isScaling;
    private SpriteRenderer _spriteRenderer;
    private bool _scored;
    private GameManager _gameManager;

    private Rigidbody2D _rb;

    public GameObject explosionPrefab;

    [ReadOnly(true)] public double area;
    public BlobType type;
    [ReadOnly(true)] public int size;
    [ReadOnly(true)] public Sprite[] blobSprites = new Sprite[6];

    public double minAudioVelocity = 1f;
    private AudioSource _audioSource;
    public List<AudioClip> bounceAudioClips = new List<AudioClip>();
    public List<AudioClip> positiveAudioClips = new List<AudioClip>();
    public List<AudioClip> negativeAudioClips = new List<AudioClip>();
    public List<AudioClip> explosionAudioClips = new List<AudioClip>();
    public float bounceVolume = 1.0f;
    public float mergeVolume = 1.0f;

    private void UpdateArea()
    {
        area = Math.PI * Math.Pow(transform.localScale.x / 2f, 2);
    }

    private void Awake()
    {
    }
    
    private void Start()
    {
        size = 1;
        _rb = GetComponent<Rigidbody2D>();
        _from = transform.localScale;
        _to = transform.localScale;
        _createdAt = DateTime.UtcNow;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _scored = false;
        _gameManager = GameManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        UpdateArea();
    }
    

    private void Update()
    {
        if (_spriteRenderer.sprite != blobSprites[(int) type])
        {
            _spriteRenderer.sprite = blobSprites[(int) type];
        }
    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, _to, ref _velocity, SmoothTime);
        UpdateArea();
    }

    private void PlayRandomClip(IList<AudioClip> clips, bool requireMinVelocity = true, float volume = 1.0f)
    {
        if (!requireMinVelocity || _rb.velocity.magnitude > minAudioVelocity)
        {
            _audioSource.PlayOneShot(clips.GetRandomItem(), volume);
        }
    }

    private void OnCollision(Collision2D other, bool isAudioSource = true)
    {
        if (!other.gameObject.CompareTag(tag))
        {
            if (isAudioSource)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.pitch = 1.0f / size;
                    PlayRandomClip(bounceAudioClips, true, bounceVolume);
                }
            }

            return;
        }

        var otherBlob = other.gameObject.GetComponent<Blob>();

        if (type != otherBlob.type)
        {
            if (type == BlobType.VeryHappy && size == MaxSize &&
                otherBlob.type == BlobType.VeryUnhappy && otherBlob.size == MaxSize)
            {
                Destroy(gameObject);
                Destroy(other.gameObject);
            }

            if (type == BlobType.VeryHappy && size == MaxSize &&
                otherBlob.type == BlobType.VeryUnhappy && otherBlob.size == 1)
            {
                otherBlob.type = BlobType.Unhappy;
            }

            if (type == BlobType.VeryHappy && size == MaxSize &&
                otherBlob.type == BlobType.Unhappy && otherBlob.size == 1)
            {
                otherBlob.type = BlobType.SlightlyUnhappy;
            }

            if (type == BlobType.VeryUnhappy && size == MaxSize &&
                otherBlob.type == BlobType.VeryHappy && otherBlob.size == 1)
            {
                otherBlob.type = BlobType.Happy;
            }

            if (type == BlobType.VeryUnhappy && size == MaxSize &&
                otherBlob.type == BlobType.Happy && otherBlob.size == 1)
            {
                otherBlob.type = BlobType.SlightlyHappy;
            }
        }

        if (_rb.velocity.magnitude > otherBlob._rb.velocity.magnitude || size > otherBlob.size)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.pitch = 1.0f / size;
                PlayRandomClip(bounceAudioClips, true, bounceVolume);
            }
        }

        // Larger object absorbs the other
        // If area is equal then the object with the lower speed absorbs the other

        if (size >= MaxSize || size != otherBlob.size || type != otherBlob.type) return;

        // If we're merging, only play the merge sound
        // otherwise play the bounce sound
        PostProcessing.Instance.PlayExplosion(0.7f, 1f);

        size += 1;
        _from = transform.localScale;
        _to = BaseSize * size;
        _isScaling = true;
        _createdAt = DateTime.UtcNow;
        Destroy(other.gameObject);

        

        if (size == MaxSize && !_scored && !_gameManager.finished)
        {
            GetComponent<Explodable>().explode();
            Explode();

            _scored = true;
            if (type is BlobType.VeryHappy or BlobType.Happy or BlobType.SlightlyHappy)
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }

                PlayRandomClip(positiveAudioClips, false, mergeVolume);
                if (BlobScores.TryGetValue(type, out var value))
                {
                    _gameManager.happyScore += value*5;
                }
            }

            if (type is BlobType.VeryUnhappy or BlobType.Unhappy or BlobType.SlightlyUnhappy)
            {
                _audioSource.pitch = 1.0f;
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }

                PlayRandomClip(negativeAudioClips, false, mergeVolume);
                if (BlobScores.TryGetValue(type, out var value))
                {
                    _gameManager.unhappyScore += value*5;
                }
            }

            return;
        }
        if (!_scored && !_gameManager.finished)
        {
            if (type is BlobType.VeryHappy or BlobType.Happy or BlobType.SlightlyHappy)
            {
                if (BlobScores.TryGetValue(type, out var value))
                {
                    _gameManager.happyScore += value;
                }
            }


            if (type is BlobType.VeryUnhappy or BlobType.Unhappy or BlobType.SlightlyUnhappy)
            {
                if (BlobScores.TryGetValue(type, out var value))
                {
                    _gameManager.unhappyScore += value;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCollision(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        OnCollision(other, false);
    }

    private void Explode()
    {
        var audioSource = GameManager.Instance.audioSource;
        audioSource.pitch = 1.0f / size;
        audioSource.PlayOneShot(explosionAudioClips.GetRandomItem(), 1.0f);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}