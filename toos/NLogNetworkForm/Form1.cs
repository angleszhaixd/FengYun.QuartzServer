using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CacheManager.Logging.NLog.WinFrom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>
        private UdpClient udpcSend;
        /// <summary>
        /// 用于UDP接收的网络服务类
        /// </summary>
        private UdpClient udpcRecv;

        private string ListenerIP = ConfigurationManager.AppSettings["ListenerIP"].ToString();
        private int ListenerPort =Convert.ToInt32(ConfigurationManager.AppSettings["ListenerPort"].ToString());

        #region 接收信息
        /// <summary>
        /// 开关：在监听UDP报文阶段为true，否则为false
        /// </summary>
        bool IsUdpcRecvStart = false;
        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        Thread thrRecv;

        /// <summary>
        /// 按钮：接收数据开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecv_Click(object sender, EventArgs e)
        {
            if (!IsUdpcRecvStart) // 未监听的情况，开始监听
            {
                IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse(ListenerIP), ListenerPort); // 本机IP和监听端口号

                udpcRecv = new UdpClient(localIpep);

                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();

                IsUdpcRecvStart = true;
                ShowMessage(txtRecvMssg, "UDP监听器已成功启动");
            }
            else                  // 正在监听的情况，终止监听
            {
                thrRecv.Abort(); // 必须先关闭这个线程，否则会异常
                udpcRecv.Close();

                IsUdpcRecvStart = false;
                //ResetTextBox(txtRecvMssg);
                ShowMessage(txtRecvMssg, "UDP监听器已成功关闭");
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);

                    ShowMessage(txtRecvMssg,string.Format("[{0}] {1}{2}", remoteIpep.ToString(), message,Environment.NewLine));
                }
                catch (Exception ex)
                {
                    ShowMessage(txtRecvMssg, ex.Message);
                    break;
                }
            }
        }
        #endregion

        #region 发送消息
        /*
        /// <summary>
        /// 按钮：发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSendMssg.Text))
            {
                MessageBox.Show("请先输入待发送内容");
                return;
            }

            // 匿名发送
            //udpcSend = new UdpClient(0);             // 自动分配本地IPv4地址

            // 实名发送
            IPEndPoint localIpep = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 12345); // 本机IP，指定的端口号
            udpcSend = new UdpClient(localIpep);

            Thread thrSend = new Thread(SendMessage);
            //thrSend.Start(txtSendMssg.Text);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessage(object obj)
        {
            string message = (string)obj;
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);

            IPEndPoint remoteIpep = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 8848); // 发送到的IP地址和端口号

            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();

            //ResetTextBox(txtSendMssg);
        }
        */
        #endregion


        // 向TextBox中添加文本
        delegate void ShowMessageDelegate(TextBox txtbox, string message);
        private void ShowMessage(TextBox txtbox, string message)
        {
            if (txtbox.InvokeRequired)
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                txtbox.Invoke(showMessageDelegate, new object[] { txtbox, message });
            }
            else
            {
                //txtbox.Text += message + "\r\n";
                //滚动条保持在最下面
                txtbox.AppendText(message + "\r\n");
            }
        }

        // 清空指定TextBox中的文本
        delegate void ResetTextBoxDelegate(TextBox txtbox);
        private void ResetTextBox(TextBox txtbox)
        {
            if (txtbox.InvokeRequired)
            {
                ResetTextBoxDelegate resetTextBoxDelegate = ResetTextBox;
                txtbox.Invoke(resetTextBoxDelegate, new object[] { txtbox });
            }
            else
            {
                txtbox.Text = "";
            }
        }

        /// <summary>
        /// 关闭程序，强制退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetTextBox(txtRecvMssg);
        }
    }
}
