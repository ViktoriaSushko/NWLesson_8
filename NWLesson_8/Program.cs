using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net;
using System.Net.Mail;

string password_app = "application password";
string fromEmail = "from@gmail.com";
string toEmail = "to@gmail.com";
string attachmentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "scotish-breed.png");
string attachmentPath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "ford-focus.jpg");
//1
//await SendMessageMailKit(password_app, fromEmail, toEmail, attachmentPath);
//2
await SendMessage(password_app, fromEmail, toEmail, attachmentPath);

async Task SendMessage(string password_app, string fromEmail, string toEmail, string attachmentPath)
{
    MailAddress from = new MailAddress(fromEmail, "Vika");
    MailAddress to = new MailAddress(toEmail);
    MailMessage message = new MailMessage(from, to);
    message.Subject= "Тестовий лист!";
    message.Body= @"<h1 style='color:green;font-size:18px'>Текстовий лист працює з smtp-client</h1></hr><p style='color:blue;font-style:italic;font-family:Georgia,serif;font-size:16px;'>Цей текстовий лист надісланий через з smtp-client</p>";
    message.IsBodyHtml = true;
    if (File.Exists(attachmentPath1))
    {
        Attachment attachment = new Attachment(attachmentPath1);
        message.Attachments.Add(attachment);
        Console.WriteLine("Вкладення додано!");
    }
    else
        Console.WriteLine($"Вкладення не знайдено за цим шляхом {attachmentPath1}");
    System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
    smtpClient.Credentials = new NetworkCredential(fromEmail, password_app);
    smtpClient.EnableSsl = true;
    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.Timeout = 30000;
    try
    {
        //smtpClient.Send(message);
        await Task.Run(() =>
        {
            smtpClient.Send(message);
            Console.WriteLine("Лист успішно надіслано!");
        });
    }
    catch (SmtpException ex)
    {

        Console.WriteLine($"Загальна помилка при надсилані листа SMTP {ex.Message}");
    }
    catch (Exception ex)
    {

        Console.WriteLine($"Загальна помилка при надсилані листа {ex.Message}");
    }
}

async Task SendMessageMailKit(string password_app, string fromEmail, string toEmail, string attachmentPath)
{
    MimeMessage? email = new MimeMessage();
    email.From.Add(new MailboxAddress("Vika", fromEmail));
    email.To.Add(new MailboxAddress("", toEmail));
    email.Subject = "Тестовий лист з MailKIT";
    var builder = new BodyBuilder();
    builder.HtmlBody = @"<h1 style='color:green;font-size:16px'>Текстовий лист працює з smtp-client</h1></hr><p style='font-style:italic;font-family:Georgia,serif;font-size:14px;'>Цей текстовий лист надісланий через з smtp-client</p>";
    if (File.Exists(attachmentPath))
    {
        Attachment attachment = new Attachment(attachmentPath);
        await builder.Attachments.AddAsync(attachmentPath);
        Console.WriteLine("Вкладення додано!");
    }
    else
        Console.WriteLine($"Вкладення не знайдено за цим шляхом {attachmentPath}");
    email.Body = builder.ToMessageBody();
    using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
    try
    {
        await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(fromEmail, password_app);
        await smtpClient.SendAsync(email, System.Threading.CancellationToken.None);
        await smtpClient.DisconnectAsync(true);
        Console.WriteLine("Лист успішно надіслано!");
    }
    catch (Exception ex)
    {

        Console.WriteLine($"Загальна помилка при надсилані листа {ex.Message}");
    }
}