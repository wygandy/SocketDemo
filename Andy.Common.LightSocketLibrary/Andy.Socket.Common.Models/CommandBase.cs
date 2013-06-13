using System;
//基础命令类
namespace Andy.Socket.Common.Models {
    /// <summary>
    /// 所有扩展数据包的基类
    /// </summary>
    public class CommandBase {
        /// <summary>
        /// 命令ID，CS通信数据单元唯一标识
        /// 用于描述当前命令的具体功能
        /// 不同的命令ID对应的数据单元结构
        /// 组织不同.
        /// </summary>
        public uint CommandID { get; set; }

        /// <summary>
        /// 命令的长度，整个通信数据单元的数据长度
        /// </summary>
        public uint CommandLength { get; set; }
    }
}
