/*------------------------------------------------------------------
*
* Title: ��ҵ����־ϵͳ 
*
* Description: ֧�ֱ����ļ�д�롢�Զ�����ɫ��־��FPSʵʱ��ʾ���ֻ���־����ʱ�鿴����־��������޳���ProtoBuffתJson����־�ض���
* 
* Author: https://www.taikr.com/user/63798c7981862239d5b3da44d820a7171f0ce14d ����
*
* Date: 2023.8.13
*
* Modify: 
-------------------------------------------------------------------*/
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Protobuff תΪJosn �ַ����������д�ӡ
/// </summary>
public class ProtoBuffConvert 
{
    /// <summary>
    /// Protobuff תΪJosn �ַ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proto"></param>
    public static void ToJson<T>(T proto)
    {
        Debuger.Log(JsonConvert.SerializeObject(proto));
    }
}
