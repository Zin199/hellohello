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


        //Bot test
        // private static TelegramBotClient bot = new TelegramBotClient("1623962486:AAEnaEoZjtJ7cG8YSFTF4bHh6le2Oa4mXA4");

        //Bot Release
        private static TelegramBotClient bot = new TelegramBotClient("1623962486:AAEnaEoZjtJ7cG8YSFTF4bHh6le2Oa4mXA4");


        private static ChargingTransactionRepository _chargingTransactionRepository = new ChargingTransactionRepository();

        private static CheckCardTransactionRepository _checkCardTransactionRepository = new CheckCardTransactionRepository();

        private static TopupTransactionRepository _topupTransactionRepository = new TopupTransactionRepository();

        private static ChargingTopupTransactionRepository _chargingTopupTransactionRepository = new ChargingTopupTransactionRepository();
       

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
                    //Get partner by chatId
                    var partner = _partnerRepository.GetPartnerByTelegramGroupId(e.Message.Chat.Id.ToString());


                    if (partner != null && e.Message.Text.StartsWith("/sr "))
                    {
                        var serial = e.Message.Text.Replace("/sr", "").Trim();
                       
                        var chargingTran = _chargingTransactionRepository.GetTransactionBySerialPartnerCode(serial, partner.Code);
                       
                        if (chargingTran != null)
                        {
                            if(chargingTran.ProviderCode != "PAYEXPRESS" && chargingTran.ProviderCode != "THANHTOANVIP")
                            {
                                //if(chargingTran.ProviderCode == "CIUCIU")
                                {
                                    //bot.SendTextMessageAsync("-493773906", $"Kiểm tra giúp mình serial {chargingTran.CardSerial} Mệnh giá : {chargingTran.CardAmount} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} ");
                                }
                                if (chargingTran.ProviderCode == "XBOOM")
                                {
                                    bot.SendTextMessageAsync("-542279635", $"Kiểm tra giúp mình serial {chargingTran.CardSerial} Mệnh giá : {chargingTran.CardAmount} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} ");
                                }
                                
                            }    
                            else if (chargingTran.Status == Constant.CARD_STATUS_SUCCESS)
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial {chargingTran.CardSerial} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]} Mệnh giá : {chargingTran.CardAmount} đã thực hiện callback lại lần nữa, a/c kiểm tra lại giúp e");
                                // Thuc hien callback luon
                            }
                            else if (chargingTran.Status == 2 && chargingTran.InternalErrorCode == 6)
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial {chargingTran.CardSerial} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]} Mệnh giá : {chargingTran.CardAmount}.Gửi giúp em thông tin serial đúng theo cú pháp sai /sai serial sai /dung serial đúng. VD: /sai 1000123123 /dung 10001231234");
                                // gui cu phap kiem tra cho no
                            }
                            else if (chargingTran.Status == 100)
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã nhận được serial {chargingTran.CardSerial} Nhà mạng :{Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]} Mệnh giá : {chargingTran.CardAmount} đã thực hiện callback lại lần nữa, a/c kiểm tra lại giúp e");
                            }
                            else if(chargingTran.Status == 2 && chargingTran.InternalErrorMessage == "Wrong card amount")
                            {
                                // Thong bao sai menh gia, bao voi nhom van hanh noi bo de xem co ho tro hay khong
                                //bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"{chargingTran.CardSerial} Chọn nhầm mệnh giá, hỗ trợ callback lại nếu như kêu check.");
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
                                    {
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Bên em đã nhận được thẻ serial {chargingTran.CardSerial} Nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]} Mã thẻ sử dụng nhà mạng báo không hợp lệ, a/c kiểm tra lại giúp e . ");
                                    }
                                    else if (checkCard.Status == 1 && checkCard.UseTime < chargingTran.CreatedTime)
                                    {
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Bên em đã nhận được thẻ serial {chargingTran.CardSerial} Nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]}. Thẻ đã sử dụng trước khi gửi qua hệ thống bên em. Thông tin sử dụng SĐT {checkCard.Isdn} Thời gian : {checkCard.UseTime} Mệnh giá : {checkCard.CardAmount}");
                                    }
                                    else if (checkCard.Status == 1 && chargingTran.Status == 2 && checkCard.Isdn.Substring(0, 5) != checkMobile.Mobile.Substring(1, 5))
                                    {
                                        bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"  Bên em đã nhận được thẻ serial {chargingTran.CardSerial} Nhà mạng: {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Trạng thái : {Constant.CARDSTATUS[(byte)chargingTran.Status]}. Thuê bao sử dụng không phải hệ thống bên em a/c kiểm tra lại giúp e. Thông tin sử dụng SĐT {checkCard.Isdn} Thời gian : {checkCard.UseTime} Mệnh giá : {checkCard.CardAmount}");
                                    }

                                }
                                else
                                {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Bên em đã nhân được thẻ serial {chargingTran.CardSerial} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} a/c đợi bên em kiểm tra rồi phản hồi lại @mrtelesupport . ");
                                }

                            }
                        }
                        else
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $" Serial không tìm thấy trên hệ thống bên em. A/c kiểm tra lại giúp e. ");
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

                            var chargingTran = _chargingTransactionRepository.GetTransactionBySerialFalsePartnerCode(serialSai, partner.Code);

                            var addCard = _checkCardTransactionRepository.InsertCheckCard(serialDung, (byte)chargingTran.CardType);

                            var checkMobile = _chargingTopupTransactionRepository.GetChargingTopupTran(chargingTran);


                            /*
                              * 1. Khach gui serial sai.
                              * 2. Kiem tra serial sai và trả lời kèm cú pháp hướng dẫn sai đúng.
                              * 3. Lấy serial đúng check serial, đợi khoảng 1-30s, có kết quả thì sửa luôn trạng thái thẻ và bot trả lời luôn
                              Logic đợi.
                             1. Insert vào bảng checkcard.
                             2.Cho vào vòng lặp check xem cái serial vừa insert có kết quả chưa, nếu chưa thì sleep 5000.
                              */
                            var a = 0;
                            var checkCardTran = new CheckCardTransaction();
                            do
                            {

                                addCard = _checkCardTransactionRepository.GetCheckCard(serialDung);
                                if (addCard != null || a > 3)
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

                            if (addCard.Status == 1 && addCard.Isdn.Substring(0, 5) == checkMobile.Mobile.Substring(1, 5))
                            {
                                //var update = _chargingTransactionRepository.UpdateSerialFalse(chargingTran, 1, (int)addCard.CardAmount);
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã tiếp nhận được thẻ serial sai {serialSai} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Serial đúng {serialDung} Trạng thái : thành công Mệnh giá : {addCard.CardAmount}. Đã thực hiện callback lại lần nữa, a/c kiểm tra giúp e");
                            }
                            else if
                                (checkCardTran.Status == 13)
                            {
                                bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Bên em đã tiếp nhận được thẻ serial sai {serialSai} Nhà mạng : {Constant.CARDTYPESERIAL[(byte)chargingTran.CardType]} Serial đúng {serialDung}. A/c đợi bên em kiểm tra rồi phản hồi lại. ");
                            }

                        }
                        else
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id.ToString(), $"Cú pháp sai, a/c sử dụng đúng theo cú pháp theo cú pháp '/sai serialsai /dung serialdung' 'ví dụ : /sai 1000123123 /dung 10001231234'.");
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



