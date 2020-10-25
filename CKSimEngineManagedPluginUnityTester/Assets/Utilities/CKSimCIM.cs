using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKSimCIM : ICKSimMotor
{
    private float Ks { get; } = 0.30165000f; //0.781046438 angular  // V  //Static friction offset voltage
    private float Kv { get; } = 0.0210063041f;  // V per rad/s
    private float Ka { get; } = 0.686739979f;  // V per rad/s^2
    private float kEpsilon { get; } = 1e-6f;  //RPM
    private float SpeedPerVolt {
        get {
            return 1.0f / Kv;
        }
    }
    public float LeverRadius { get; set; } = 0.118f; //Meters
    public float SystemInertia { get; set; } = 20.0f; //kg
    private float TorquePerVolt { 
        get {
            return (float)(Math.Pow(LeverRadius, 2) * SystemInertia / (2.0f * Ka));
        }
    } // (LeverRadius^2 * Inertia) / (2Ka) // Units in meters

    public float CurrentSpeedRadPerSec { get; private set; }
    public float CurrentSpeedRPM {
        get {
            return CurrentSpeedRadPerSec * 9.5492965964254f;
        }
    }

    private float prevTime;
    public override float GetOutputTorque(float inputVoltage)
    {
        if (prevTime == 0)
        {
            prevTime = Time.time;
            return 0;
        }
        //Debug.Log($@"Timestep {Time.time - prevTime}");
        CurrentSpeedRadPerSec += (GetEffectiveVoltageAfterFriction(inputVoltage) - (CurrentSpeedRadPerSec / SpeedPerVolt)) * (Ka);
        Debug.Log($@"Effective Voltage {GetEffectiveVoltageAfterFriction(inputVoltage)}, CurrentSpeedVolt {(CurrentSpeedRadPerSec / SpeedPerVolt)}, CurrSpeed {CurrentSpeedRPM}, Output Torque {GetTorqueForVoltage(inputVoltage)}");
        prevTime = Time.time;
        return GetTorqueForVoltage(inputVoltage);
    }

    private float GetTorqueForVoltage(float voltage)
    {
        return TorquePerVolt * (-CurrentSpeedRadPerSec / SpeedPerVolt + GetEffectiveVoltageAfterFriction(voltage));
    }

    private float GetEffectiveVoltageAfterFriction(float inputVoltage)
    {
        float effectiveVoltage = inputVoltage;
        if (CurrentSpeedRadPerSec > kEpsilon)
        {
            // Forward motion, rolling friction.
            effectiveVoltage -= Ks;
        }
        else if (CurrentSpeedRadPerSec < -kEpsilon)
        {
            // Reverse motion, rolling friction.
            effectiveVoltage += Ks;
        }
        else if (inputVoltage > kEpsilon)
        {
            // System is static, forward torque.
            effectiveVoltage = Math.Max(0.0f, inputVoltage - Ks);
        }
        else if (inputVoltage < -kEpsilon)
        {
            // System is static, reverse torque.
            effectiveVoltage = Math.Min(0.0f, inputVoltage + Ks);
        }
        else
        {
            // System is idle.
            return 0.0f;
        }
        return effectiveVoltage;
    }
}
