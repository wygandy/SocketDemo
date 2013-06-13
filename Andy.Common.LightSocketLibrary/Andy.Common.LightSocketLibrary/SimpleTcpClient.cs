using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
namespace Andy.Common.LightSocketLibrary {
    /// <summary>
    /// 简单的TCP客户端
    /// </summary>
    public class SimpleTcpClient  {
        private TcpClient _client;
        private NetworkStream _networkstream = null;
        private int readbytes;

        public event Action<string /*exception message*/> SocketError;
        public event Action<byte[]> DataReceived;

        private EventWaitHandle allDone = new EventWaitHandle(false, EventResetMode.ManualReset);
        private int _port;
        private string _ipaddress;
        public string IpAddress {
            get { return _ipaddress; }
            set {
                _ipaddress = value;
                
            }
        }

        public int Port {
            get { return _port; }
            set {
                _port = value;
            }
        }

        private bool _isAutoResetConnect= false;
        /// <summary>
        /// 标识是否自动重置连接
        /// </summary>
        public bool IsAutoResetConnect {
            get { return this._isAutoResetConnect; }
            set {
                this._isAutoResetConnect = value;
            }
        }

        public SimpleTcpClient(string address, int port) {
            this._ipaddress = address;
            this._port = port;
        }

        public void Connect() {
            if (_client == null) {
                _client = new TcpClient();
            }

            try {
                if (_client.Connected == false) {
                    AsyncCallback callback = new AsyncCallback(RequestCallback);
                    allDone.Reset();

                    _client.BeginConnect(IPAddress.Parse(_ipaddress), _port, callback, _client);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex) {
                if (SocketError != null) {
                    SocketError(ex.Message);
                }
            }

        }

        private void RequestCallback(IAsyncResult ar) {
            allDone.Set();
            try {
                TcpClient tcp = (TcpClient)ar.AsyncState;
                tcp.EndConnect(ar);
                _networkstream = tcp.GetStream();
            }
            catch (Exception ex) {
                if (SocketError != null) {
                    SocketError(ex.Message);
                }
            }
        }

        public void Send(byte[] buffer) {
            try {
                if (_networkstream.CanWrite) {
                    lock (_networkstream) {
                        _networkstream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(sendCallBack), _networkstream);
                        _networkstream.Flush();
                    }
                }
            }
            catch (Exception ex) {
                _networkstream.Close();
                if (SocketError != null) {
                    SocketError(ex.Message);
                }
            }
        }

        private void sendCallBack(IAsyncResult ar) {
            try {
                _networkstream.EndWrite(ar);
            }
            catch (Exception ex) {
                if (SocketError != null) {
                    this.SocketError(ex.Message);
                }
            }
        }

        private void RaiseDataReceive(byte[] buffer) {
            if (DataReceived != null) {
                DataReceived(buffer);
            }
        }


        private const int DEFAULT_BUFFER_SIZE = 1024;
        private byte[] _buffer = new byte[DEFAULT_BUFFER_SIZE];
        public void StartReceiveData() {
            try {
                if (_networkstream != null) {
                    if (_networkstream.CanRead) {
                        lock (_networkstream) {
                            _networkstream.BeginRead(_buffer, 0, _buffer.Length, OnReceived, _networkstream);
                        }
                    }
                }
            }
            catch (Exception ex) {
                if (SocketError != null) {
                    SocketError(ex.Message);
                }
            }
        }

        private void OnReceived(IAsyncResult ar) {
            NetworkStream temp_stream = (NetworkStream)ar.AsyncState;
            try {
                int bytereads = temp_stream.EndRead(ar);
                if (bytereads > 0) {
                    RaiseDataReceive(_buffer);
                }

                temp_stream.BeginRead(_buffer, 0, _buffer.Length, OnReceived, _networkstream);
            }
            catch (Exception ex) {
                if (SocketError != null) {
                    SocketError(ex.Message);
                }
            }
        }
    }
}
