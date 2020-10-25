using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICKSimMotor
{
    public abstract float GetOutputTorque(float inputVoltage);
}
