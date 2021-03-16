using BotTelegram.Repository;
using System;
using System.Linq;
using System.Threading;
using Telegram.Bot.Args;
using Telegram.Bot;
using System.Collections.Generic;
using BotTelegram.Model;

namespace BotTelegram
{
    class Program
    {


        private static TelegramBotClient bot = new TelegramBotClient("1636960131:AAHlvUDkoJ0XtPd2jmd1dQQcYxGhn4tgKMA");

        private static ChargingTransactionRepository _chargingTransactionRepository = new ChargingTransactionRepository();

        private static CheckCardTransactionRepository _checkCardTransactionRepository = new CheckCardTransactionRepository();

        private static TopupTransactionRepository _topupTransactionRepository = new TopupTransactionRepository();

        private static ChargingTopupTransactionRepository _chargingTopupTransactionRepository = new ChargingTopupTransactionRepository();

        private static PartnerRepository _partnerRepository = new PartnerRepository();

        


        //private static System.Timers.Timer _checkCardTimer = new System.Timers.Timer();


        private static void Main(string[] args)
        {


            bot.OnMessage += ChargingTrans;
            //bot.OnMessage += CheckCardTrans;
            //TopupTrans();
            bot.StartReceiving();

            Console.ReadLine();

        }

        //private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        //{
        //    var partnerCode = "ZOTA";
        //    int Status = 13;


        //    var topupTran = _topupTransactionRepository.GetTopupTransactinon(partnerCode, Status);
        //    if (topupTran.Status == 13)
        //    {
        //        bot.SendTextMessageAsync("-1001180320054", $" Mã GD {topupTran.TransCode} Thành Công Sai Serial SĐT {topupTran.Mobile}. Yêu cầu cập nhật lại.");

        //    }

        //}

        private static void ChargingTrans(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                //Get partner by chatId
                var partner = _partnerRepository.GetPartnerByTelegramGroupId(e.Message.Chat.Id.ToString());


                if (partner != null && e.Message.Text.StartsWith("/check "))
                {
                    var serial = e.Message.Text.Replace("/check", "").Trim();

                    var chargingTran = _chargingTransactionRepository.GetTransactionBySerialPartnerCode(serial, partner.Code);

                    if (chargingTran != null)
                    {
                        if (chargingTran.Status == 1)
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"{chargingTran.CardSerial} Thành công mệnh giá {chargingTran.CardAmount} đã callback lại");
                        }
                        else if (chargingTran.Status == 2 && chargingTran.InternalErrorCode == 6)
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"{chargingTran.CardSerial} Serial thẻ bị sai, cho e xin serial đúng để bên em check");
                        }
                        else if (chargingTran.Status == 100)
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"{chargingTran.CardSerial} Không sử dụng bên mình.");
                        }
                        else
                        {
                            var addCard = _checkCardTransactionRepository.InsertCheckCard(serial, (byte)chargingTran.CardType);
                            var checkCard = new CheckCardTransaction();
                            var checkMobile = _chargingTopupTransactionRepository.GetChargingTopupTran(chargingTran);
                            var a = 0;
                            do
                            {
                                checkCard = _checkCardTransactionRepository.GetCheckCardSuccess((int)addCard.Id);
                                if (checkCard != null || a > 3)
                                {
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(5000);
                                }
                                a++;
                            }
                            while (a <= 3);

                            if (checkCard != null)
                            {
                                if (checkCard.Status == 1 && checkCard.CardStatus == 0)
                                {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Serial {checkCard.CardSerial} chưa sử dụng, mã thẻ sai. ");
                                }
                                if (checkCard.Status == 1 && checkCard.UseTime < chargingTran.CreatedTime)
                                {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Serial {checkCard.CardSerial} mệnh giá {checkCard.CardAmount} SĐT {checkCard.Isdn}\nthời gian sử dụng {checkCard.UseTime}| thẻ đã sử dụng trước.  ");
                                }
                                else if (checkCard.Status == 1 && chargingTran.Status == 2 && checkCard.Isdn.Substring(0, 5) != checkMobile.Mobile.Substring(1, 5))
                                {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Serial {checkCard.CardSerial} mệnh giá {checkCard.CardAmount}, sđt {checkCard.Isdn}\nthời gian sử dụng {checkCard.UseTime}| sđt nạp k phải bên mình.");
                                }

                            }
                            else
                            {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Bên em đã tiếp nhận thông tin thẻ cào, a/c đợi chút để bên em kiểm tra ạ @mrtelesupport ");   
                            }                           
                        }
                    }
                    else
                    {
                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Serial Không tìm thấy");
                    }

                }
                //if (e.Message.Text.StartsWith("/serialsai"))
                //{
                //    List<string> listSeri = e.Message.Text.Split(' ').ToList();
                //    foreach (var a in listSeri)
                //    {
                //        Console.WriteLine(a);
                //    }
                //    Console.WriteLine("/serialsai: " + listSeri[0]);
                //    Console.WriteLine("seri1: " + listSeri[1]);
                //    Console.WriteLine("/serialdung: " + listSeri[2]);
                //    Console.WriteLine("seri2: " + listSeri[3]);

                //    var serialsai = "";
                //    var serialdung = "";

                //    // /serialsai 4390328402 /serialdung alsdkjfsld

                //    if (listSeri[0] == "/serialsai" && listSeri[2] == "/serialdung")
                //    {

                //        var chargingTran = _chargingTransactionRepository.GetTransactionBySerialFalsePartnerCode(serialsai, partner.Code);

                //        var insertedCheckSerial = _checkCardTransactionRepository.InsertCheckCard(serialdung, (byte)chargingTran.CardType);

                //        var checkMobile = _chargingTopupTransactionRepository.GetChargingTopupTran(chargingTran);
                //        /*
                //          * 1. Khach gui serial sai.
                //          * 2. Kiem tra serial sai và trả lời kèm cú pháp hướng dẫn sai đúng.
                //          * 3. Lấy serial đúng check serial, đợi khoảng 1-30s, có kết quả thì sửa luôn trạng thái thẻ và bot trả lời luôn
                //          Logic đợi.
                //         1. Insert vào bảng checkcard.
                //         2.Cho vào vòng lặp check xem cái serial vừa insert có kết quả chưa, nếu chưa thì sleep 5000.
                //          */
                //        var dem = 0;
                //        var checkCardTran = new CheckCardTransaction();
                //        do
                //        {
                //            checkCardTran = _checkCardTransactionRepository.GetCheckCard(serialdung);
                //            Thread.Sleep(5000);
                //            dem++;
                //            if (dem < 5)
                //            {
                //                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã tiếp nhận thông tin serial sai và serial đúng. A/c chờ chút bên em kiểm tra ạ @mrtelesupport ");
                //                break;
                //            }
                //        } while (checkCardTran.Status == 1);

                //        if (checkCardTran.Status == 1 && checkCardTran.Isdn.Substring(0, 5) == checkMobile.Mobile.Substring(1, 5))
                //        {
                //            //_checkCardTransactionRepository.UpdateSerialSai(serialsai, checkCardTran.CardAmount);
                //            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" serialsai {listSeri[1]} serialdung {listSeri[2]} thành công {checkCardTran.CardAmount} đã callback lại.");
                //        }
                //        else if
                //            (checkCardTran.Status == 13)
                //        {
                //            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã tiếp nhận thông tin serial sai và serial đúng. A/c chờ chút bên em kiểm tra ạ @mrtelesupport");
                //        }

                //    }
                //    else
                //    {
                //        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Sai cú pháp kiểm tra.");
                //    }
                //}
            }



        }
    private static void TopupTrans()
    {
        var partnerCode = "ZOTA";
        int Status = 13;
        do
        {
            var topupTran = _topupTransactionRepository.GetTopupTransactinon(partnerCode, Status);
            if (topupTran.Status == 13)
            {
                bot.SendTextMessageAsync("-1001180320054", $" Mã GD: {topupTran.TransCode} Thành Công Sai Serial SĐT: {topupTran.Mobile} Yêu cầu cập nhật lại.");
                Thread.Sleep(5000);
            }

        } while (true);
    }

}

}



