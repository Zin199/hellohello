using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Model;

namespace BotTelegram.Repository
{
    class ServiceProviderRepository
    {
       public ServiceProvider GetChargingTelegramSupportID(string ChargingTelegroupID)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    var provider = db.ServiceProviders.Where(c => c.ChargingTelegramSupportGroupId == ChargingTelegroupID).FirstOrDefault();

                    return provider;
                }
                
            }
            catch(Exception ex)
            {

            }
            return null;
        }
    }
}
