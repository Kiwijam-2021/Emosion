using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomWord : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    
    public List<string> positiveWords =
        new List<string>()
        {
            "Lovely",
            "Sweet",
            "Jolly",
            "Beautiful"
        };

    public List<string> negativeWords =
        new List<string>()
        {
            "Psycho",
            "Terrible",
            "Nasty",
            "Cruel",
            "Horrible",
            "Sadistic",
            "Lonely"
        };

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        var happyScore = GameManager.Instance.happyScore;
        var unhappyScore = GameManager.Instance.unhappyScore;
        string text;

        if (happyScore == unhappyScore)
        {
            text = "Wow, hahah";
        }
        else if (GameManager.Instance.happyScore > GameManager.Instance.unhappyScore)
        {
            text = positiveWords.GetRandomItem();
        }
        else
        {
            text = negativeWords.GetRandomItem();
        }
        
        tmp.SetText(text);
    }
}