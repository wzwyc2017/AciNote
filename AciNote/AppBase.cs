﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AciNote
{
    public class AppBase
    {
        private const string AppName = "AciNote";
        private const string ConfigFileName = "config.ini";
        public static string JcSecretKey = "www.cnsos.net";
        public static string FileFilter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";

        #region 获取文件编码
        /// <summary>
        /// 获取文本文件的编码格式
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static System.Text.Encoding GetFileEncodeType(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 把颜色的字符串值转换成画刷
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static System.Windows.Media.Brush ConvertStringToBrush(string value)
        {
            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        private static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        /// <summary>
        /// 将画刷转换成颜色
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static System.Drawing.Color ConvertBrushToColor(System.Windows.Media.Brush brush)
        {
            var convert = new System.Drawing.ColorConverter();
            System.Drawing.Color color = (System.Drawing.Color)(convert.ConvertFromString(brush.ToString()));

            return color;
        }

        /// <summary>
        /// 将颜色转换为画刷
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Windows.Media.Brush ConvertColorToBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
        #endregion
    }
}
