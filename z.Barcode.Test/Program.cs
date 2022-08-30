using System;
using System.Drawing;
using System.IO;

namespace z.Barcode.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rendering QR");

            var br = new BarcodeWriter();
            br.Format = BarcodeFormat.QR_CODE;
            var bmp = br.Write("hello this is qr data");

            bmp.Save($@"c:\temp\QR-{Guid.NewGuid().ToString("N")}.jpg");


            GenerateZXingQRCode();

            Console.WriteLine("Rendering QR Completed");

            //


        }

        static void GenerateZXingQRCode()
        {
           
            var mpc = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var img = Path.Combine(mpc, "Resources", "Picture1.png");

            Bitmap overlay = new Bitmap(img);
 
            var qrCode = new QRWriter();
            var bmp = qrCode.Create("Hello Fuckers").GetGraphic(overlay, iconBorderWidth: 5);
            bmp.Save($@"c:\temp\QR-{Guid.NewGuid().ToString("N")}.jpg");
        }
    }
}
