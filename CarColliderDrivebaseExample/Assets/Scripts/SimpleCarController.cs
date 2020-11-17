using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CKSimEngineManagedPlugin;

[System.Serializable]
public class DifferentialDrive
{
    public List<WheelCollider> leftWheels;
    public List<WheelCollider> rightWheels;

    public override string ToString()
    {
        string s = "";

        leftWheels.ForEach((w) => s += $@"Wheel {w.motorTorque} ");
        rightWheels.ForEach((w) => s += $@" Wheel {w.motorTorque} ");
        s += "\n";
        return s;
    }
}

public class SimpleCarController : MonoBehaviour
{
    public DifferentialDrive differentialDrive;
    public float maxMotorTorque;

    public void VisualUpdate(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation * new Quaternion(90, 90, 0, 0);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < differentialDrive.leftWheels.Count; i++)
        {
            differentialDrive.leftWheels[i].motorTorque = CKSimNetCode.Instance.GetMotor(i) * maxMotorTorque;
            VisualUpdate(differentialDrive.leftWheels[i]);
        }
        for (int i = differentialDrive.leftWheels.Count; i < differentialDrive.rightWheels.Count + differentialDrive.leftWheels.Count; i++)
        {
            differentialDrive.rightWheels[i - differentialDrive.leftWheels.Count].motorTorque = CKSimNetCode.Instance.GetMotor(i) * maxMotorTorque;
            VisualUpdate(differentialDrive.rightWheels[i - differentialDrive.leftWheels.Count]);
        }
    }

    //public void FixedUpdate()
    //{
    //    float motor = maxMotorTorque * Input.GetAxis("Vertical");
    //    float steering = maxMotorTorque * Input.GetAxis("Horizontal");
    //    differentialDrive.leftWheels.ForEach((w) => {
    //        w.motorTorque = motor + steering;
    //        VisualUpdate(w);
    //    });
    //    differentialDrive.rightWheels.ForEach((w) => {
    //        w.motorTorque = motor - steering;
    //        VisualUpdate(w);
    //    });
    //    //Debug.Log(differentialDrive.ToString());
    //}
}
