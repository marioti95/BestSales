using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;

class Program
{
    static void Main(string[] args)
    {
        //Tu bedzie pobierac mail
        Console.WriteLine("Podaj adres e-mail:");
        var userEmail = Console.ReadLine();

        var confirmationToken = GenerateConfirmationToken(userEmail);

        SendConfirmationEmail(userEmail, confirmationToken);

        //To na stronie bedzie sie wyswietlac
        Console.WriteLine("E-mail z linkiem potwierdzającym został wysłany.");
    }

    private static string GenerateConfirmationToken(string userEmail)
    {
        var tokenLength = 32;
        var random = new Random();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var token = new char[tokenLength];

        for (int i = 0; i < tokenLength; i++)
        {
            var seed = (userEmail + i).GetHashCode();
            random = new Random(seed);

            token[i] = chars[random.Next(chars.Length)];
        }

        return new string(token);
    }

    private static void SendConfirmationEmail(string userEmail, string confirmationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("BestSales", "BestSales@gmail.com"));
        message.To.Add(new MailboxAddress("", userEmail));
        message.Subject = "Potwierdzenie rejestracji";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.TextBody = $"Witaj! Dziękujemy za rejestrację w naszej aplikacji. Kliknij w poniższy link, aby potwierdzić swoje konto:\n\nhttps://BestSales.pl/potwierdzenie?token={confirmationToken}";

        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.AuthenticateAsync("BestSales@gmail.com", "haslo123");
            client.SendAsync(message);
            client.DisconnectAsync(true);
        }

        Console.WriteLine($"E-mail z linkiem potwierdzającym został wysłany na adres {userEmail}.");
    }
}
