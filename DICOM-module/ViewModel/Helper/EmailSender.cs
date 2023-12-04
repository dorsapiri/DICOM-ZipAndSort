
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;
//using System.Net.Mail;
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
                MimeMessage mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("Behyar", "dorsapiri@chmail.ir"));
                mailMessage.To.Add(new MailboxAddress("Recipient", recipient));
                mailMessage.Subject = subject;

                // Body of the email
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "DICOM Files";
               
                //Attachment
                bodyBuilder.Attachments.Add(attachmentPath);

                mailMessage.Body = bodyBuilder.ToMessageBody();

                // Create and configure the SmtpClient
                using (SmtpClient smtpClient = new SmtpClient())
                { 
                    smtpClient.Connect("smtp.chmail.ir", 465,true);
                    smtpClient.Authenticate("dorsapiri@chmail.ir", "Behyar1402");

                    // Send the email
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }
    }
}