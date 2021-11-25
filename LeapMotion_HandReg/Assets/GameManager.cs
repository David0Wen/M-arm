using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private Text t_old;               //之前操作的字体组件
    private int index = 0;            //字体组件数组下标
    private Coroutine bulingbuling;   //协程字段
    private GestureDetection gestureDetection;    //手势检测的实例

    public Text[] t_Nums;
    public Text Tips;
    public Color normal;     //正常的颜色
    public Color hightLight; //高亮
    public Color selectLight; //选中后的颜色
    public Color YesOkLight;  //yeah 颜色

    private float timeCounter = 0;  //tip 的 倒计时
    public float tipShowTime = 1;   //tip 的 显示时长


    private void Awake()
    {
        //查找组件
        gestureDetection = FindObjectOfType<GestureDetection>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //注册回调
        gestureDetection.SwipLeft += Priv;
        gestureDetection.SwipRight += Next;
        gestureDetection.Fist += Select;
        gestureDetection.YesOk += YesOkCallback;
        //数值初始化
        for (int i = 0; i < t_Nums.Length; i++)
        {
            t_Nums[i].color = normal;
        }
        Change(index);
    }


    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Priv();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Next();
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Select();
        }
#endif
        //倒计时
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            return;
        }
        else if (Tips.text != string.Empty)
        {
            Tips.text = string.Empty;
        }
    }
    /// <summary> 
    ///  向左挥手 回调
    /// </summary>
    private void Priv()
    {
        index = Mathf.Clamp(--index, 0, t_Nums.Length - 1);
        SetTips("左挥");
        Change(index);

        gestureDetection.msg[0] = 0;
        gestureDetection.msg[1] = 1;
        gestureDetection.SendData(gestureDetection.msg);
        Debug.Log("发送了01");
    }
    /// <summary>
    ///  向右挥手 回调
    /// </summary>
    private void Next()
    {
        index = Mathf.Clamp(++index, 0, t_Nums.Length - 1);
        SetTips("右挥");
        Change(index);

        gestureDetection.msg[0] = 1;
        gestureDetection.msg[1] = 0;
        gestureDetection.SendData(gestureDetection.msg);
        Debug.Log("发送了10");
    }


    /// <summary>
    ///  握拳 回调
    /// </summary>
    private void Select()
    {
        if (bulingbuling != null)
        {
            StopCoroutine(bulingbuling);
        }
        SetTips("握拳");
        t_Nums[index].color = hightLight;
        bulingbuling = StartCoroutine(BulingBuling(selectLight));

        gestureDetection.msg[0] = gestureDetection.msg[1] = 0;
        gestureDetection.SendData(gestureDetection.msg);
        Debug.Log("发送了00");
    }
    /// <summary>
    /// yeah 手势回调
    /// </summary>
    private void YesOkCallback()
    {
        SetTips("YesOk");
        t_Nums[index].color = hightLight;
        bulingbuling = StartCoroutine(BulingBuling(YesOkLight));

        gestureDetection.msg[0] = gestureDetection.msg[1] = 1;
        gestureDetection.SendData(gestureDetection.msg);
        Debug.Log("发送了11");
    }

    /// <summary>
    /// 设置 tip 的值
    /// </summary>
    /// <param name="tip"></param>
    private void SetTips(string tip)
    {
        Tips.text = tip;
        timeCounter = tipShowTime;
    }
    /// <summary>
    /// 退出
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }



    /// <summary>
    /// 变色操作
    /// </summary>
    /// <param name="index"></param>
    private void Change(int index)
    {
        if (bulingbuling != null)
        {
            StopCoroutine(bulingbuling);
        }
        if (t_old != null)
        {
            t_old.color = normal;   //将上一个操作的字体组件颜色重置
        }
        t_Nums[index].color = hightLight;  //让选中的字体组件颜色高亮
        t_old = t_Nums[index];   //记录这个组件
    }

    private IEnumerator BulingBuling(Color col)
    {
        int count = 0;
        Color tempColor = col;
        while (count <= 5)
        {
            if (t_Nums[index].color != tempColor)
            {
                t_Nums[index].color = Color.Lerp(t_Nums[index].color, tempColor, Time.deltaTime * 75);
            }
            else
            {
                tempColor = tempColor.Equals(col) ? hightLight : col;
                count++;
            }
            yield return null;
        }
        t_Nums[index].color = hightLight;
    }




}
