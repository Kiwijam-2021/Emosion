using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    public GameObject blobPrefab;
    public GameObject arrowPrefab;
    public GameObject blobPreviewPrefab;
    public int numberOfInitialBlobs;

    private bool _isSpawnQueued = false;
    private bool _canSpawn = true;
    private float _spawnDelayTime = 0;
    private Vector3 _shootDirection;
    private float _shootDistance;

    [Range(0.0f, 100.0f)] public float spawnDelay = 1;
    [Range(0, 100)] public float speed = 5;

    private float _playerOneCachedPlatter;
    private float _playerTwoCachedPlatter;
    private float _playerOneShootDirection;
    private float _playerTwoShootDirection;
    private float _playerOneShootDistance;
    private float _playerTwoShootDistance;
    private GameObject _playerOneArrow;
    private GameObject _playerTwoArrow;
    private GameObject _sharedArrow;
    private GameObject _nextEmoji;
    private BlobType _nextEmojiType;
    private GameObject _playerOneSpawner;
    private GameObject _playerTwoSpawner;
    private MidiController _midiController;
    private AudioSource _playerOneAudioSource;
    private AudioSource _playerTwoAudioSource;
    private GameObject _centerField;
    private GameObject _leftField;
    private GameObject _rightField;

    private void Awake()
    {
        var position = transform.position;
        _midiController = GetComponent<MidiController>();
        _playerOneSpawner = GameObject.FindGameObjectWithTag("PlayerOne");
        _playerTwoSpawner = GameObject.FindGameObjectWithTag("PlayerTwo");
        _centerField = GameObject.FindGameObjectWithTag("Center");
        _leftField = GameObject.FindGameObjectWithTag("LeftField");
        _rightField = GameObject.FindGameObjectWithTag("RightField");
        _playerOneAudioSource = _playerOneSpawner.GetComponent<AudioSource>();
        _playerTwoAudioSource = _playerTwoSpawner.GetComponent<AudioSource>();
        _playerOneArrow = Instantiate(arrowPrefab, _playerOneSpawner.transform.position, Quaternion.identity);
        _playerTwoArrow = Instantiate(arrowPrefab, _playerTwoSpawner.transform.position, Quaternion.identity);
        _sharedArrow = Instantiate(arrowPrefab, position, Quaternion.identity);
        _playerOneArrow.SetActive(false);
        _playerTwoArrow.SetActive(false);
        _leftField.SetActive(false);
        _rightField.SetActive(false);
        _nextEmoji = Instantiate(blobPreviewPrefab, new Vector3(position.x, position.y, position.z - 1),
            Quaternion.identity);
    }

    private void Start()
    {
        var position = transform.position;
        _playerOneShootDirection = 0;
        _playerTwoShootDirection = 0;
        _playerOneShootDistance = 1.0f;
        _playerTwoShootDistance = 1.0f;

        _nextEmojiType = ChooseRandomEmoji(GameManager.Instance.isPlayerOneTurn);
        _nextEmoji.GetComponent<BlobPreview>().type = _nextEmojiType;

        Reconnect();
        // ShootInitialBlobs();
        StartCoroutine(nameof(ShootInitialBlobs));
    }

    private void Update()
    {
        if (_sharedArrow.activeSelf)
        {
            var position = transform.position;
            position.z--;
            _nextEmoji.transform.position = position;
        }
        else
        {
            if (GameManager.Instance.isPlayerOneTurn)
            {
                var position = _playerOneSpawner.transform.position;
                position.z--;
                _nextEmoji.transform.position = position;
            }
            else
            {
                var position = _playerTwoSpawner.transform.position;
                position.z--;
                _nextEmoji.transform.position = position;
            }
        }

        // Update mouse
        {
            var position = transform.position;
            _shootDirection = GetMousePosition().normalized;
            _shootDistance = Vector3.Distance(GetMousePosition(), position);
        }

        // Arrows angle
        _sharedArrow.transform.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), GetShootVector());
        _playerOneArrow.transform.rotation =
            Quaternion.FromToRotation(new Vector3(1, 0, 0), GetDirectionVector(_playerOneShootDirection));
        _playerTwoArrow.transform.rotation =
            Quaternion.FromToRotation(new Vector3(1, 0, 0), GetDirectionVector(_playerTwoShootDirection));
        // Arrows length
        var sharedScale = _sharedArrow.transform.localScale;
        sharedScale.x = GetShootForce();
        _sharedArrow.transform.localScale = sharedScale;
        var playerOneScale = _playerOneArrow.transform.localScale;
        playerOneScale.x = _playerOneShootDistance * 2;
        _playerOneArrow.transform.localScale = playerOneScale;
        var playerTwoScale = _playerTwoArrow.transform.localScale;
        playerTwoScale.x = _playerTwoShootDistance * 2;
        _playerTwoArrow.transform.localScale = playerTwoScale;
        // Dim inactive arrow
        var dimPlayerOne = false;
        var dimPlayerTwo = false;
        if (GameManager.Instance.finished)
        {
            dimPlayerOne = true;
            dimPlayerTwo = true;
        }
        else
        {
            if (GameManager.Instance.isPlayerOneTurn)
            {
                dimPlayerTwo = true;
            }
            else
            {
                dimPlayerOne = true;
            }
        }

        var playerOneColor = _playerOneArrow.GetComponent<SpriteRenderer>().color;
        playerOneColor.a = dimPlayerOne ? 0.5f : 1.0f;
        _playerOneArrow.GetComponent<SpriteRenderer>().color = playerOneColor;
        var playerTwoColor = _playerTwoArrow.GetComponent<SpriteRenderer>().color;
        playerTwoColor.a = dimPlayerTwo ? 0.5f : 1.0f;
        _playerTwoArrow.GetComponent<SpriteRenderer>().color = playerTwoColor;

        // var isClick = Input.GetMouseButtonDown(0);
        // if (Input.touches.Length > 0)
        // {
        //     isClick = isClick || Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began;
        //     Debug.Log("Touch!");
        // }
        //
        // if (isClick && _canSpawn)
        // {
        //     _isSpawnQueued = true;
        //     _canSpawn = false;
        //     _spawnDelayTime = 0;
        //     Debug.Log("Spawn!");
        // }
    }

    private void FixedUpdate()
    {
        _spawnDelayTime += Time.deltaTime;

        if (_spawnDelayTime >= spawnDelay)
        {
            _canSpawn = true;
        }

        // if (!_isSpawnQueued) return;
        // _isSpawnQueued = false;
    }

    private Vector3 GetMousePosition()
    {
        System.Diagnostics.Debug.Assert(Camera.main != null, "Camera.main != null");
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = transform.position.z;
        return pos;
    }

    private void OnMouseUp()
    {
        if (!_canSpawn) return;
        _canSpawn = false;
        _spawnDelayTime = 0;
        Debug.Log("Spawn!");
        GameManager.Instance.NextTurn();
        ShootBlob(transform.position, GetShootVector(), _nextEmojiType);
        _nextEmojiType = ChooseRandomEmoji(GameManager.Instance.isPlayerOneTurn);
        _nextEmoji.GetComponent<BlobPreview>().type = _nextEmojiType;
    }

    private void ShootBlob(Vector3 position, Vector3 direction, BlobType type)
    {
        var blobGameObject = Instantiate(blobPrefab, position, Quaternion.identity);
        var blob = blobGameObject.GetComponent<Blob>();
        blobGameObject.GetComponent<Explodable>().fragmentInEditor();
        blob.type = type;
        var blobRigidbody = blobGameObject.GetComponent<Rigidbody2D>();
        blobRigidbody.AddForce(direction * (speed * blobRigidbody.mass), ForceMode2D.Impulse);
    }

    private IEnumerator ShootInitialBlobs()
    {
        var position = transform.position;
        for (var i = 0; i < numberOfInitialBlobs; ++i)
        {
            _shootDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f),
                position.z);
            _shootDistance = 1;
            ShootBlob(position, GetShootVector(), (BlobType)(i % Enum.GetNames(typeof(BlobType)).Length));
            yield return new WaitForSeconds(0.3f);
        }
    }

    private float GetShootForce()
    {
        return (float)Math.Log(_shootDistance * 10 + 1) / 2;
    }

    private Vector3 GetShootVector()
    {
        return _shootDirection * -GetShootForce();
    }

    private Vector3 GetDirectionVector(float radian)
    {
        return Quaternion.Euler(0, 0, Mathf.Rad2Deg * radian) * new Vector3(1, 0, transform.position.z);
    }

    private static BlobType ChooseRandomEmoji(bool isPlayerOneTurn)
    {
        var value = 0;
        if (isPlayerOneTurn)
        {
            value = UnityEngine.Random.Range((int)BlobType.VeryHappy, (int)BlobType.SlightlyHappy + 1);
            return (BlobType)Enum.Parse(typeof(BlobType), value.ToString());
        }

        value = UnityEngine.Random.Range((int)BlobType.SlightlyUnhappy, (int)BlobType.VeryUnhappy + 1);
        return (BlobType)Enum.Parse(typeof(BlobType), value.ToString());
    }

    public void Reconnect()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            if ((change != InputDeviceChange.Added) && (change != InputDeviceChange.Reconnected)) return;

            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            _playerOneArrow.SetActive(true);
            _playerTwoArrow.SetActive(true);
            _sharedArrow.SetActive(false);
            _leftField.SetActive(true);
            _rightField.SetActive(true);
            _centerField.SetActive(false);

            midiDevice.onWillNoteOn += (note, velocity) =>
            {
                // Note that you can't use note.velocity because the state
                // hasn't been updated yet (as this is "will" event). The note
                // object is only useful to specify the target note (note
                // number, channel number, device name, etc.) Use the velocity
                // argument as an input note velocity.
                Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    velocity,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                // Play pads
                if (note.noteNumber >= 20 && note.noteNumber <= 27 && (note.device as Minis.MidiDevice)?.channel == 4)
                {
                    _midiController.PlayPad(note.noteNumber - 20, _playerOneAudioSource);
                }

                if (note.noteNumber >= 20 && note.noteNumber <= 27 && (note.device as Minis.MidiDevice)?.channel == 5)
                {
                    _midiController.PlayPad(note.noteNumber - 20, _playerTwoAudioSource);
                }

                // Left play button
                if (note.noteNumber == 16 && (note.device as Minis.MidiDevice)?.channel == 8)
                {
                    if (_canSpawn && GameManager.Instance.isPlayerOneTurn)
                    {
                        _canSpawn = false;
                        _spawnDelayTime = 0;
                        Debug.Log("Spawn!");
                        GameManager.Instance.NextTurn();
                        ShootBlob(_playerOneSpawner.transform.position,
                            GetDirectionVector(_playerOneShootDirection) * _playerOneShootDistance,
                            _nextEmojiType);
                        _nextEmojiType = ChooseRandomEmoji(GameManager.Instance.isPlayerOneTurn);
                        _nextEmoji.GetComponent<BlobPreview>().type = _nextEmojiType;
                    }
                }

                // Right play button
                if (note.noteNumber == 16 && (note.device as Minis.MidiDevice)?.channel == 9)
                {
                    if (_canSpawn && !GameManager.Instance.isPlayerOneTurn)
                    {
                        _canSpawn = false;
                        _spawnDelayTime = 0;
                        Debug.Log("Spawn!");
                        GameManager.Instance.NextTurn();
                        ShootBlob(_playerTwoSpawner.transform.position,
                            GetDirectionVector(_playerTwoShootDirection) * _playerTwoShootDistance,
                            _nextEmojiType);
                        _nextEmojiType = ChooseRandomEmoji(GameManager.Instance.isPlayerOneTurn);
                        _nextEmoji.GetComponent<BlobPreview>().type = _nextEmojiType;
                    }
                }
            };

            midiDevice.onWillNoteOff += (note) =>
            {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));
            };

            midiDevice.onWillControlChange += (control, value) =>
            {
                Debug.Log(string.Format(
                    "Value Change #{0} ({1}) ch:{2} dev:'{3}'",
                    control.controlNumber,
                    value,
                    (control.device as Minis.MidiDevice)?.channel,
                    control.device.description.product
                ));

                // Left platter
                if (control.controlNumber == 6 && (control.device as Minis.MidiDevice)?.channel == 0)
                {
                    var difference = value - _playerOneCachedPlatter;
                    if (difference > 0.9f)
                    {
                        difference -= 1.0f;
                    }

                    if (difference < -0.9f)
                    {
                        difference += 1.0f;
                    }

                    difference *= -0.25f;
                    _playerOneShootDirection += difference;
                    _playerOneCachedPlatter = value;
                }

                // Right platter
                if (control.controlNumber == 6 && (control.device as Minis.MidiDevice)?.channel == 1)
                {
                    var difference = value - _playerTwoCachedPlatter;
                    if (difference > 0.9f)
                    {
                        difference -= 1.0f;
                    }

                    if (difference < -0.9f)
                    {
                        difference += 1.0f;
                    }

                    difference *= -0.25f;
                    _playerTwoShootDirection += difference;
                    _playerTwoCachedPlatter = value;
                }

                // Deck 3 fader
                if (control.controlNumber == 28 && (control.device as Minis.MidiDevice)?.channel == 0)
                {
                    _playerOneShootDistance = value;
                }

                // Deck 4 fader
                if (control.controlNumber == 28 && (control.device as Minis.MidiDevice)?.channel == 1)
                {
                    _playerTwoShootDistance = value;
                }
            };
        };
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(position, GetMousePosition());
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, GetShootVector());
        if (_playerOneSpawner != null && _playerTwoSpawner != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_playerOneSpawner.transform.position,
                (_playerOneSpawner.transform.position + GetDirectionVector(_playerOneShootDirection)) *
                _playerOneShootDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_playerTwoSpawner.transform.position,
                (_playerTwoSpawner.transform.position + GetDirectionVector(_playerTwoShootDirection)) *
                _playerTwoShootDistance);
        }
    }
}