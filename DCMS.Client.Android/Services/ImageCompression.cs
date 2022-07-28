using Android.Graphics;
using Android.Media;
using DCMS.Client.Droid.Services;
using DCMS.Client.Services;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageCompression))]
namespace DCMS.Client.Droid.Services
{
    public class ImageCompression : IImageCompression
    {
        public ImageCompression() { }

        public byte[] CompressImage(byte[] imageData, int compressionPercentage)
        {
            return GetResizedImage(imageData, compressionPercentage);
        }

        private byte[] GetResizedImage(byte[] imageData, int compressionPercentage)
        {
            Bitmap img = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            var scaledBitmap = ResizeImage(img);
            using (MemoryStream ms = new MemoryStream())
            {
                scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 20, ms);
                return ms.ToArray();
            }
        }


        private Matrix GetMatrix()
        {
            int rotate = 0;
            int orientation = 6;
            switch (orientation)
            {
                case (int)Orientation.Rotate270:
                    rotate = 270;
                    break;
                case (int)Orientation.Rotate180:
                    rotate = 180;
                    break;
                case (int)Orientation.Rotate90:
                    rotate = 90;
                    break;
            }
            Matrix matrix = new Matrix();
            matrix.PreRotate(rotate);
            GC.Collect();
            return matrix;
        }

        //private Bitmap ResizeAndReduceQuality(Bitmap bitmap, int compressionPercentage)
        //{
        //    var ResizedBitmap = ResizeImage(bitmap);
        //    var data = BitmapToArray(ResizedBitmap, compressionPercentage);
        //    ResizedBitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
        //    GC.Collect();
        //    return ResizedBitmap;
        //}

        private Bitmap ResizeImage(Bitmap sourceImage)
        {
            int SourceWidth = sourceImage.Width;
            int SourceHeight = sourceImage.Height;
            int ScaledWidth = 500;
            int ScaledHeight = 500;

            if (SourceWidth < SourceHeight)
            {
                ScaledHeight = 500;
                ScaledWidth = 500;
            }
            float Percent = 0;
            float PercentWidth = 0;
            float PercentHeight = 0;

            PercentWidth = ((float)ScaledWidth / (float)SourceWidth);
            PercentHeight = ((float)ScaledHeight / (float)SourceHeight);

            if (PercentHeight < PercentWidth)
                Percent = PercentHeight;
            else
                Percent = PercentWidth;

            int destWidth = (int)(SourceWidth * Percent);
            int destHeight = (int)(SourceHeight * Percent);

            Matrix matrix = GetMatrix();
            var scaledBitmap = Bitmap.CreateBitmap(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height, matrix, false);
            GC.Collect();
            return scaledBitmap;
        }




        private String HttpUploadFile(string url, Bitmap img, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            System.IO.Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, Guid.NewGuid() + ".png", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            img.Compress(Bitmap.CompressFormat.Png, 100, rs);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                System.IO.Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                return null;
            }
            finally
            {
                wr = null;
            }
        }
    }
}