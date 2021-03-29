using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Model;
namespace BotTelegram.Repository
{
    class PinCodeStockRepository
    {      
      public PinCodeStock GetPincode(string cardserial)
        {
            try
            {
                using (var db = new DevPayExpressEntities()) 
                {

                    var pincode = db.PinCodeStocks.Where(c => c.CardSerial == cardserial).FirstOrDefault();


                    return pincode;
                }              
            }
            catch(Exception ex)
            {

            }
            return null;
        }
      
        
        
    }
}
