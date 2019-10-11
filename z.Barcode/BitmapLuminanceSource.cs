﻿/*
* Copyright 2012 z.Barcode.Net authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace z.Barcode
{
   public partial class BitmapLuminanceSource : BaseLuminanceSource
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      protected BitmapLuminanceSource(int width, int height)
         : base(width, height)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapLuminanceSource"/> class
      /// with the image of a Bitmap instance
      /// </summary>
      /// <param name="bitmap">The bitmap.</param>
      public BitmapLuminanceSource(Bitmap bitmap)
         : base(bitmap.Width, bitmap.Height)
      {
         var height = bitmap.Height;
         var width = bitmap.Width;

         // In order to measure pure decoding speed, we convert the entire image to a greyscale array
         luminances = new byte[width * height];

         // The underlying raster of image consists of bytes with the luminance values
#if WindowsCE
         var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
#else
         var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
#endif
         try
         {
            var stride = Math.Abs(data.Stride);
            var pixelWidth = stride / width;

            if (pixelWidth > 4)
            {
               // old slow way for unsupported bit depth
               Color c;
               for (int y = 0; y < height; y++)
               {
                  int offset = y * width;
                  for (int x = 0; x < width; x++)
                  {
                     c = bitmap.GetPixel(x, y);
                     luminances[offset + x] = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B + 0.01);
                  }
               }
            }
            else
            {
               var strideStep = data.Stride;
               var buffer = new byte[stride];
               var ptrInBitmap = data.Scan0;
               
#if !WindowsCE
               // prepare palette for 1 and 8 bit indexed bitmaps
               var luminancePalette = new byte[bitmap.Palette.Entries.Length];
               for (var index = 0; index < bitmap.Palette.Entries.Length; index++)
               {
                  var color = bitmap.Palette.Entries[index];
                  luminancePalette[index] = (byte)(0.3 * color.R +
                                                    0.59 * color.G +
                                                    0.11 * color.B + 0.01);
               }
               if (bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                   bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
               {
                  pixelWidth = 40;
               }
#endif

               for (int y = 0; y < height; y++)
               {
                  // copy a scanline not the whole bitmap because of memory usage
                  Marshal.Copy(ptrInBitmap, buffer, 0, stride);
#if NET40
                  ptrInBitmap = IntPtr.Add(ptrInBitmap, strideStep);
#else
                  ptrInBitmap = new IntPtr(ptrInBitmap.ToInt64() + strideStep);
#endif
                  var offset = y * width;
                  switch (pixelWidth)
                  {
#if !WindowsCE
                     case 0:
                        for (int x = 0; x * 8 < width; x++)
                        {
                           for (int subX = 0; subX < 8 && 8 * x + subX < width; subX++)
                           {
                              var index = (buffer[x] >> (7 - subX)) & 1;
                              luminances[offset + 8 * x + subX] = luminancePalette[index];
                           }
                        }
                        break;
                     case 1:
                        for (int x = 0; x < width; x++)
                        {
                           luminances[offset + x] = luminancePalette[buffer[x]];
                        }
                        break;
#endif
                     case 2:
                        // should be RGB565 or RGB555, assume RGB565
                        {
                           for (int index = 0, x = 0; index < 2 * width; index += 2, x++)
                           {
                              var byte1 = buffer[index];
                              var byte2 = buffer[index + 1];

                              var b5 = byte1 & 0x1F;
                              var g5 = (((byte1 & 0xE0) >> 5) | ((byte2 & 0x03) << 3)) & 0x1F;
                              var r5 = (byte2 >> 2) & 0x1F;
                              var r8 = (r5 * 527 + 23) >> 6;
                              var g8 = (g5 * 527 + 23) >> 6;
                              var b8 = (b5 * 527 + 23) >> 6;

                              luminances[offset + x] = (byte)(0.3 * r8 + 0.59 * g8 + 0.11 * b8 + 0.01);
                           }
                        }
                        break;
                     case 3:
                        for (int x = 0; x < width; x++)
                        {
                           var luminance = (byte)(0.3  * buffer[x * 3] +
                                                  0.59 * buffer[x * 3 + 1] +
                                                  0.11 * buffer[x * 3 + 2] + 0.01);
                           luminances[offset + x] = luminance;
                        }
                        break;
                     case 4:
                        // 4 bytes without alpha channel value
                        for (int x = 0; x < width; x++)
                        {
                           var luminance = (byte)(0.30 * buffer[x * 4] +
                                                  0.59 * buffer[x * 4 + 1] +
                                                  0.11 * buffer[x * 4 + 2] + 0.01);

                           luminances[offset + x] = luminance;
                        }
                        break;
                     case 40:
                        // with alpha channel; some barcodes are completely black if you
                        // only look at the r, g and b channel but the alpha channel controls
                        // the view
                        for (int x = 0; x < width; x++)
                        {
                           var luminance = (byte)(0.30 * buffer[x * 4] +
                                                  0.59 * buffer[x * 4 + 1] +
                                                  0.11 * buffer[x * 4 + 2] + 0.01);

                           // calculating the resulting luminance based upon a white background
                           // var alpha = buffer[x * pixelWidth + 3] / 255.0;
                           // luminance = (byte)(luminance * alpha + 255 * (1 - alpha));
                           var alpha = buffer[x * 4 + 3];
                           luminance = (byte)(((luminance * alpha) >> 8) + (255 * (255 - alpha) >> 8));
                           luminances[offset + x] = luminance;
                        }
                        break;
                     default:
                        throw new NotSupportedException();
                  }
               }
            }
         }
         finally
         {
            bitmap.UnlockBits(data);
         }
      }

      /// <summary>
      /// Should create a new luminance source with the right class type.
      /// The method is used in methods crop and rotate.
      /// </summary>
      /// <param name="newLuminances">The new luminances.</param>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <returns></returns>
      protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
      {
         return new BitmapLuminanceSource(width, height) { luminances = newLuminances };
      }
   }
}