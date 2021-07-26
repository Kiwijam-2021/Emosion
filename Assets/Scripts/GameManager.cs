using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private GameManager _instance;

    public static GameManager Instance { get; private set; }

    public const string PLAYER_1_TAG = "P1_Timer";
    public const string PLAYER_2_TAG = "P2_Timer";

    public Timer playerOneTimer;
    public Timer playerTwoTimer;
    public int happyScore;
    public int unhappyScore;
    public bool finished;

    [HideInInspector] public bool isPlayerOneTurn = true;

    [HideInInspector] public AudioSource audioSource;

    public GameObject gameOverCanvas;
    private AudioListener _listener;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
        _listener = GetComponent<AudioListener>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        happyScore = 0;
        unhappyScore = 0;
        _listener.enabled = true;
        SetTimers();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!finished && playerOneTimer.Finished() && playerTwoTimer.Finished())
        {
            finished = true;
            FinishGame();
        }
    }

    private void ResetTimers()
    {
        if (playerOneTimer is not null)
        {
            playerOneTimer.ResetTimer();
        }

        if (playerTwoTimer is not null)
        {
            playerTwoTimer.ResetTimer();
        }
    }

    private void SetTimers()
    {
        if (isPlayerOneTurn)
        {
            playerOneTimer.StartTimer();
            playerTwoTimer.StopTimer();
        }
        else
        {
            playerOneTimer.StopTimer();
            playerTwoTimer.StartTimer();
        }
    }

    public void NextTurn()
    {
        if (playerOneTimer.Finished() && playerTwoTimer.Finished())
        {
            FinishGame();
            return;
        }

        if (playerOneTimer.Finished())
        {
            isPlayerOneTurn = false;
        }
        else if (playerTwoTimer.Finished())
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
        _listener.enabled = false;
        gameOverCanvas.SetActive(true);
    }
}