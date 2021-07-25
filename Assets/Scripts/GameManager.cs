using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager _instance;

    public static GameManager Instance { get; private set; }

    public const string PLAYER_1_TAG = "P1_Timer";
    public const string PLAYER_2_TAG = "P2_Timer";

    private Timer _playerOneTimer;
    private Timer _playerTwoTimer;
    public int happyScore;
    public int unhappyScore;
    public bool finished;

    [HideInInspector] public bool isPlayerOneTurn = true;

    [HideInInspector] public AudioSource audioSource;

    private GameObject _gameOverCanvas;
    public AudioListener listener;


    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(this);
        };
        Instance = this;

        var t1 = GameObject.FindWithTag(PLAYER_1_TAG);
        _playerOneTimer = t1.GetComponent<Timer>();

        var t2 = GameObject.FindWithTag(PLAYER_2_TAG);
        _playerTwoTimer = t2.GetComponent<Timer>();

        audioSource = GetComponent<AudioSource>();
        _gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
        _gameOverCanvas.SetActive(false);
        
        Debug.Log($"GameOverCanvas {_gameOverCanvas}");
    }

    // Start is called before the first frame update
    private void Start()
    {
        happyScore = 0;
        unhappyScore = 0;
        SetTimers();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!finished && _playerOneTimer.Finished() && _playerTwoTimer.Finished())
        {
            finished = true;
            FinishGame();
        }
    }

    private void SetTimers()
    {
        if (isPlayerOneTurn)
        {
            _playerOneTimer.StartTimer();
            _playerTwoTimer.StopTimer();
        }
        else
        {
            _playerOneTimer.StopTimer();
            _playerTwoTimer.StartTimer();
        }
    }

    public void NextTurn()
    {
        if (_playerOneTimer.Finished() && _playerTwoTimer.Finished())
        {
            FinishGame();
            return;
        }

        if (_playerOneTimer.Finished())
        {
            isPlayerOneTurn = false;
        }
        else if (_playerTwoTimer.Finished())
        {
            isPlayerOneTurn = true;
        }
        else
        {
            isPlayerOneTurn = !isPlayerOneTurn;
        }

        SetTimers();
    }

    private void FinishGame()
    {
        Debug.Log("FinishGame");
        finished = true;
        listener.enabled = false;
        _gameOverCanvas.SetActive(true);
    }
    
}