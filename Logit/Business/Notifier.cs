//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Diagnostics;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Text;
//using System.Web;
//using Logit.Models;

//namespace Logit.Business
//{
//    public class Notifier
//    {
//        public const string API_ID = "3384186";
//        public const string PASSWORD = "Bdes4JG6";
//        public const string USER = "lars.erik.finholt";

//        public string Notify(string email, string msg, string title, string phone)
//        {
//            Debug.WriteLine("    ---- Sending mail to " + email);

//            MailAddress from = new MailAddress("reminder.goaly@gmail.com", "Logit!");
//            MailAddress to = new MailAddress(email);

//            MailMessage message = new MailMessage(from, to);

//            message.Subject = "Reminder from Goaly - " + title;
//            message.Body = string.Format("<h1>Maybe you forgot something...?</h1><p>{0}</p><br/><a href='http://goaly.no/'>Take me to Goaly and let me register a new 'DoneIt' now!</a>", msg);
//            message.IsBodyHtml = true;

//            SmtpClient client = new SmtpClient();
//            //client.EnableSsl = true;
//            //client.Send(message);
//            return email + " - " + msg;
//        }

//        public string SendSMS(string phoneNumber, string msg)
//        {
//            var retval = "";

//            try
//            {
//                msg = msg.Replace("å", "\u00e5");

//                //Encoding ascii = Encoding.UTF8;
//                //Encoding unicode = Encoding.Unicode;

//                // //Convert the string into a byte array.
//                //byte[] asciiBytes = unicode.GetBytes(msg);

//                //// Perform the conversion from one encoding to the other.
//                //byte[] unicodeBytes = Encoding.Convert(ascii, unicode, asciiBytes);

//                //// Convert the new byte[] into a char[] and then into a string.
//                //char[] unicodeChars = new char[unicode.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];
//                //unicode.GetChars(unicodeBytes, 0, unicodeBytes.Length, unicodeChars, 0);
//                //string unicodeString = new string(unicodeChars);




//                if (!string.IsNullOrEmpty(msg) && phoneNumber.StartsWith("47"))
//                {
//                    ///  http://api.clickatell.com/http/sendmsg?user=lars.erik.finholt&password=Bdes4JG6&api_id=3384186&to=4795271895&text=Message&unicode=no
//                    using (var client = new WebClient())
//                    {
//                        var reqParams = new NameValueCollection();
//                        reqParams.Add("user", USER);
//                        reqParams.Add("text", msg);
//                        reqParams.Add("api_id", API_ID);
//                        reqParams.Add("password", PASSWORD);
//                        reqParams.Add("to", phoneNumber);
//                        reqParams.Add("unicode", "0");
//                        var response = client.UploadValues("http://api.clickatell.com/http/sendmsg", reqParams);
//                        var responsebody = (new System.Text.UTF8Encoding()).GetString(response);
//                        retval = responsebody;
//                    }
//                }
//                else
//                {
//                    retval = "invalid phonenumber";
//                }

//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            return retval;
//        }

//        public string SendReminder(Reminder reminder)
//        {
//            ReminderState nextState = ReminderState.StopSending;
//            var msg = "";
//            switch (reminder.Goal.ReminderState)
//            {
//                case Goaly.Models.ReminderState.NonSent:
//                    msg = "First warning: You're getting behind on goal <b>" + reminder.Goal.Title + "</b>. Current value is " + reminder.CurrentLevel;
//                    nextState = ReminderState.FirstWarning;
//                    break;
//                case Goaly.Models.ReminderState.FirstWarning:
//                    msg = "Second warning: You're getting behind on goal <b>" + reminder.Goal.Title + "</b>. Current value is " + reminder.CurrentLevel;
//                    nextState = ReminderState.SecondWarning;
//                    break;
//                case Goaly.Models.ReminderState.SecondWarning:
//                    msg = "Last warning: You're getting behind on goal <b>" + reminder.Goal.Title + "</b>. Current value is " + reminder.CurrentLevel;
//                    nextState = ReminderState.StopSending;
//                    break;
//                case Goaly.Models.ReminderState.StopSending:
//                    break;
//                default:
//                    break;
//            }

//            if (!string.IsNullOrEmpty(msg))
//            {
//                var db = new ModelContext();
//                var setting = db.Settings.Where(s => s.UserName == reminder.Goal.UserName).FirstOrDefault();
//                var email = setting.Email;
//                var phone = "47" + setting.PhoneNumber;


//                var retval = Notify(email, msg, reminder.Goal.Title, phone);
//                if (reminder.Goal.ReminderState == ReminderState.FirstWarning)
//                {
//                    var sms = "Oopps! Time to do some work on '" + reminder.Goal.Title + "'. Current value is " + reminder.CurrentLevel + ". Go fix it! http://goaly.no";
//                    SendSMS(phone, sms);
//                    var logger = new HistoryLogger(db);
//                    logger.Add(reminder.Goal.UserName, null, LogEntryType.FallingBehind, reminder.Goal.Title, reminder.Goal.Amount.ToString("#.##"), null, null, reminder.Goal.Unit);

//                }
//                var goal = db.Goals.Find(reminder.Goal.Id);
//                goal.ReminderState = nextState;
//                db.SaveChanges();
//                return retval;
//            }
//            return reminder.Goal.UserName + " - (stopped)";
//        }
//    }
//}