using IronBarCode;
using MergeImage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace MergeImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Load the large image
            Image largeImage = Image.FromFile("wwwroot/images/6aee5048cca06dfe34b1.png");

            // Load the small image
            //Image smallImage = Image.FromFile("wwwroot/images/MOC_004.png");

            // Create graphics from the large image
            Graphics graphics = Graphics.FromImage(largeImage);

            // Define where you want to place the small image (x, y)
            int x = 860;
            int y = 1660;

            for (int i = 1; i <= 10; i++)
            {
                string code = "CF_" + i.ToString("D4");
                string fileName = string.Concat(code, ".png");
                Image qrCode = Image.FromFile("wwwroot/QRCode/" + fileName);
                //graphics.DrawImage(qrCode, new Point(0, 0));
                // Draw the small image on the large image
                graphics.DrawImage(qrCode, new Point(x, y));
                largeImage.Save("wwwroot/images/" + code + "1" + ".png");
            }

            // Save the new image

            // Clean up
            graphics.Dispose();


            return View();
        }

        public IActionResult Privacy()
        {

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "QRCode");
            for(int i = 1; i <= 10; i++)
            {
                string code = "CF_" + i.ToString("D4");
                string fileName = string.Concat(code, ".png");
                GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(GenerateRandomString(6), 200);
                barcode.AddAnnotationTextBelowBarcode(code);
                barcode.SetMargins(10);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                
                string filePath = Path.Combine(path, fileName);
                barcode.SaveAsPng(filePath);
            }


            return View();
        }
        public string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
