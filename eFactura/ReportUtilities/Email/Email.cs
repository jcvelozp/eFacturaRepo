using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using ReportUtilities.Tools;

namespace ReportUtilities
{
    public class Email
    {
        public string from { get; set; }
        private List<string> to = new List<string>();
        private List<string> files = new List<string>();

        public List<string> toAddress {
            get {
                return to;
            }
            set {
                to = value;
            }
        }

        public List<string> Archivos {
            get {
                return files;
            }
            set {
                files = value;
            }
        }


        public string password { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        public bool Send()
        {
            try
            {
                password = Configuraciones.PasswordEmail;
                var fromAddress = new MailAddress(from, Configuraciones.NombreEmailEmisor);
                var smtp = new SmtpClient
                {
                    Host = Configuraciones.HostEmail,
                    Port = Convert.ToInt32(Configuraciones.PuertoEmail),
                    EnableSsl = Convert.ToBoolean(Configuraciones.HabilitarSSL),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(from, password)
                };
                MailMessage message = new MailMessage();
                message.Subject = subject;
                message.Body = body;
                message.From = new MailAddress(from);
                message.IsBodyHtml = true;

                foreach (var item in to)
                {
                    message.To.Add(new MailAddress(item));
                }

                if (files != null) {
                    foreach (var item in files)
                    {
                        message.Attachments.Add(new Attachment(item));
                    }
                }

                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                if (!Configuraciones.ModoServicio)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
                Logs.WriteErrorLog(ex);
            }
            return false;
        }

    }
}
