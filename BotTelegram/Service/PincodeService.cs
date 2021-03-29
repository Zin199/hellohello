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

        public void GetCodeStock(object sender, Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot)
        {

            var getSerial = new PinCodeStock();
            try
            {
                getSerial = _pinCodeStockRepository.GetPincode("675657");

                if (getSerial != null)
                {

                   
                }
                else
                {

                    Console.WriteLine("ko lay dc");
                }


            }
            catch (Exception ex)
            {

            }
            
        }


    }
}
