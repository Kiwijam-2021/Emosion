using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private Rigidbody2D _rb;

    [ReadOnly(true)]
    public double velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        this.velocity = _rb.velocity.magnitude;
    }
}
