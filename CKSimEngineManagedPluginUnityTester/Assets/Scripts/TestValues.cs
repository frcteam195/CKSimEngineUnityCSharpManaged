using CKSimEngineManagedPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestValues : MonoBehaviour
{
    CKSimCIM c = new CKSimCIM();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private uint counter;
    void FixedUpdate()
    {
        //CKSimNetCode.Instance.SetAccelerometer(0, 0.4f);
        //Debug.Log(CKSimNetCode.Instance.GetMotor(0));
        c.GetOutputTorque(12);

        if (counter++ % 100 == 0)
        {
            //Debug.Log(c.CurrentSpeedRPM);
        }
    }
}
