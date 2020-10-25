using CKSimEngineManagedPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rigi : MonoBehaviour
{
    CKSimCIM c = new CKSimCIM();
    private Rigidbody mRigidBody { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float trq = c.GetOutputTorque(12);
        mRigidBody.AddForce(Vector3.up * trq, ForceMode.Force);
    }
}
