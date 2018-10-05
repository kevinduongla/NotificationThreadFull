using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Transactions;
using Notification.Processor.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mail;

namespace Notification.Processor
{
    class NotificationServer: INotificationServer
    {
        private bool _shouldWork;
        public void Initialize()
        {
            _shouldWork = true;
        }
        public void Start()
        {
            var workingThread = new Thread(new ThreadStart(SendNotification));
            workingThread.Start();
        }
        public void Stop()
        {
            _shouldWork = false;
        }
        public void Pause()
        {
        }
        public void Resume()
        {
        }
        void SendNotification()
        {
            Console.Write("Waiting for notification \n");

            do
            {
                try
                {
                    // declare the transaction options
                    var transactionOptions = new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted
                    };
                   
                    using (var transScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                    {
                        using (var db = new NotificationEntities())
                        {
                            var unsentList = db.CustomerNotifications.Include("Customer").Where(n => n.NotificationSent == false).ToList();
                            var rowId = 1;
                            StreamWriter streamWriter = new StreamWriter(@"D:\Log\NotificationLog.txt",true);

                            foreach (var item in unsentList)
                            {
                                var notificationModel = new NotificationModel();
                                notificationModel.FirstName = item.Customer.ContactFirstName;
                                notificationModel.CurrentExpense = item.CurrentExpense;

                                if (!string.IsNullOrEmpty(item.Customer.ContactEmail))
                                {
                                    if (IsValidEmail(item.Customer.ContactEmail))
                                    {
                                        streamWriter.WriteLine("Client Name: " + item.Customer.ContactFirstName + " - Contact email: " + item.Customer.ContactEmail + " - Sent On: " + DateTime.Today.ToString());
                                        Console.Write("Start sending \n");
                                        var sender = "kevinduongla@gmail.com";

                                        MailMessage mailMessage = new MailMessage(sender, item.Customer.ContactEmail);

                                        mailMessage.Subject = "Notification";
                                        mailMessage.Body = NotificationTemplate.EmailTemplate(notificationModel);

                                        mailMessage.IsBodyHtml = true;
                                        SmtpClient smtpClient = new SmtpClient();
                                        smtpClient.Send(mailMessage);

                                        item.NotificationSent = true;
                                        db.SaveChanges();
                                        Console.Write("Complete sending \n");
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("Failed Row " + rowId.ToString() + ":" + item.Customer.ContactEmail);
                                    }
                                }
                            }
                            //Close Log file
                            streamWriter.Close();   

                        }
                        transScope.Complete();
                     
                    }
                }
                catch (Exception x)
                {
                    Console.Write("transScope.Complete exception, sleeping 10 sec", x);
                    Thread.Sleep(10000);

                }

                Thread.Sleep(Settings.Default.FetchingIntervalSeconds);

            } while (_shouldWork);
        }
        public static bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(email))
                    return (true);
                else
                    return (false);
            }
            else
            {
                return (false);
            }
        }

    }
}
