using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ADBDemo
{
    class CAPTCHA
    {
        Bitmap CAPTCHABitmap = null;
        int CAPTCHAWidth = 0;
        int CAPTCHAHeight = 0;

        byte[,] rband = null;
        byte[,] gband = null;
        byte[,] bband = null;

        public int DetectX = 0;
        public int DetectY = 0;

        public Bitmap bmp
        {
            get { return CAPTCHABitmap; }
            set { CAPTCHABitmap = value; }
        }

        public CAPTCHA(Bitmap bmp)
        {
            ReadCAPTCHA(bmp);
        }

        /// <summary>
        /// 读入需要处理的bmp图片,记录宽高。
        /// </summary>
        /// <param name="bmp"></param>
        public void ReadCAPTCHA(Bitmap bmp)
        {
            DetectX = 0;
            DetectY = 0;
            CAPTCHABitmap = (Bitmap)bmp.Clone();
            CAPTCHAWidth = bmp.Width;
            CAPTCHAHeight = bmp.Height;
            ReadToRGBTable();
            WriteRGBTableToBMP();
        }

        /// <summary>
        /// 把bmp图片写入到RGB数组里面，采用指针操作
        /// </summary>
        /// <param name="bmp"></param>
        private void ReadToRGBTable()
        {
            //http://blog.csdn.net/liehuo123/article/details/6110708    c#使用指针快速操作图片

            int depth = Bitmap.GetPixelFormatSize(CAPTCHABitmap.PixelFormat) / 8;

            Rectangle lockRect = new Rectangle(0, 0, CAPTCHAWidth, CAPTCHAHeight);
            BitmapData bmpData = CAPTCHABitmap.LockBits(lockRect, ImageLockMode.ReadOnly, CAPTCHABitmap.PixelFormat);

            rband = new byte[CAPTCHAHeight, CAPTCHAWidth];   // 彩色图片的R、G、B三层分别构造一个二维数组
            gband = new byte[CAPTCHAHeight, CAPTCHAWidth];
            bband = new byte[CAPTCHAHeight, CAPTCHAWidth];

            //Stride是Bitmap里一个令人头痛的东西，它代表着一张图片每一行的扫描宽度（跨距）。
            //跨距总是大于或等于实际像素宽度。如果跨距为正，则位图自顶向下。如果跨距为负，则位图颠倒。
            //Stride是指图像每一行需要占用的字节数。根据BMP格式的标准，Stride一定要是4的倍数。
            //举个例子，一幅1024*768的24bppRgb的图像，每行有效的像素信息应该是1024*3 = 3072。
            //因为已经是4的倍数，所以Stride就是3072。
            //那么如果这幅图像是35*30，那么一行的有效像素信息是105，但是105不是4的倍数，
            //所以填充空字节（也就是0），Stride应该是108。

            int rowOffset = bmpData.Stride - CAPTCHAWidth * depth;  //图像数据在内存中存储时是按4字节对齐的'

            // 采用效率更高的指针来获取图像像素点的值
            unsafe
            {
                byte* imgPtr = (byte*)bmpData.Scan0.ToPointer();

                for (int i = 0; i < CAPTCHAHeight; ++i)
                {
                    for (int j = 0; j < CAPTCHAWidth; ++j)
                    {
                        rband[i, j] = imgPtr[2];   // 每个像素的指针是按BGR的顺序存储的
                        gband[i, j] = imgPtr[1];
                        bband[i, j] = imgPtr[0];
                        int r = rband[i, j];
                        int g = gband[i, j];
                        int b = bband[i, j];
                        if (g==55 && r + b > 145 && r + b < 148)
                        {
                            DetectY = i;
                            DetectX = j;                            
                        }
                        imgPtr += depth;       // 偏移一个像素
                    }
                    imgPtr += rowOffset;   // 偏移到下一行
                }
            }
            CAPTCHABitmap.UnlockBits(bmpData);
        }

        /// <summary>
        /// 把RGB数组写入到bmp图片里面，采用指针操作
        /// </summary>
        /// <param name="bmp"></param>
        public void WriteRGBTableToBMP()
        {
            int depth = Bitmap.GetPixelFormatSize(CAPTCHABitmap.PixelFormat) / 8;

            Rectangle lockRect = new Rectangle(0, 0, CAPTCHAWidth, CAPTCHAHeight);
            BitmapData bmpData = CAPTCHABitmap.LockBits(lockRect, ImageLockMode.ReadOnly, CAPTCHABitmap.PixelFormat);

            int rowOffset = bmpData.Stride - CAPTCHAWidth * depth;  //图像数据在内存中存储时是按4字节对齐的'
            unsafe
            {
                byte* imgPtr = (byte*)bmpData.Scan0.ToPointer();
                for (int i = 0; i < CAPTCHAHeight; ++i)
                {
                    for (int j = 0; j < CAPTCHAWidth; ++j)
                    {
                        int r = rband[i, j];
                        int g = gband[i, j];
                        int b = bband[i, j];
                        if (Math.Abs(i-DetectY)<5 && Math.Abs(j-DetectX)<5)
                        {
                            imgPtr[2] = 180;   // 每个像素的指针是按BGR的顺序存储的
                            imgPtr[1] = 0;
                            imgPtr[0] = 0;
                        }
                        else
                        {
                            imgPtr[2] = rband[i, j];   // 每个像素的指针是按BGR的顺序存储的
                            imgPtr[1] = gband[i, j];
                            imgPtr[0] = bband[i, j];
                        }
                        imgPtr += depth;       // 偏移一个像素
                    }
                    imgPtr += rowOffset;   // 偏移到下一行
                }
            }
            CAPTCHABitmap.UnlockBits(bmpData);
        }

        public void ClearThreshold(int Threshold)
        {
            double r = 255;
            double g = 255;
            double b = 255;

            for (int i = 0; i < CAPTCHAHeight; i++)
            {
                for (int j = 0; j < CAPTCHAWidth; j++)
                {
                    if (i < 2 || j < 2 || i > CAPTCHAHeight - 3 || j > CAPTCHAWidth - 3)
                    {
                        rband[i, j] = (byte)255;
                        gband[i, j] = (byte)255;
                        bband[i, j] = (byte)255;
                    }
                    else
                    {
                        r = rband[i, j];
                        g = gband[i, j];
                        b = bband[i, j];

                        if (r + g + b > Threshold)
                        {
                            rband[i, j] = (byte)255;
                            gband[i, j] = (byte)255;
                            bband[i, j] = (byte)255;
                        }
                    }
                }
            }
        }

        public void ClearLines(int Threshold)
        {
            int count = 0;

            for (int i = 2; i < CAPTCHAHeight - 2; i++)
            {
                for (int j = 2; j < CAPTCHAWidth - 2; j++)
                {
                    count = 0;

                    //取垂直线白点数量
                    if (getSumcolor(i - 2, j) > Threshold) count++;
                    if (getSumcolor(i - 1, j) > Threshold) count++;
                    if (getSumcolor(i, j) > Threshold) count++;
                    if (getSumcolor(i + 1, j) > Threshold) count++;
                    if (getSumcolor(i + 2, j) > Threshold) count++;

                    if (count > 2)
                    {
                        rband[i, j] = (byte)255;
                        gband[i, j] = (byte)255;
                        bband[i, j] = (byte)255;
                    }
                }
            }
        }

        private int getSumcolor(int i, int j)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            r = rband[i, j];
            g = gband[i, j];
            b = bband[i, j];

            return r + g + b;
        }

        public void Clearoutliers(int step, int Threshold)
        {
            int count = 0;
            for (int i = 1; i < CAPTCHAHeight - step - 2; i++)
            {
                for (int j = 1; j < CAPTCHAWidth - step; j++)
                {
                    count = 0;

                    if (getSumcolor(i, j) < 255 * 3)     //非白点
                    {
                        //检测白边，左右竖线
                        for (int s = -1; s < step + 2; s++)
                        {
                            if (getSumcolor(i + s, j - 1) > Threshold) count++;
                            if (getSumcolor(i + s, j + step) > Threshold) count++;

                        }
                        //检测白边，上下横线
                        for (int s = -1; s < step; s++)
                        {
                            if (getSumcolor(i - 1, j + s) > Threshold) count++;
                            if (getSumcolor(i + step, j + s) > Threshold) count++;
                        }

                        if (count >= step * 4 + 4)
                        {
                            for (int i1 = i; i1 < i + step; i1++)
                            {
                                for (int j1 = j; j1 < j + step; j1++)
                                {
                                    rband[i, j1] = (byte)255;
                                    gband[i, j1] = (byte)255;
                                    bband[i, j1] = (byte)255;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ClearNoise(int step, int Threshold)
        {
            int count = 0;

            for (int i = 2; i < CAPTCHAHeight - 2; i++)
            {
                for (int j = 2; j < CAPTCHAWidth - 2; j++)
                {
                    count = 0;

                    if (rband[i - 1, j - 1] > Threshold) count++;
                    if (rband[i - 1, j] > Threshold) count++;
                    if (rband[i - 1, j + 1] > Threshold) count++;
                    if (rband[i, j - 1] > Threshold) count++;
                    if (rband[i, j] > Threshold) count++;
                    if (rband[i, j + 1] > Threshold) count++;
                    if (rband[i + 1, j - 1] > Threshold) count++;
                    if (rband[i + 1, j] > Threshold) count++;
                    if (rband[i + 1, j + 1] > Threshold) count++;

                    if (count > 5)
                    {
                        rband[i, j] = (byte)255;
                        gband[i, j] = (byte)255;
                        bband[i, j] = (byte)255;
                    }
                }
            }
        }

        public void Gray()
        {
            double r = 0;
            double g = 0;
            double b = 0;
            int gray = 0;

            for (int i = 0; i < CAPTCHAHeight; i++)
            {
                for (int j = 0; j < CAPTCHAWidth; j++)
                {
                    r = rband[i, j];
                    g = gband[i, j];
                    b = bband[i, j];

                    gray = (int)(r * 0.3 + g * 0.59 + b * 0.11);

                    rband[i, j] = (byte)gray;
                    gband[i, j] = (byte)gray;
                    bband[i, j] = (byte)gray;
                }
            }
        }
    }
}
