using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    TcpClient client;
    Button payBtn;
    Text PayResult1, PayResult2;
    AliComponent Ali;

    void Start()
    {
        Ali= GameObject.Find("AliComponent").GetComponent<AliComponent>();

        client = new TcpClient();
        Connect();

        payBtn = transform.Find("Panel/AliPayButton").GetComponent<Button>();
        payBtn.onClick.AddListener(Send);
        PayResult1=transform.Find("Panel/PayResult1").GetComponent<Text>();
        PayResult2 = transform.Find("Panel/PayResult2").GetComponent<Text>();

        Ali.aliPayCallBack += GetAliPayResult;

    }

    private void GetAliPayResult(string obj)
    {
        PayResult1.text = $"同步通知:{obj}";
    }

    async void Connect() {
        try
        {
            await client.ConnectAsync("193.112.44.199",7577);
            Receive();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"连接异常:{e.Message}");
        }
    }

    private async void Receive()
    {
        try
        {
            if (client.Connected)
            {
                byte[] buffer = new byte[1024];
                int length= await client.GetStream().ReadAsync(buffer,0, buffer.Length);
                string s = Encoding.UTF8.GetString(buffer,0,length);
                if (s!="error")
                {
                    if (s!="支付成功"&&s!="支付失败")
                    {
                        Ali.AliPay(s);
                    }
                    else
                    {
                        PayResult2.text = $"异步通知结果:{s}";
                    }
                   
                }
                else
                {
                    Debug.LogError("服务器请求预支付单号时候出现异常");
                }
                Receive();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"接收异常:{e.Message}");
           
        }
    }

    string sendStr = "pay";
    private async void Send() {
        try
        {
            if (client.Connected)
            {
                PayResult1.text = "同步通知:";
                PayResult2.text = "异步通知结果:";

                byte[] data = Encoding.UTF8.GetBytes(sendStr);
                await client.GetStream().WriteAsync(data, 0, data.Length);
                Debug.Log("发送成功,请求支付参数");
            }
        
        }
        catch (Exception e)
        {

            Debug.LogError($"发送异常:{e.Message}");
        }
      
    }
}
