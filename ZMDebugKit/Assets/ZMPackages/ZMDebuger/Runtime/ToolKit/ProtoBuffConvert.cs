/*------------------------------------------------------------------
*
* Title: ��ҵ����־ϵͳ 
*
* Description: ֧�ֱ����ļ�д�롢�Զ�����ɫ��־��FPSʵʱ��ʾ���ֻ���־����ʱ�鿴����־��������޳���ProtoBuffתJson����־�ض���
* 
* Author: https://www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b ����
*
* Date: 2023.8.13
*
* Modify: 
-------------------------------------------------------------------*/
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.DebugerKit
{
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
        Debuger.Log(JsonConvert.SerializeObject(proto, Formatting.Indented));
    }
}
}
