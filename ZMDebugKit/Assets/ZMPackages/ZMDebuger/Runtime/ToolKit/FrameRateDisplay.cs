/*------------------------------------------------------------------
*
* Title: 毕业级日志系统
*
* Description: 支持本地文件写入、自定义颜色日志、FPS实时显示、手机日志运行时查看、日志代码编译剔除、ProtoBuff转Json、日志重定向
*
* Author: https://www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b 铸梦
*
* Date: 2023.8.13
*
* Modify:
-------------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

namespace ZM.DebugerKit
{
    public class FrameRateDisplay : MonoBehaviour
    {
        float deltaTime = 0.0f;

        GUIStyle mStyle;

        void Awake()
        {
            mStyle = new GUIStyle();
            mStyle.alignment = TextAnchor.UpperLeft;
            mStyle.normal.background = null;
            mStyle.fontSize = 35;
            mStyle.normal.textColor = Color.red;
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            Rect rect = new Rect(0, 0, 500, 300);
            float fps = 1.0f / deltaTime;
            string text = $" FPS:{fps:N0} ";
            GUI.Label(rect, text, mStyle);
        }
    }
}