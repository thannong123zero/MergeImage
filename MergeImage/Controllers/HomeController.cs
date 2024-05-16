using IronBarCode;
using MergeImage.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
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
            //GenderData();
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "LocalData\\VoucherData.json");
            using StreamReader reader = new(filePath);
            var json = reader.ReadToEnd();
            List<Voucher> vouchers = JsonConvert.DeserializeObject<List<Voucher>>(json);

            //ExportVouchersToExcel(vouchers, Path.Combine(_webHostEnvironment.WebRootPath, "LocalData\\VoucherData.xlsx"));
            MergeImage();
            return View(vouchers);
        }

        public IActionResult Privacy()
        {

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "QRCode");
            for(int i = 1; i <= 10; i++)
            {
                string code = "CF_" + i.ToString("D4");
                string fileName = string.Concat(code, ".png");
                GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(GenerateRandomString(6), 280);
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
        public void GenderData()
        {
            List<Voucher> vouchers = new List<Voucher>();
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "LocalData\\VoucherData.json");
            //using StreamReader reader = new(filePath);
            //var json = reader.ReadToEnd();
            for (int i = 1; i <= 5000; i++)
            {
                string key = "CF_" + i.ToString("D4");
                string code = GenerateRandomString(8);
                //How to check code is exist in voucher list
                bool check = vouchers.Any(x => x.Code == code);
                while (check)
                {
                    code = GenerateRandomString(8);
                    check = vouchers.Any(x => x.Code == code);
                }
                vouchers.Add(new Voucher { Key = key, Code = code });
            }
            string jsonData = JsonConvert.SerializeObject(vouchers);
            System.IO.File.WriteAllText(filePath, jsonData);

        }
        public void MergeImage()
        {
            // Load the large image
            Image largeImage = Image.FromFile("wwwroot/images/c81bb228f0c2519c08d3.png");

            // Load the small image
            //Image smallImage = Image.FromFile("wwwroot/images/MOC_004.png");

            // Create graphics from the large image
            Graphics graphics = Graphics.FromImage(largeImage);

            // Define where you want to place the small image (x, y)
            int x = 780;
            int y = 1625;

            for (int i = 1; i <= 5; i++)
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
        }
        public void ExportVouchersToExcel(List<Voucher> vouchers, string filePath)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Vouchers");

                // Write headers
                worksheet.Cells[1, 1].Value = "Key";
                worksheet.Cells[1, 2].Value = "Code";

                // Write data
                for (int i = 0; i < vouchers.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = vouchers[i].Key;
                    worksheet.Cells[i + 2, 2].Value = vouchers[i].Code;
                }

                // Save to file
                package.SaveAs(new FileInfo(filePath));
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
