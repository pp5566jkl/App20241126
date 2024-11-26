namespace App20241126
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "圖像文件(JPeg, Gif, Bmp, etc.)|.jpg;*jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog1.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }

            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        pixel = oldBitmap.GetPixel(x, y);
                        int r, g, b, Result = 0;
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        Result = (299 * r + 587 * g + 114 * b) / 1000;
                        newBitmap.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
                    }
                this.pictureBox1.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 設定亮度門檻
                int threshold = 128; // 可以根據需要修改此值

                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        pixel = oldBitmap.GetPixel(x, y);
                        // 將 pixel 轉換為灰階
                        int grayValue = (int)((299 * pixel.R + 587 * pixel.G + 114 * pixel.B) / 1000);

                        // 根據門檻設定二值化像素值
                        if (grayValue >= threshold)
                        {
                            newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255)); // 設定為白色
                        }
                        else
                        {
                            newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0)); // 設定為黑色
                        }
                    }
                }
                this.pictureBox2.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Bitmap Image|*.bmp";
                saveFileDialog1.Title = "?存圖片";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            this.pictureBox3.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }

        struct XYPoint
        {
            public short X;
            public short Y;
        };

        struct LineParameters
        {
            public int Angle;
            public int Distance;
        };
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int Height = this.pictureBox2.Image.Height;
                int Width = this.pictureBox2.Image.Width;
                Bitmap oldBitmap = (Bitmap)this.pictureBox2.Image;
                int EdgeNum = 0;
                XYPoint[] EdgePoint = new XYPoint[Width * Height];
                LineParameters[] Line = new LineParameters[Width * Height];
                for (short x = 0; x < Width; x++)
                    for (short y = 0; y < Height; y++)
                        if (oldBitmap.GetPixel(x, y).G == 255)
                        {
                            EdgePoint[EdgeNum].X = x;
                            EdgePoint[EdgeNum].Y = y;
                            EdgeNum++;
                        }
                int AngleNum = 360;
                int DistNum = (int)Math.Sqrt(Width * Width + Height * Height) * 2;
                int Threshold = Math.Min(Width, Height) / 5;
                int HoughSpaceMax = 0;
                Bitmap newBitmap = new Bitmap(AngleNum, DistNum);
                int pixH;
                double DeltaAngle, DeltaDist;
                double MaxDist, MinDist;
                double Angle, Dist;
                int LineCount;
                int[,] HoughSpace = new int[AngleNum, DistNum];
                MaxDist = Math.Sqrt(Width * Width + Height * Height);
                MinDist = (double)-Width;
                DeltaAngle = Math.PI / AngleNum;
                DeltaDist = (MaxDist - MinDist) / DistNum;

                for (int i = 0; i < AngleNum; i++)
                    for (int j = 0; j < DistNum; j++)
                        HoughSpace[i, j] = 0;
                for (int i = 0; i < EdgeNum; i++)
                    for (int j = 0; j < AngleNum; j++)
                    {
                        Angle = j * DeltaAngle;
                        Dist = EdgePoint[i].X * Math.Cos(Angle) + EdgePoint[i].Y * Math.Sin(Angle);
                        HoughSpace[j, (int)((Dist - MinDist) / DeltaDist)]++;
                    }
                // Vote line in Hough Space
                LineCount = 0;
                for (int i = 0; i < AngleNum; i++)
                    for (int j = 0; j < DistNum; j++)
                    {
                        if (HoughSpace[i, j] > HoughSpaceMax) HoughSpaceMax = HoughSpace[i, j];
                        if (HoughSpace[i, j] >= Threshold)
                        {
                            Line[LineCount].Angle = i;
                            Line[LineCount].Distance = j;
                            LineCount++;
                        }
                    }


                // Draw Hough transform candidates
                for (int x = 0; x < AngleNum; x++)
                    for (int y = 0; y < DistNum; y++)
                    {
                        pixH = 255 - (HoughSpaceMax - HoughSpace[x, y]) * 255 / HoughSpaceMax; // Normalization
                        if (HoughSpace[x, y] > Threshold)
                            newBitmap.SetPixel(x, y, Color.FromArgb(pixH, 0, 0));
                        else
                            newBitmap.SetPixel(x, y, Color.FromArgb(pixH, pixH, pixH));
                    }
                this.pictureBox3.Image = newBitmap;
                // Draw Hough transform lines
                for (int i = 0; i < LineCount & i < Width * Height; i++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int y = (int)((Line[i].Distance * DeltaDist + MinDist - x * Math.Cos(Line[i].Angle * DeltaAngle)) / Math.Sin(Line[i].Angle * DeltaAngle));
                        if (y >= 0 & y < Height)
                        {
                            pixH = oldBitmap.GetPixel(x, y).G;
                            oldBitmap.SetPixel(x, y, Color.FromArgb(pixH ^ 255, pixH, pixH));
                        }
                    }

                    for (int y = 0; y < Height; y++)
                    {
                        int x = (int)((Line[i].Distance * DeltaDist + MinDist - y * Math.Sin(Line[i].Angle * DeltaAngle)) / Math.Cos(Line[i].Angle * DeltaAngle));
                        if (x >= 0 & x < Width)
                        {
                            pixH = oldBitmap.GetPixel(x, y).G;
                            oldBitmap.SetPixel(x, y, Color.FromArgb(pixH ^ 255, pixH, pixH));
                        }
                    }
                }
                this.pictureBox2.Image = oldBitmap;
                this.label1.Text = "Hough transform 完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示");
            }
        }

        

    }
}

