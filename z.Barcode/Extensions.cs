using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace z.Barcode
{
    public static class Extensions
    {

        /// <summary>
        /// Generate Barcode
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Bitmap GenerateBarcode(this string data, BarcodeFormat format = BarcodeFormat.QR_CODE)
        {
            BarcodeWriter br = new BarcodeWriter();
            br.Format = format;
            return br.Write(data);
        }

        public static Bitmap GenerateBarcode(this string data, Color color, BarcodeFormat format = BarcodeFormat.QR_CODE)
        {
            BarcodeWriter br = new BarcodeWriter();
            br.Options.Color = color;
            br.Format = format;
            return br.Write(data);
        }

        /// <summary>
        /// Generate Barcode with Options
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="BarcodeOnly"></param>
        /// <returns></returns>
        public static Bitmap GenerateBarcode(this string data, BarcodeFormat format, int width, int height, bool BarcodeOnly = false)
        {
            BarcodeWriter br = new BarcodeWriter();
            br.Format = format;
            br.Options.Width = width;
            br.Options.Height = height;
            br.Options.PureBarcode = BarcodeOnly;
            return br.Write(data);
        }

        /// <summary>
        /// Generate Barcode with Options
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="BarcodeOnly"></param>
        /// <returns></returns>
        public static Bitmap GenerateBarcode(this string data, BarcodeFormat format, int width, int height, Color color ,bool BarcodeOnly = false)
        {
            BarcodeWriter br = new BarcodeWriter();
            br.Format = format;
            br.Options.Color = color;
            br.Options.Width = width;
            br.Options.Height = height;
            br.Options.PureBarcode = BarcodeOnly;
            return br.Write(data);
        }

        /// <summary>
        /// Example #ffffff
        /// </summary>
        /// <param name="colorhex"></param>
        /// <returns></returns>
        public static Color FromHtml(this string colorhex)
        {
            return System.Drawing.ColorTranslator.FromHtml(colorhex);
        }

        /// <summary>
        /// Convert Bitmap Image to Base64 - use for HTML
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static string ToBase64String(this Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;

            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, imageFormat);

            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();

            memoryStream.Close();

            base64String = Convert.ToBase64String(byteBuffer);
            byteBuffer = null;

            return base64String;
        }

        /// <summary>
        /// Convert Bitmap Image to Base64 and Automatically Generate Image Url
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="imageformat"></param>
        /// <returns></returns>
        public static string ToBase64StringImgData(this Bitmap bmp, ImageFormat imageformat){
            return string.Format("data:image/{0};base64,{1}", imageformat.ToString(), bmp.ToBase64String(imageformat));
        }

    }
}
