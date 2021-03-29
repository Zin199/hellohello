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
        public ChargingTransaction GetTransactionBySerialPartnerCode(string cardSerial, List<string> listPartnerCode)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    //Lay du lieu
                    var chargingTran = db.ChargingTransactions.Where(c => c.CardSerial == cardSerial && listPartnerCode.Contains(c.PartnerCode)).FirstOrDefault();
                    chargingTran.IsCallbackPartner = false;
              
                    db.SaveChanges();
                    return chargingTran;
                }                   
            }
            catch(Exception ex)
            {

            }
            return null;
        }
        public ChargingTransaction GetTransactionBySerialFalsePartnerCode(string cardSerial, List<string> listPartnerCode)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {                 
                    var chargingTran = db.ChargingTransactions.Where(c =>  c.CardSerial == cardSerial
                                                                        && listPartnerCode.Contains(c.PartnerCode)
                                                                        ).FirstOrDefault();                   
                    

                    return chargingTran;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public ChargingTransaction UpdateSerialFalse(ChargingTransaction chargingTran, int status, int cardamount)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    var item = db.ChargingTransactions.FirstOrDefault(c => c.Id ==  chargingTran.Id);
                    if (item != null)
                    {
                        
                        item.Status = (short)status;
                        item.CardAmount = cardamount;
                        item.IsCallbackPartner = false;

                        db.SaveChanges();
                        return item;
                    }
                }
            }

            catch (Exception ex)
            {

            }
            return null;
        }
        public ChargingTransaction UpdateSerialWrong( ChargingTransaction chargingTran, int status, int cardamount)
        {
            try
            {
                using (var db = new DevPayExpressEntities())
                {
                    var item = db.ChargingTransactions.FirstOrDefault(c => c.Id == chargingTran.Id);

                    if (item != null)
                    {
                        item.Status = (short)status;
                        item.CardAmount = cardamount;
                        item.IsCallbackPartner = false;

                        db.SaveChanges();
                        return item;
                    }
                }
            }
            catch (Exception)
            {

            }
            return null;
        }


    }
}
