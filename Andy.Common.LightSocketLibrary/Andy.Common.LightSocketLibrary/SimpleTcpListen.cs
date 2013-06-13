using System;
using System.Net;
using System.Net.Sockets;

// SimpleTcpListen 对TcpListen的封装
namespace Andy.Common.LightSocketLibrary {
    public class SimpleTcpListen {
        private IPEndPoint obj_listenIpEndPoint;
        public IPEndPoint ListenIpEndPoint {
            get {
                return obj_listenIpEndPoint;
            }
        }

        public event Action<byte[]> DataReceived;

        public SimpleTcpListen(string address, int port) {
            IPEndPoint temp_ipendpoint = new IPEndPoint(IPAddress.Parse(address), port);
            this.obj_listenIpEndPoint = temp_ipendpoint;

            obj_listen = new TcpListener(this.obj_listenIpEndPoint);
        }

        private TcpListener obj_listen = null;
        private TcpClient obj_tcpClient = null;
        private NetworkStream obj_network_stream = null;
        private const int DEFUATL_BUFFER_SIZE = 1024;
        private byte[] byte_buffer = new byte[DEFUATL_BUFFER_SIZE];

        public void StartListen() { 
            this.obj_listen.Start();
            this.obj_listen.BeginAcceptTcpClient(ConnectAccept, null);
        }
        public void StopListen() { this.obj_listen.Stop(); }

        private void ConnectAccept(IAsyncResult ar) {
            obj_tcpClient = this.obj_listen.EndAcceptTcpClient(ar);
            obj_network_stream = this.obj_tcpClient.GetStream();
            obj_network_stream.BeginRead(byte_buffer, 0, byte_buffer.Length, DataArriveOnConnection, null);
        }

        private void DataArriveOnConnection(IAsyncResult ar) {
            int bytesRead = obj_network_stream.EndRead(ar);
            if (bytesRead > 0) {
                //对获取数据的处理
                System.Diagnostics.Debug.WriteLine("Message:" + System.Text.Encoding.ASCII.GetString(byte_buffer));
                if (DataReceived != null) {
                    DataReceived(byte_buffer);
                }
                obj_network_stream.BeginRead(byte_buffer, 0, byte_buffer.Length, DataArriveOnConnection, null);
            }
        }

        public void Send(byte[] buffer) {
            if (obj_network_stream != null && obj_network_stream.CanWrite) {
                obj_network_stream.Write(buffer,0,buffer.Length);
                obj_network_stream.Flush();
            }
        }
    }
}
