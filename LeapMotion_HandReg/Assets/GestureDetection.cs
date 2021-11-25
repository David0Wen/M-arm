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

    //�ص� �¼�
    public event Action SwipLeft;
    public event Action SwipRight;
    public event Action Fist;
    public event Action YesOk;

    private float timeCounter;
    public float invokeTime = 0.25f;

    public void SendData(byte[] msg)
    {
        //����Ҫ���һ��msg
        if (port.IsOpen)
        {
            port.Write(msg, 0, msg.Length);
        }
    }

    private void Start()
    {
        leapProvider = FindObjectOfType<LeapProvider>();  //�������

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
            Debug.Log("���ڴ򿪳ɹ���");
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
                Debug.Log("ȷʵû��");
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

        if (hand.GetFistStrength() >= 0.88f)   //��� ��ȭ
        {
            timeCounter = invokeTime;
            Fist?.Invoke();
        }
        if ((hand.PalmVelocity.ToVector3().x - oldPos.x) > 1) // ��һ֡���Ƶ�λ�� �ڵ�ǰ֡�����  ��Ϊ���ұ߻���
        {
            timeCounter = invokeTime;
            SwipRight?.Invoke();
        }
        else if ((hand.PalmVelocity.ToVector3().x - oldPos.x) < -1) // ��һ֡���Ƶ�λ�� �ڵ�ǰ֡���ұ�  ��Ϊ����߻���
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
    /// yeah ���Ƽ�ⷽ��
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

        if (Mathf.Abs(angle1 - 0.7f) < 0.1f && Mathf.Abs(angle2 - 2.7f) < 0.1f && Mathf.Abs(angle3 - 2.7f) < 0.2f)  //��� ��ָ�� ����������ָ�ĽǶȱ���ĳ�ֹ�ϵ (��ֵ����ʵ�ʲ���)
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
        Debug.Log("�˳���");
        port.Close();
    }



}
