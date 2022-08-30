using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using z.Barcode.QrCode.Internal;
using z.Barcode.Rendering;

namespace z.Barcode
{
    public class QRWriter
    {
        private Bitmap QRBitmap;

        public QRWriter Create(string data, int width = 500, int height = 500, int margin = 3)
        {
            var bw = new BarcodeWriter();

            var encOptions = new Common.EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = margin,
                PureBarcode = false
            };

            encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            bw.Renderer = new BitmapRenderer();
            bw.Options = encOptions;
            bw.Format = BarcodeFormat.QR_CODE;

            QRBitmap = bw.Write(data);

            return this;
        }

        public Bitmap GetGraphic() => QRBitmap;

        public Bitmap GetGraphic(Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 0, Color? iconBackgroundColor = null)
        { 
            var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

            if (!drawIconFlag)
                return GetGraphic();

            using (var gfx = Graphics.FromImage(QRBitmap))
            { 
                var iconDestWidth = iconSizePercent * QRBitmap.Width / 100f;
                var iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
                var iconX = (QRBitmap.Width - iconDestWidth) / 2;
                var iconY = (QRBitmap.Height - iconDestHeight) / 2;
                var centerDest = new RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth +iconBorderWidth * 2, iconDestWidth + iconBorderWidth * 2);
                var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                var iconBgBrush = iconBackgroundColor != null ? new SolidBrush((Color)iconBackgroundColor): new SolidBrush(Color.White);

                //only render icon/logo background, if iconborderWitdh is set > 0
                if(iconBorderWidth > 0)
                {
                    using(var iconPath = CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2) )
                    {
                        gfx.FillPath(iconBgBrush, iconPath);
                    }
                }
                gfx.DrawImage(icon, iconDestRect, new RectangleF(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
                gfx.Save();
            }
            return QRBitmap;
        }

        internal GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {
            var roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}
