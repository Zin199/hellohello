using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Model;
namespace BotTelegram.Repository
{
    
    public class TopupTransactionRepository
    {
        public TopupTransaction GetTopupTransactinon(string partnerCode, int status)
        {

            try
            {
                using (var db = new DevPayExpressEntities()) 
                {
                    var topupTran = db.TopupTransactions.Where(s => s.PartnerCode == partnerCode && s.Status == status).FirstOrDefault();

                    return topupTran;
                }
            }
            catch(Exception ex)
            {

            }
            return null;
        }

    }
}
