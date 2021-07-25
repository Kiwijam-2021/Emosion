using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public bool isHappyScore = true;
    private TextMeshProUGUI tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var instance = GameManager.Instance;
        var score = isHappyScore ? instance.happyScore : instance.unhappyScore;
        tmp.SetText($"{score}");
    }
}
