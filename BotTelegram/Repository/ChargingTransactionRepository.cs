using BotTelegram.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.Repository
{
    public class ChargingTransactionRepository
    {
        public ChargingTransaction GetTransactionBySerialPartnerCode( string cardSerial, string partnerCode)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    //Lay du lieu
                    var chargingTran = db.ChargingTransactions.Where(c => c.CardSerial == cardSerial && c.PartnerCode == partnerCode).FirstOrDefault();
                    //chargingTran.IsCallbackPartner = false;
                    db.SaveChanges();

                    return chargingTran;
                }                   
            }
            catch(Exception ex)
            {

            }
            return null;
        }
        public ChargingTransaction GetTransactionBySerialFalsePartnerCode(string cardSerial, string partnerCode)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    //Lay du lieu
                    var chargingTran = db.ChargingTransactions.Where(c => c.CardSerial == cardSerial
                                                                   && c.PartnerCode == partnerCode
                                                                   && c.InternalErrorCode == 6).FirstOrDefault();
                    
                    db.SaveChanges();

                    return chargingTran;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ChargingTransaction UpdateSerialFalse(string serial, byte cardType)
        {
            using (var db = new DevPayExpressEntities())
            {
                var item = db.ChargingTransactions.FirstOrDefault(c => c.CardSerial == serial && c.CardType == cardType);
                if (item != null)
                {
                    item.Status = 1;
                    item.CardType = cardType;
                    item.IsCallbackPartner = false;
                    db.SaveChanges();
                    return item;
                }
            }
            return null;
        }

    }
}
