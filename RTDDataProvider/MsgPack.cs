using RTDDataProvider.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MsgPack;
using MsgPack.Serialization;

namespace RTDDataProvider
{
    public static class MsgBytes
    {
        public static string ToJson(Stream MsgPackStream)
        {
            var msg = MessagePackSerializer.Create<MessagePackObject>();
            return msg.Unpack(MsgPackStream).ToString();
        }
    }
}
