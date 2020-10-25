using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CKSimGlobalConstants
{
    public static float SystemVoltage { get; } = 12.0f;
    public static float TimeStep {
        get {
            return Time.fixedDeltaTime;
        }
    }


}
