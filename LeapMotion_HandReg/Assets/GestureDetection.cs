using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;
//
using System.IO.Ports;
//using System.Threading;


public class GestureDetection : MonoBehaviour
{
    //serial
    public string portName = "COM1";
    public int baudrate = 115200;
    public Parity parite = Parity.None;
    public int dataBits = 8;
    public StopBits stopbits = StopBits.One;
    SerialPort port;
    //Thread portRev, portDeal;
    //Queue<string> dataQueue;
    //string outStr = string.Empty;
    public byte[] msg;


    private LeapProvider leapProvider;
    private Hand hand;
    private Vector3 oldPos;

    //回调 事件
    public event Action SwipLeft;
    public event Action SwipRight;
    public event Action Fist;
    public event Action YesOk;

    private float timeCounter;
    public float invokeTime = 0.25f;

    public void SendData(byte[] msg)
    {
        //这里要检测一下msg
        if (port.IsOpen)
        {
            port.Write(msg, 0, msg.Length);
        }
    }

    private void Start()
    {
        leapProvider = FindObjectOfType<LeapProvider>();  //查找组件

        //serial
        //dataQueue = new Queue<string>();
        msg = new byte[2];
        portName = "COM1";
        baudrate = 115200;
        Debug.Log(portName);
        port = new SerialPort(portName, baudrate, parite, dataBits, stopbits);
        port.ReadTimeout = 400;
        try
        {
            port.Open();
            Debug.Log("串口打开成功！");
            //portRev = new Thread(PortReceivedThread);
            //portRev.IsBackground = true;
            //portRev.Start();
            //portDeal = new Thread(DealData);
            //portDeal.Start();
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            if (!port.IsOpen)
                Debug.Log("确实没开");
        }

    }



    private void Update()
    {

        if (hand == null)
        {
            hand = leapProvider.Get(Chirality.Right);
            if (hand == null)
            {
                hand = leapProvider.Get(Chirality.Left);
            }
            return;
        }

        if (timeCounter >= 0)
        {
            timeCounter -= Time.deltaTime;
            return;
        }

        if (hand.GetFistStrength() >= 0.88f)   //如果 握拳
        {
            timeCounter = invokeTime;
            Fist?.Invoke();
        }
        if ((hand.PalmVelocity.ToVector3().x - oldPos.x) > 1) // 上一帧手掌的位置 在当前帧的左边  则为向右边挥手
        {
            timeCounter = invokeTime;
            SwipRight?.Invoke();
        }
        else if ((hand.PalmVelocity.ToVector3().x - oldPos.x) < -1) // 上一帧手掌的位置 在当前帧的右边  则为向左边挥手
        {
            timeCounter = invokeTime;
            SwipLeft?.Invoke();
        }

        if (HandYesOk(hand))
        {
            timeCounter = invokeTime;
            YesOk?.Invoke();
        }
    }


    /// <summary>
    /// yeah 手势检测方法
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private bool HandYesOk(Hand hand)
    {
        Vector middle = hand.GetMiddle().Direction;
        Vector thurmb = hand.GetThumb().Direction;
        Vector ring = hand.GetRing().Direction;
        Vector pinky = hand.GetPinky().Direction;
        float angle1 = middle.AngleTo(thurmb);
        float angle2 = middle.AngleTo(ring);
        float angle3 = middle.AngleTo(pinky);

        if (Mathf.Abs(angle1 - 0.7f) < 0.1f && Mathf.Abs(angle2 - 2.7f) < 0.1f && Mathf.Abs(angle3 - 2.7f) < 0.2f)  //如果 中指和 其他几个手指的角度保持某种关系 (数值来自实际测算)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("退出！");
        port.Close();
    }



}
