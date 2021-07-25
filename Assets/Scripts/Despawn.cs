using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float delay = 1f;
    private float duration;

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (duration >= delay)
        {
            Destroy(gameObject);
        }
    }
}