using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackyResetScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Reset();
    }
}
