using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram
{
    public class Constant
    {
        public static byte CARD_STATUS_SUCCESS = 1;
        public static byte CARD_STATUS_FAILED = 2;
        public static byte CARD_STATUS_NOT_USE = 100;
        public static byte CARD_STATUS_PROCESSING = 11;
        public static byte CARD_STATUS_WRONG_SERIAL = 6;
        //public static string VIDU = """
        public static string[] CARDTYPESERIAL = new string[]
    {
        "",
        "VIETTEL",
        "MOBIFONE",
        "VINAPHONE",
        
    };
        public static string[] CARDSTATUS = new string[]
    {
        "",
        "THÀNH CÔNG",
        "THẤT BẠI",
    };
        public static string[] PINCODESTATUS = new string[]
    {
        "NHẬP KHO",
        "ĐÃ BÁN",
        "THẺ LỖI",
    };
    }
}
