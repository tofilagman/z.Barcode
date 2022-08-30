using System;
using System.Drawing;
using System.IO;
using z.Barcode.QrCode.Internal;
using z.Barcode.Rendering;

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
            var bw = new BarcodeWriter();

            var encOptions = new Common.EncodingOptions
            {
                Width = 500,
                Height = 500,
                Margin = 3,
                PureBarcode = false
            };

            encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            bw.Renderer = new BitmapRenderer();
            bw.Options = encOptions;
            bw.Format = BarcodeFormat.QR_CODE;
            Bitmap bm = bw.Write("Hello fuckers");

            var mpc = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var img = Path.Combine(mpc, "Resources", "Picture1.png");

            Bitmap overlay = new Bitmap(img);

            int deltaHeigth = bm.Height - overlay.Height;
            int deltaWidth = bm.Width - overlay.Width;

            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));

            bm.Save($@"c:\temp\QR-{Guid.NewGuid().ToString("N")}.jpg");
        }
    }
}
