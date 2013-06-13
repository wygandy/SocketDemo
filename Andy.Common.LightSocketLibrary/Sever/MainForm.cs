/*
 * 
 * User: Andy
 * Date: 2013-6-13
 * Time: 16:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Andy.Common.LightSocketLibrary;
using Andy.Socket.Common.Com;
using Andy.Socket.Common.Models;
namespace Sever
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

            _server.DataReceived += new Action<byte[]>(_server_DataReceived);
            this.label1.Text = "Information....";
		}

        void _server_DataReceived(byte[] obj) {
            try {
                CommandBase c = CommonHelper.BuildData(obj);
                this.BeginInvoke((Action<uint,uint>)SHowLabelInformation, new object[] { c.CommandID, c.CommandLength });
                _server.Send(System.Text.Encoding.UTF8.GetBytes("Server receive information" +c.CommandID.ToString()));
            }
            catch (Exception x) {
                MessageBox.Show(x.Message);
            }

        }

        private void SHowLabelInformation(uint id, uint length) {
            this.label1.Text = "命令号: " + id.ToString() + "\n" + "命令长度:" + length.ToString();
        }


        private SimpleTcpListen _server = new SimpleTcpListen("127.0.0.1", 20001);
        private void button1_Click(object sender, EventArgs e) {
            _server.StartListen();
        }

	}

    public class CommonHelper {
        public static CommandBase BuildData(byte[] buffer) {
            CommandBase clsobject = new CommandBase();
            clsobject.CommandID = MemoryStreamUtility.Instance.ReadUint32(buffer, 0);
            clsobject.CommandLength = MemoryStreamUtility.Instance.ReadUint32(buffer, 4);
            return clsobject;
        }
    }
}
