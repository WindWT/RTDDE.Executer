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
        /// <summary>
        /// Direct Convert MsgPack to DataTable
        /// </summary>
        /// <param name="MsgPackStream"></param>
        /// <param name="MDBType"></param>
        /// <returns></returns>
        public static DataTable ParseMDB(Stream MsgPackStream, MASTERDB MDBType)
        {
            DataTable dt = new DataTable();
            var msg = MessagePackSerializer.Create<MessagePackObject>();
            string json = msg.Unpack(MsgPackStream).ToString();
            return JSON.ParseJSONMDB(json, MDBType);
        }
    }
}
