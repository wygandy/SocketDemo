/*
 * 
 * User: Andy
 * Date: 2013-6-13
 * Time: 16:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Andy.Common.LightSocketLibrary;
using Andy.Socket.Common.Com;
namespace Client
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
           
		}

        private SimpleTcpClient _client = new SimpleTcpClient("127.0.0.1", 20001);
        private void button1_Click(object sender, EventArgs e) {

            byte[] buf = CommonHelper.GetRawStream(new Andy.Socket.Common.Models.CommandBase { CommandID = 101, CommandLength = 100 });
            _client.Send(buf);
        }

        private void button2_Click(object sender, EventArgs e) {
            byte[] buf = CommonHelper.GetRawStream(new Andy.Socket.Common.Models.CommandBase { CommandID = 102, CommandLength = 100 });
            _client.Send(buf);
        }

        private void button3_Click(object sender, EventArgs e) {
            byte[] buf = CommonHelper.GetRawStream(new Andy.Socket.Common.Models.CommandBase { CommandID = 103, CommandLength = 100 });
            _client.Send(buf);
        }

        private void button4_Click(object sender, EventArgs e) {
            _client.Connect();

            System.Threading.Thread.Sleep(500);
            _client.DataReceived += new Action<byte[]>(_client_DataReceived);
            _client.StartReceiveData();
        }

        void _client_DataReceived(byte[] obj) {
            string temp_string = System.Text.Encoding.UTF8.GetString(obj);
            this.BeginInvoke( (Action<string> )showinformation , new object[]{temp_string});
        }

        private void showinformation(string str) {
            this.label1.Text = str;
        }
	}


    public class CommonHelper {
        public static byte[] GetRawStream(Andy.Socket.Common.Models.CommandBase baseCommand) {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                MemoryStreamUtility.Instance.WriteUint32(baseCommand.CommandID, ms);
                MemoryStreamUtility.Instance.WriteUint32(baseCommand.CommandLength, ms);

                byte[] buff = new byte[8];
                Array.Copy(ms.GetBuffer(), 0, buff, 0, buff.Length);

                return buff;
            }
        }
    }
}
