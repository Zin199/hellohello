using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.Service
{
    public class CheckCardService
    {
        public void CheckSerial(string serial)
        {
            
            //B1: Goi repository de get partner.

            //B2: Lay the trong charging transaction voi partner code.

            //B3: Gui tin nhan thong bao lai cho doi tac.
        }

        public void XuLySerialSaiDung(string serialsai, string serialdung, string partnerTelegramGroupId)
        {

            //B1: Goi repository de get partner.
            // GetPartnerCodeByTelgramGroupId


            //B2: Lay the trong charging transaction voi partner code va serial sai.
            // GetChargingTransactionByPartnerCodeAndSerial

            //B3: Goi repository de insert serial dung vao check va doi ket qua sau 30s.

            //B4: Goi repository Cap nhat chargingtransaction.

            //B5: Thong bao lai cho doi tac

        }

    }
}
