using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Model;
namespace BotTelegram.Repository
{
    
    public class PartnerRepository
    {
        public Partner GetPartnerByTelegramGroupId(string telgramGroupId)
        {

            try
            {
                using (var db = new DevPayExpressEntities()) 
                {
                    var parnter = db.Partners.Where(s => s.TelegramSuportGroupId == telgramGroupId).FirstOrDefault();

                    return parnter;
                }
            }
            catch(Exception ex)
            {

            }
            return null;
        }

    }
}
