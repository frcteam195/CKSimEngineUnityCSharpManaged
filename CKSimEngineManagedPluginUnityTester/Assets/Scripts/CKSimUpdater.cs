using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CKSimEngineManagedPlugin;
using Unity.Jobs;

public class CKSimUpdater : MonoBehaviour
{
    private CKSimNetCode ckSim = CKSimNetCode.Instance;

    // Start is called before the first frame update
    void Start()
    {
        CKSimNetCode.Instance.Start();
        InvokeRepeating("UpdateCKSim", 0, 0.010f);
    }

    void UpdateCKSim()
    {
        CKSimNetCode.Instance.Update();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {

    }

    void OnDestroy()
    {
        CancelInvoke();
    }
}
