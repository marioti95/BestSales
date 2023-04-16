using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

class Program
{
    static void Main(string[] args)
    {
        int pageCount = 2;
        string urlLaptopy = "https://www.x-kom.pl/g-2/c/159-laptopy-notebooki-ultrabooki.html";
        string urlSmartfony = "https://www.x-kom.pl/g-4/c/1590-smartfony-i-telefony.html";
        string urlTelewizory = "https://www.x-kom.pl/g-8/c/1117-telewizory.html";
        Parser(urlLaptopy, pageCount);
        Parser(urlSmartfony, pageCount);
        Parser(urlTelewizory, pageCount);
        Console.ReadLine();
    }
    private static void Parser(string urlBase, int pageCount)
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

            // Wyświetlenie nazw, linków, cen i adresów URL zdjęć dla produktów
            foreach (var product in products)
            {
                var name = product.Descendants("h3").First().InnerText.Trim();
                var link = product.Descendants("a").First().Attributes["href"].Value;
                var price = product.Descendants("span").FirstOrDefault(n => n.Attributes["data-name"]?.Value == "productPrice")?.InnerText.Trim();
                var image = product.Descendants("img").First().Attributes["src"].Value;

                Console.WriteLine();
                Console.WriteLine("Nazwa: " + name);
                Console.WriteLine("Cena: " + price);
                Console.WriteLine("Link: "+ link);
                Console.WriteLine("Image url: "+ image);
            }
        }
    }

}
