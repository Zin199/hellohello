using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Model;
namespace BotTelegram.Repository
{
    class ChargingTopupTransactionRepository
    {
       public ChargingTopupTransaction GetChargingTopupTran(ChargingTransaction chargingTransaction)
        {
            try
            {
                using (var db = new DevPayExpressEntities()) 
                {
                    var chargingTopup = db.ChargingTopupTransactions.Where(c => c.PartnerChargingTransactionId == chargingTransaction.Id).FirstOrDefault();


                    return chargingTopup;
                }


            }
            catch (Exception ex)
            {

            }
            return null;
        }
        

    }
}
