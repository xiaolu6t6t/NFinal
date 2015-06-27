using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NFinal.Template
{
    public class Compress
    {
        public static string GetHexGz(string html)
        {
            StringBuilder  sbHexGz=new StringBuilder();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
            MemoryStream ms = new MemoryStream();
            GZipStream gz = new GZipStream(ms, CompressionMode.Compress);
            gz.Write(buffer, 0, buffer.Length);
            gz.Close();
            buffer = ms.ToArray();
            ms.Close();
            for (int i = 0; i < buffer.Length; i++)
            {
                sbHexGz.Append( buffer[i].ToString("X2"));
            }
            return sbHexGz.ToString();
        }
        public static byte[] GetBytesGz(string html)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
            MemoryStream ms = new MemoryStream();
            GZipStream gz = new GZipStream(ms, CompressionMode.Compress);
            gz.Write(buffer, 0, buffer.Length);
            gz.Close();
            buffer=ms.ToArray();
            ms.Close();
            return buffer;
        }
        public static byte[] RemoveHeader(byte[] buffer)
        {
            byte[] result=new byte[buffer.Length -2];
            for(int i=2;i<buffer.Length;i++)
            {
                result[i-2]=buffer[i];
            }
            return result;
        }
        public static string GetHexDef(string html)
        {
            StringBuilder sbHexGz = new StringBuilder();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
            MemoryStream ms = new MemoryStream();
            DeflateStream gz = new DeflateStream(ms, CompressionMode.Compress);
            gz.Write(buffer, 0, buffer.Length);
            gz.Close();
            buffer = ms.ToArray();
            ms.Close();
            for (int i = 0; i < buffer.Length; i++)
            {
                sbHexGz.Append(buffer[i].ToString("X2"));
            }
            return sbHexGz.ToString();
        }
    }
}