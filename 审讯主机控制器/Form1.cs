using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 审讯主机控制器
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 登录的摄像头
        /// </summary>
        private Int32 m_lUserID = -1;
        private Int32 m_lRealHandle = -1;
        /// <summary>
        /// 正在录制
        /// </summary>
        private bool Recording = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (textBoxIP.Text == "" || textBoxPort.Text == "" ||
                textBoxUserName.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("请输入IP、端口、用户名和密码！");
                return;
            }
            if (m_lUserID < 0)
            {
                string DVRIPAddress = textBoxIP.Text; //设备IP地址或者域名
                Int16 DVRPortNumber = Int16.Parse(textBoxPort.Text);//设备服务端口号
                string DVRUserName = textBoxUserName.Text;//设备登录用户名
                string DVRPassword = textBoxPassword.Text;//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //登录成功
                    MessageBox.Show("Login Success!");
                    btnLogin.Text = "Logout";
                }

            }
            else
            {
                //注销登录 Logout the device
                if (m_lRealHandle >= 0)
                {
                    MessageBox.Show("请先停止预览");
                    return;
                }

                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lUserID = -1;
                btnLogin.Text = "Login";
            }
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_lUserID < 0)
            {
                MessageBox.Show("请先登录设备");
                return;
            }

            if (!Recording)
            {
                if (!CHCNetSDK.NET_DVR_StartDVRRecord(m_lUserID, 0xffff, 0))
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "开始录制异常, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                button1.Text = "停止";
                Recording = true;
            }
            else
            {
                if (!CHCNetSDK.NET_DVR_StopDVRRecord(m_lUserID, 0xffff))
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    var str = "停止录制异常, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                button1.Text = "开始";
                Recording = false;
            }
        }
    }
}
