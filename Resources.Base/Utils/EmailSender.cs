using Microsoft.Extensions.Options;
using Resources.Base.SettingsModels;
using System;
using System.Net.Mail;

namespace Resources.Base.Utils
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailServerSettings> _emailServerSettings;

        public EmailSender(IOptions<EmailServerSettings> emailServerSettings)
        {
            _emailServerSettings = emailServerSettings;
        }

        public bool SendAdminEmailAboutAVideoReport(string videoId, string userId, string reportReason)
        {
            var subject = "Un video a primit REPORT!";
            var body = "<p>Un utilizator tocmai ce a dat report la un video</p><br>" +
                "<p><strong>Id Video:</strong> " + videoId + "</p><br>" +
                "<p><strong>Motiv report:</strong> " + reportReason + "</p><br>" +
                "<p><strong>Id Utilizator care a dat report:</strong >" + (userId ?? "utilizatorul nu este autentificat") + "</p><br>";

            return SendGenericEmail("office@weveed.com", subject, body);
        }

        public bool SendProducerEmailAfterAdminValidateAccount(string email, string producerName)
        {
            var subject = "Contul de producator a fost validat cu succes!";
            var body = "<p>Felicitari, <strong>" + producerName + "</strong>!</p><br>" +
                "<p><strong>Contul tau de producator a fost validat de catre administratorii platformei.</strong></p><br>" +
                "<p>De acum, productiile incarcate de tine vor fi afisate in pagina Descopera si vor rula in canalele WeVeed.</p><br>" +
                "<br><strong>Echipa WeVeed</strong><br><p>office@weveed.com</p><p>+40729123772</p>";

            return SendGenericEmail(email, subject, body);
        }

        public bool SendAdminEmailAfterProducerRegisters(string producerId, string producerName, string producerEmail, string userName)
        {
            var subject = "Producator nou pe WeVeed!";
            var body = "<p>Un producator tocmai ce si-a facut cont pe WeVeed</p><br>" +
                "<p><strong>Id Producator:</strong>" + producerId + "</p><br>" +
                "<p><strong>Nume Producator:</strong>" + producerName + "</p><br>" +
                "<p><strong>Nume Utilizator:</strong>" + userName + "</p><br>" +
                "<p><strong>Email Producator:</strong>" + producerEmail + "</p><br>";

            return SendGenericEmail("office@weveed.com", subject, body);
        }

        public bool SendAdminEmailAfterUserUpdatedToProducer(string producerId, string producerName)
        {
            var subject = "Producator nou pe WeVeed!";
            var body = "<p>Un utilizator a facut update la un cont de PRODUCATOR</p><br>" +
                "<p><strong>Id Producator:</strong>" + producerId + "</p><br>" +
                "<p><strong>Nume Producator:</strong>" + producerName + "</p><br>";

            return SendGenericEmail("office@weveed.com", subject, body);
        }

        public bool SendEmailToResetPassword(string email, string token, string userName)
        {
            var subject = "Resetarea parolei Weveed";

            var urlString = "https://www.weveed.com/rstpwd/" + token;
            var body = "<p>Hei <strong>" + userName + "</strong>!</p><br>" +
                "<p><strong>Ai solicitat modificarea parolei pentru contul tau WeVeed? Daca da, apasa pe link-ul de mai jos.</strong> Link-ul este valabil timp de 2 ore.</p><br>" +
                "<a href=\"" + urlString + "\">" + urlString + "</a><br><br>" +
                "<p>Daca nu, poti ignora acest email.</p><br>" +
                "<br><strong>Echipa WeVeed</strong><br><p>office@weveed.com</p><p>+40729123772</p>";

            return SendGenericEmail(email, subject, body);
        }

        public bool SendGenericEmail(string email, string subject, string body)
        {
            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient(_emailServerSettings.Value.SmtpClient);

                mail.From = new MailAddress(_emailServerSettings.Value.MailAddress);
                mail.To.Add(email);
                mail.Subject = subject;

                mail.IsBodyHtml = true;

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                mail.AlternateViews.Add(view);

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_emailServerSettings.Value.AddressUser, _emailServerSettings.Value.AddressPassword);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool SendRegisterConfirmationEmail(Guid guid, string email, string firstName)
        {
            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient(_emailServerSettings.Value.SmtpClient);

                mail.From = new MailAddress(_emailServerSettings.Value.MailAddress);
                mail.To.Add(email);
                mail.Subject = "WeVeed - Înregistrare";

                mail.IsBodyHtml = true;

                var urlString = "http://www.weveed.com/actx/" + guid;
                string body = "<p>Salutare, " + firstName + "!</p><br><p>Acum ești membru al platformei WeVeed. Ne bucurăm că faci parte din comunitatea noastră!</p><br><p>Apasă pe linkul de mai jos pentru a confirma adresa de email:</p><br><a href=\"" + urlString + "\">" + urlString + "</a><br><br><br><strong>Echipa WeVeed</strong><br><p>office@weveed.com</p><p>+40729123772</p>";

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                mail.AlternateViews.Add(view);

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_emailServerSettings.Value.AddressUser, _emailServerSettings.Value.AddressPassword);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool SendSNSConfirmationToken(string token)
        {
            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient(_emailServerSettings.Value.SmtpClient);

                mail.From = new MailAddress(_emailServerSettings.Value.MailAddress);
                mail.To.Add("stiuca.mihai@yahoo.com");
                mail.Subject = "Confirmare SNS";

                mail.IsBodyHtml = true;

                string body = $"<p>{token}</p>";

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                mail.AlternateViews.Add(view);

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_emailServerSettings.Value.AddressUser, _emailServerSettings.Value.AddressPassword);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
