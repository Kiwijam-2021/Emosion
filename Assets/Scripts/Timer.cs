using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    public double initialSeconds = 60;
    private double _seconds;

    private bool _isRunning = false;
    private bool _isFinished = false;

    // Start is called before the first frame update
    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        _seconds = initialSeconds;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning) return;
        
        _seconds -= Time.deltaTime;
        
        if (_seconds < 0)
        {
            _isFinished = true;
        }

        _seconds = Math.Max(0, _seconds);

        var timespan = TimeSpan.FromSeconds(_seconds).ToString("ss\\.fff");
        _tmp.SetText($"<mspace=0.7em>{timespan}</mspace>");

        if (_isFinished)
        {
            StopTimer();
            GameManager.Instance.NextTurn();
        }
    }

    public bool Finished()
    {
        return _isFinished;
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }
}