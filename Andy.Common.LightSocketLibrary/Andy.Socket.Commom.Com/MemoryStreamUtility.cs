using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Andy.Socket.Common.Com {
    /// <summary>
    /// MemoryStream辅助类
    /// </summary>
    public class MemoryStreamUtility {
        private MemoryStreamUtility() { }

       
        private static MemoryStreamUtility _instance = null;
        public static MemoryStreamUtility Instance {
            get {
                if (_instance == null) {
                    _instance = new MemoryStreamUtility();
                }

                return _instance;
            }
        }

        private void WriteBytes(byte[] buf, MemoryStream ms) {
            foreach (byte b in buf) {
                ms.WriteByte(b);
            }
        }

        public void WriteUint32(uint data, MemoryStream ms) {
            WriteBytes(BitConverter.GetBytes(data), ms);
        }

        public void WirteUtfString(string uftString , MemoryStream ms ) {
            WriteBytes(System.Text.Encoding.UTF8.GetBytes(uftString), ms);
        }

        public UInt32 ReadUint32(byte[] buffer, int offset) {
            uint temp_uint32_value = BitConverter.ToUInt32(buffer, offset);
            return temp_uint32_value;
        }


        public string ReadUtf8String(byte[] buffer, int offset, int readLength) {
            string temp_utf8_string_value = System.Text.Encoding.UTF8.GetString(buffer, offset, readLength);
            return temp_utf8_string_value;
        }
    }
}
