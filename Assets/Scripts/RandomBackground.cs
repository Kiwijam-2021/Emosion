using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    private SpriteRenderer _spriteRenderer;
    private List<GameObject> backgrounds = new();
    
    private float duration = 0;
    private float delay = 1;

    private GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            backgrounds.Add(child.gameObject);
        }

        item = backgrounds.GetRandomItem();
        item.SetActive(true);
    }

    // void Update()
    // {
    //     duration += Time.deltaTime;
    //     if (duration > delay)
    //     {
    //         duration = 0;
    //
    //         var nextItem = backgrounds.GetRandomItem();
    //         while (nextItem.GetInstanceID() == item.GetInstanceID())
    //         {
    //             nextItem = backgrounds.GetRandomItem();
    //         }
    //
    //         nextItem.SetActive(true);
    //         item.SetActive(false);
    //         item = nextItem;
    //     }
    // }
}