using BotTelegram.Repository;
using System;
using System.Linq;
using System.Threading;
using Telegram.Bot.Args;
using Telegram.Bot;
using System.Collections.Generic;
using BotTelegram.Model;
using NLog;
using ServiceStack;

namespace BotTelegram
{
    class Program
    {

        public static Logger _LOG = LogManager.GetCurrentClassLogger();

        //Bot test
        //private static TelegramBotClient bot = new TelegramBotClient("1636960131:AAHlvUDkoJ0XtPd2jmd1dQQcYxGhn4tgKMA");

        //Bot Release
        private static TelegramBotClient bot = new TelegramBotClient("1015933066:AAGT6BBzsN5a-TfV606Z7X0yAYlbWTdhJJg");


        private static ChargingTransactionRepository _chargingTransactionRepository = new ChargingTransactionRepository();

        private static CheckCardTransactionRepository _checkCardTransactionRepository = new CheckCardTransactionRepository();

        private static TopupTransactionRepository _topupTransactionRepository = new TopupTransactionRepository();

        private static ChargingTopupTransactionRepository _chargingTopupTransactionRepository = new ChargingTopupTransactionRepository();

        private static ServiceProviderRepository serviceProviderRepository = new ServiceProviderRepository();

        private static PartnerRepository _partnerRepository = new PartnerRepository();



        //public static string INTERNAL_TELEGRAM_GROUPID = "-1";


        //private static System.Timers.Timer _checkCardTimer = new System.Timers.Timer();


        private static void Main(string[] args)
        {


           

            bot.OnMessage += ChargingTrans;
            //bot.OnMessage += CheckCardTrans;
            //TopupTrans();
            bot.StartReceiving();

            Console.ReadLine();

        }
        private static void ChargingTrans(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                {

                    _LOG.Info(e.Message.ToJson());
                    //Get partner by chatId
                    var listPartnerCode = _partnerRepository.GetListPartnerByTelegramGroupId(e.Message.Chat.Id.ToString());
                    


                    if (listPartnerCode.Count > 0 && e.Message.Text.StartsWith("/sr "))

                    {
                        var serial = e.Message.Text.Replace("/sr", "").Trim();

                        var chargingTran = _chargingTransactionRepository.GetTransactionBySerialPartnerCode(serial, listPartnerCode);

                     

                        if (chargingTran != null)
                        {
                            if (chargingTran.ProviderCode != "PAYEXPRESS" && chargingTran.ProviderCode != "THANHTOANVIP")
                            {

                               
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}. A/c đợi chút bên em kiểm tra rồi trả kết quả ạ. ");

                                if (chargingTran.ProviderCode == "CIUCIU")
                                {
                                    //ciuciu -493773906
                                    bot.SendTextMessageAsync("-493773906", $"Kiểm tra giúp mình serial {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} ");
                                }
                                else if
                                (chargingTran.ProviderCode == "XBOOM")
                                {
                                    //xboom -351143187
                                    bot.SendTextMessageAsync("-212144332", $"Kiểm tra giúp mình serial {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} ");
                                }
                            }
                            else if (chargingTran.Status == Constant.CARD_STATUS_SUCCESS)
                            {
                                //TH tc chua nhan dc callback

                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được thẻ serial: {serial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]}, mệnh giá gửi: {chargingTran.RequestAmount}, đã thực hiện callback lại lần nữa. A/c kiểm tra lại giúp.");
                                
                            }
                            else if (chargingTran.Status == 2 && chargingTran.InternalErrorCode == 6)
                            {   //TH sai serial                           
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]} , mệnh giá gửi: {chargingTran.RequestAmount}. Gửi giúp em thông tin serial đúng theo cú pháp \"/sai serialsai /dung: serialdung\", ví dụ: \"/sai 1000123123 /dung 10001231234\"");
                                
                            }
                            else if (chargingTran.Status == 100)
                            {
                                //TH the k su dung
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]} , mệnh giá gửi: {chargingTran.RequestAmount}. Đã thực hiện callback lại lần nữa, a/c kiểm tra lại giúp e");

                            }
                            else if (chargingTran.Status == 2 && chargingTran.InternalErrorMessage == "Wrong card amount")
                            {
                                // Thong bao sai menh gia, bao voi nhom van hanh noi bo de xem co ho tro hay khong
                                //tpl-van hanh : -572467985
                                //vpsgw : -487546667
                                bot.SendTextMessageAsync("-487546667", $" Serial {chargingTran.CardSerial} chọn nhầm mệnh giá, đối tác: {chargingTran.PartnerCode}, kiểm tra có hỗ trợ hay không.");
                            }
                            else
                            {



                                var checkCard = new CheckCardTransaction();
                                var checkMobile = _chargingTopupTransactionRepository.GetChargingTopupTran(chargingTran);
                                var a = 0;

                                var addCard = _checkCardTransactionRepository.InsertCheckCard(serial, (byte)chargingTran.CardType);
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
                                    {   //TH sai mã
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được thẻ serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]}. Mã thẻ sử dụng nhà mạng báo không hợp lệ, a/c kiểm tra lại giúp e .");
                                    }
                                    else if (checkCard.Status == 1 && checkCard.UseTime < chargingTran.CreatedTime)
                                    {   //TH the sd truoc
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được thẻ serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]}. Thẻ đã sử dụng trước khi gửi qua hệ thống bên em. Thông tin sử dụng: thuê bao: {checkCard.Isdn}, thời gian: {checkCard.UseTime}, mệnh giá: {checkCard.CardAmount}");
                                    }
                                    else if (checkCard.Status == 1 && chargingTran.Status == 2 && checkCard.Isdn.Substring(0, 5) != checkMobile.Mobile.Substring(1, 5))
                                    {   //TH tb nap k phai ben minh
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được thẻ serial: {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}, trạng thái: {Constant.CARDSTATUS[(byte)chargingTran.Status]}. Thuê bao sử dụng không phải hệ thống bên em a/c kiểm tra lại giúp e. Thông tin sử dụng: thuê bao: {checkCard.Isdn}, thời gian: {checkCard.UseTime}, mệnh giá: {checkCard.CardAmount}");
                                    }

                                }
                                else
                                {   //TH qua so lan tra cuu
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhân được thẻ serial {chargingTran.CardSerial}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}. A/c đợi bên em kiểm tra rồi phản hồi lại @mrtelesupport. ");
                                }

                            }
                        }
                        else
                        {   //the k tim thay
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Thẻ serial: {chargingTran.CardSerial} không có thông tin trên hệ thống bên em. A/c kiểm tra lại giúp. ");
                        }

                    }
                    if (e.Message.Text.StartsWith("/sai"))
                    {

                        var userInput = e.Message.Text;

                        userInput = userInput.Replace("/sai", " ").Replace("/dung", " ").Replace("dung", " ");
                        List<string> list = userInput.Split(' ').ToList();

                        list = list.Where(a => a.Length > 0).ToList();
                        //Console.WriteLine(a);
                        var serialSai = list[0];
                        var serialDung = list[1];


                        if (list.Count == 2) // co the check them dinh dang serial dung
                        {

                            var chargingTran = _chargingTransactionRepository.GetTransactionBySerialFalsePartnerCode(serialSai, listPartnerCode);
                            if (chargingTran == null)
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Serial sai không tồn tại. A/c kiểm tra lại giúp em");
                            }
                           
                            var addCard = _checkCardTransactionRepository.InsertCheckCard(serialDung, (byte)chargingTran.CardType);
                            var checkMobile = _chargingTopupTransactionRepository.GetChargingTopupTran(chargingTran);
                            var a = 0;
                            var checkCardTran = new CheckCardTransaction();
                           
                            do
                            {

                                addCard = _checkCardTransactionRepository.GetCheckCard(serialDung);

                                Console.WriteLine("add");
                                if (addCard.Status == 1 || a > 3)
                                {
                                    Console.WriteLine("add tc");
                                    
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(3000);
                                }
                                a++;
                            }
                            while (a <= 3);
                           
                            if (addCard != null)
                            {

                                Console.WriteLine("Check");
                                if (addCard.Status == 1 && addCard.CardStatus == 1 && addCard.Isdn.Substring(0, 5) == checkMobile.Mobile.Substring(1, 5))
                                {
                                    //TH dung so nap
                                    Console.WriteLine("zzz");
                                    var update = _chargingTransactionRepository.UpdateSerialFalse(chargingTran, 1, (int)addCard.CardAmount);
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em nhận được thẻ serial sai: {serialSai}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, serial đúng: {serialDung}, trạng thái: thành công, mệnh giá nạp: {addCard.CardAmount}, đã thực hiện lại callback lần nữa. A/c kiểm tra lại giúp. ");
                                }
                                if (addCard.Status == 1 && addCard.CardStatus == 1 && addCard.Isdn.Substring(0, 5) != checkMobile.Mobile.Substring(1, 5))
                                {   //TH khac so nap
                                    Console.WriteLine("xxx");
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em nhận được thẻ sai serial: {serialSai}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, serial đúng: {serialDung} bên mình cung cấp không nạp vào thuê bao bên em. A/c kiểm tra lại giúp em.");
                                }
                                if (addCard.Status == 13 && addCard.CardStatus == 13)
                                {   //TH seri đúng check quá số lần
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhân được thẻ serial: {serialSai}, serial đúng: {serialDung}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}. A/c đợi bên em kiểm tra rồi phản hồi lại @mrtelesupport . ");
                                }
                            }
                            else
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhân được thẻ serial sai: {serialSai}, serial đúng: {serialDung}, nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]}, mệnh giá gửi: {chargingTran.RequestAmount}. A/c đợi bên em kiểm tra rồi phản hồi lại @mrtelesupport . ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

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



