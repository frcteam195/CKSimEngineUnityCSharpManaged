using CKSimEngineManagedPlugin.Utils;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using ZeroMQ;

namespace CKSimEngineManagedPlugin
{
    public class CKSimNetCode
    {
        public static CKSimNetCode Instance { get; } = new CKSimNetCode();

        private readonly ZContext zmqContext = new ZContext();
        private ZSocket zmqPubSocket;
        private ZSocket zmqRepSocket;
        private readonly object pubLockObj = new object();
        private readonly object repLockObj = new object();
        private int pubFailCount;
        private int repFailCount;
        private readonly ElapsedTimer repTimer = new ElapsedTimer();
        private readonly ElapsedTimer pubTimer = new ElapsedTimer();
        JobHandle repHandle;
        JobHandle pubHandle;

        private readonly Dictionary<int, float> motorMap = new Dictionary<int, float>();
        private readonly Dictionary<int, float> encoderMap = new Dictionary<int, float>();
        private readonly Dictionary<int, float> accelerometerMap = new Dictionary<int, float>();
        private readonly Dictionary<int, float> gyroscopeMap = new Dictionary<int, float>();
        private readonly Dictionary<int, float> advancedMap = new Dictionary<int, float>();

        public struct RepJob : IJob
        {
            public void Execute()
            {
                Instance.RunRepJob();
            }
        }

        public struct PubJob : IJob
        {
            public void Execute()
            {
                Instance.RunPubJob();
            }
        }

        private CKSimNetCode()
        {
            ReInitPubSocket();
            ReInitRepSocket();
            repTimer.Start();
            pubTimer.Start();
        }

        public void Start()
        {
            repHandle = new RepJob().Schedule();
            pubHandle = new PubJob().Schedule();
        }

        public void Complete()
        {
            repHandle.Complete();
            pubHandle.Complete();
        }

        public void Update()
        {
            Complete();
            Start();
        }

        ~CKSimNetCode()
        {
            zmqPubSocket?.TryCloseSocket();
            zmqRepSocket?.TryCloseSocket();
            zmqContext?.Dispose();
        }

        public void RunRepJob()
        {
            if (repTimer.HasElapsed() >= 1)
            {
                if (zmqRepSocket != null)
                {
                    using (ZFrame request = zmqRepSocket.ReceiveFrame(ZSocketFlags.DontWait, out ZError zErr))
                    {
                        if (zErr == ZError.None && request.Length > 0)
                        {
                            try
                            {
                                byte[] b = request.Read();
                                ControlMessage cM = ControlMessage.Parser.ParseFrom(b);

                                lock (repLockObj)
                                {
                                    foreach (var motor in cM.Motors)
                                    {
                                        motorMap[motor.Id] = motor.Value;
                                    }
                                }
                                zmqRepSocket.Send(new ZFrame("A"));
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    Interlocked.Increment(ref repFailCount);
                    if (repFailCount >= 100)
                    {
                        ReInitRepSocket();
                    }
                }
                repTimer.Start();
            }
        }

        public void RunPubJob()
        {
            if (pubTimer.HasElapsed() >= 10)
            {
                if (zmqPubSocket != null)
                {
                    StatusMessage statusMsg = new StatusMessage();
                    lock (pubLockObj)
                    {
                        foreach (KeyValuePair<int, float> x in encoderMap)
                        {
                            ValueMessage v = new ValueMessage { Id = x.Key, Value = x.Value };
                            statusMsg.Encoders.Add(v);
                        }
                        foreach (KeyValuePair<int, float> x in accelerometerMap)
                        {
                            ValueMessage v = new ValueMessage { Id = x.Key, Value = x.Value };
                            statusMsg.Accelerometers.Add(v);
                        }
                        foreach (KeyValuePair<int, float> x in gyroscopeMap)
                        {
                            ValueMessage v = new ValueMessage { Id = x.Key, Value = x.Value };
                            statusMsg.Gyroscopes.Add(v);
                        }
                        foreach (KeyValuePair<int, float> x in advancedMap)
                        {
                            ValueMessage v = new ValueMessage { Id = x.Key, Value = x.Value };
                            statusMsg.Advanced.Add(v);
                        }
                    }
                    Console.WriteLine("Sending");
                    zmqPubSocket.Send(new ZFrame(statusMsg.ToByteArray()));
                }
                else
                {
                    Interlocked.Increment(ref pubFailCount);
                    if (pubFailCount >= 100)
                    {
                        ReInitPubSocket();
                    }
                }
                pubTimer.Start();
            }
        }

        private void ReInitPubSocket()
        {
            zmqPubSocket?.TryCloseSocket();

            zmqPubSocket = new ZSocket(zmqContext, ZSocketType.PUB);
            zmqPubSocket.Bind("tcp://*:10502", out ZError zErr);
            if (zErr != ZError.None)
            {
                UnityEngine.Debug.LogError(zErr.ToString());
                zmqPubSocket?.TryCloseSocket();
                zmqPubSocket = null;
            }
            Interlocked.Exchange(ref pubFailCount, 0);
        }

        private void ReInitRepSocket()
        {
            zmqRepSocket?.TryCloseSocket();

            zmqRepSocket = new ZSocket(zmqContext, ZSocketType.REP);
            zmqRepSocket.Bind("tcp://*:10501", out ZError zErr);
            if (zErr != ZError.None)
            {
                UnityEngine.Debug.LogError(zErr.ToString());
                zmqRepSocket?.TryCloseSocket();
                zmqRepSocket = null;
            }
            Interlocked.Exchange(ref repFailCount, 0);
        }

        public float GetMotor(int motorId)
        {
            float val;
            lock (repLockObj)
            {
                val = motorMap.TryGetValue(motorId, out val) ? val : 0;
            }
            return val;
        }

        public void SetEncoder(int encoderId, float value)
        {
            lock (pubLockObj)
            {
                encoderMap[encoderId] = value;
            }
        }

        public void SetAccelerometer(int accelerometerId, float value)
        {
            lock (pubLockObj)
            {
                accelerometerMap[accelerometerId] = value;
            }
        }

        public void SetGyroscope(int gyroscopeId, float value)
        {
            lock (pubLockObj)
            {
                gyroscopeMap[gyroscopeId] = value;
            }
        }

        public void SetAdvanced(int advancedId, float value)
        {
            lock (pubLockObj)
            {
                advancedMap[advancedId] = value;
            }
        }
    }
}
