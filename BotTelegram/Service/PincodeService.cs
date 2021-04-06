using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTelegram.Repository;
using BotTelegram.Model;
using Telegram.Bot;

namespace BotTelegram.Service
{
    public class PincodeService
    {
        private static PinCodeStockRepository _pinCodeStockRepository = new PinCodeStockRepository();

        public void GetPinCodeStock(object sender, Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot)
        {

            
            try
            {

                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                {
                    if (e.Message.Text.StartsWith("/pc"))
                    {
                        var serial = e.Message.Text.Replace("/pc","").Trim();
                        var pinCode = _pinCodeStockRepository.GetPincode(serial);
                        if(pinCode != null) 
                        {                          
                                bot.SendTextMessageAsync("-501613913", $"Serial: {pinCode.CardSerial}\nMệnh giá: {pinCode.CardAmount}\nNhà mạng: {Constant.CARDTYPESERIAL[(byte)pinCode.CardType]}\nThời gian tạo: {pinCode.CreatedTime}\nThời gian xuất kho: {pinCode.ExportedTime}\nTrạng thái: {Constant.PINCODESTATUS[(byte)pinCode.Status]} \nNhà cung cấp: {pinCode.ImportProvider}  ");
       
                        }
                        else
                        {
                            bot.SendTextMessageAsync("-501613913", $"Serial không tìm thấy trong kho");
                        }

                    }
                    
                }
            }
            catch (Exception ex)
            {

            }
            
        }


    }
}
