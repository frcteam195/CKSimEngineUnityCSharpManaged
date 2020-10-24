using CKSimEngineManagedPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestValues : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        CKSimNetCode.Instance.SetAccelerometer(0, 0.4f);
        Debug.Log(CKSimNetCode.Instance.GetMotor(0));
    }
}
