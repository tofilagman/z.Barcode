using System;

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

            bmp.Save($@"c:\temp\QR-{ Guid.NewGuid().ToString("N") }.jpg");

            Console.WriteLine("Rendering QR Completed");
        }
    }
}
