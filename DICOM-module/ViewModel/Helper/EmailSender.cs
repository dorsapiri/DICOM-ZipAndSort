
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace DICOM_module.ViewModel.Helper
{
    internal class EmailSender
    {
        public EmailSender()
        {
        }

        internal void senEmailWithAttachment(string recipient, string subject, string body, string attachmentPath)
        {
            try 
            {
                //Create a new meilMessage
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.From = new MailAddress("dorsapiri@gmail.com");
                mailMessage.To.Add(recipient);

                Attachment attacheZipfile = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attacheZipfile);

                //Create and config SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("dorsapiri@gmail.com", "password");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMessage);

                MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}