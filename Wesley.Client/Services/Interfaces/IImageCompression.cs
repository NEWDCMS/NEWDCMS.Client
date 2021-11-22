using System;
using System.Collections.Generic;
using System.Text;

namespace Wesley.Client.Services
{
    public interface IImageCompression
    {
        //byte[] CompressImage(byte[] imageData, string destinationPath, int compressionPercentage);
        byte[] CompressImage(byte[] imageData, int compressionPercentage);
    }
}
