using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        string product = "Laptopy";
        int pageCount = 2;
        string urlLaptopy = "https://www.x-kom.pl/g-2/c/159-laptopy-notebooki-ultrabooki.html";
        string urlSmartfony = "https://www.x-kom.pl/g-4/c/1590-smartfony-i-telefony.html";
        string urlTelewizory = "https://www.x-kom.pl/g-8/c/1117-telewizory.html";
        Parser(urlLaptopy, pageCount,product);
        product = "Smartfony";
        Parser(urlSmartfony, pageCount,product);
        product = "Telewizory";
        Parser(urlTelewizory, pageCount,product);
        Console.ReadLine();
    }
    private static void Parser(string urlBase, int pageCount,string productName)
    {
        string div = "//div[@class='sc-1s1zksu-0 dzLiED sc-162ysh3-1 irFnoT']";
        // klient HTTP
        var client = new HttpClient();
        // Obejscie ciastek
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

        // Parsowanie zawartości HTML przy użyciu HtmlAgilityPack
        var document = new HtmlDocument();

        for (int i = 1; i <= pageCount; i++)
        {
            // Tworzenie adresu URL dla kolejnej strony
            string url = $"{urlBase}?page={i}";

            // Pobranie zawartości strony
            var response = client.GetAsync(url).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            // Parsowanie zawartości HTML przy użyciu HtmlAgilityPack
            document.LoadHtml(content);

            // Znalezienie elementów zawierających informacje o produktach
            var products = document.DocumentNode.SelectNodes(div);

            foreach (var product in products)
            {
                var name = product.Descendants("h3").First().InnerText.Trim();
                var link = product.Descendants("a").First().Attributes["href"].Value;
                var price = product.Descendants("span").FirstOrDefault(n => n.Attributes["data-name"]?.Value == "productPrice")?.InnerText.Trim();
                var image = product.Descendants("img").First().Attributes["src"].Value;
                // Tu bedzie docelowe polaczenie z baza danych jak Dimytr wyzdrowieje 
                //InsertDataToTable(productName, name, price, link, image);
            }
        }
    }
    static void InsertDataToTable(string product, string name, string price, string link, string image)
    {
        string connectionString = "Server=localhost;Database=myDatabase;Uid=myUsername;Pwd=myPassword;";
        string query = $"INSERT INTO {product} (name, price, link, image) VALUES (@Name, @Price, @Link, @Image)";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Link", link);
                command.Parameters.AddWithValue("@Image", image);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

}
