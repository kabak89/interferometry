﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rab1.Forms
{
    public partial class Graphic : Form
    {
        private Int64[] buf_gl;
        private Int64[] buf1_gl;
        private int w;
        private int h;
        private int x0 = 40;
        private int hh = 256; 

        public Graphic(Pi_Class1.ZArrayDescriptor ZZ, int x, int y)
        {
            InitializeComponent();

            int w1 = ZZ.width;             w = w1;
            int h1 = ZZ.height;            h = h1;
            Int64[] buf = new Int64[w1];
            buf_gl = new Int64[w1];
            buf1_gl = new Int64[w1];


            for (int i = 0; i < w1; i++) { buf[i] = ZZ.array[i, y]; buf_gl[i] = buf[i]; }
            //pc1
            pc1.BackColor = Color.White;
            //pc1.Location = new System.Drawing.Point(0, 8);
            pc1.Size = new Size(w1 + 16, hh + 32);
            // pc1.SizeMode = PictureBoxSizeMode.StretchImage;
            pc1.BorderStyle = BorderStyle.Fixed3D;
            Bitmap btmBack = new Bitmap(w1 + 16, hh + 32);      //изображение
            Bitmap btmFront = new Bitmap(w1 + 16, hh + 32);     //фон
            Graphics grBack = Graphics.FromImage(btmBack);
            //Graphics grFront = Graphics.FromImage(btmFront);  //лучше объявить заранее глобально.
            pc1.Image = btmFront;
            pc1.BackgroundImage = btmBack;

            Graph(buf, w1, x, grBack);

            for (int i = 0; i < h1; i++) { buf[i] = ZZ.array[x, i]; buf1_gl[i] = buf[i]; }
            //pictureBox1
            pictureBox1.BackColor = Color.White;
            pictureBox1.Size = new Size(w1 + 16, hh + 32);
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            btmBack = new Bitmap(w1 + 16, hh + 32);      //изображение
            btmFront = new Bitmap(w1 + 16, hh + 32);     //фон
            grBack = Graphics.FromImage(btmBack);
            //Graphics grFront = Graphics.FromImage(btmFront);  //лучше объявить заранее глобально.
            pictureBox1.Image = btmFront;
            pictureBox1.BackgroundImage = btmBack;
   
            Graph(buf, h1, y, grBack);
        }
        
        
        //  Перегруженный конструктор для графика таблицы
   
        public Graphic(int w1, int x, Int64[] buf, int[] buf1)
                {
                    InitializeComponent();

   
                    int hh = 256;   //260;
                    w = w1;
                    buf_gl  = new Int64[w1];
                    buf1_gl = new Int64[w1];
                    Int64 maxx = buf[0], minx = buf[0], b;
                    for (int i = 0; i < w1; i++) { b = buf[i]; if (b < minx) minx = b; if (b > maxx) maxx = b; buf_gl[i] = b; } //buf_gl[i] = b; }
                    for (int i = 0; i < w1; i++) { buf[i] = (buf[i] - minx) * hh / (maxx - minx); }


                    Font font = new Font("Courier", 12); 
                    //StringFormat drawFormat = new StringFormat(StringFormatFlags.NoClip); //   .  NoClip);
                    // string sx = " minx =  " + minx + "  maxx =  " + maxx;

       
                    pc1.BackColor = Color.White;
                    //pc1.Location = new System.Drawing.Point(0, 8);
                    pc1.Size = new Size(w1 + 16, hh + 32);
                    pc1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pc1.BorderStyle = BorderStyle.Fixed3D;
                    Bitmap btmBack = new Bitmap(w1 + 16, hh + 64);      //изображение
                    Bitmap btmFront = new Bitmap(w1 + 16, hh + 64);     //фон
                    Graphics grBack = Graphics.FromImage(btmBack);
                    //Graphics grFront = Graphics.FromImage(btmFront);  //лучше объявить заранее глобально.
                    pc1.Image = btmFront;
                    pc1.BackgroundImage = btmBack;

                

                    Graph(buf, w1, x, grBack);


          //-------------------------------------------------------------------------------------------------------  Истинные диагонали
            Pen p4 = new Pen(Color.Blue, 1);
            for (int i = 0; i < w1 - 1; i++)
            {
                //string sx1 = " i =  " + i + "  buf1[i] =  " + buf1[i];
                buf1_gl[i] = buf1[i];
                if (buf1[i] >= 0)
                {
                   grBack.DrawLine(p4, i + x0, 8, i + x0, hh + 8);
                }
            }
           // pc1.Refresh();

           //         Controls.Add(pc1);
                }


    
        private void pc1_MouseMove(object sender, MouseEventArgs e)
        {
                       
                int xPosition = e.X - x0;
                int с_buf1 = 0;
                int с_buf2 = 0; 
              
            if (xPosition >= 0 && xPosition < w)
              {
                с_buf1   = (int) buf_gl[xPosition];
                с_buf2   = (int)buf1_gl[xPosition];
                label4.Text = Convert.ToString(xPosition);
                label5.Text = Convert.ToString(с_buf1);
                label11.Text = Convert.ToString(с_buf2);
              }
        }
        private void pc2_MouseMove(object sender, MouseEventArgs e)
        {

            int xPosition = e.X - x0;
            int с_buf1 = 0;

            if (xPosition >= 0 && xPosition < h)
            {
                с_buf1 = (int)buf1_gl[xPosition];
                label8.Text = Convert.ToString(xPosition);
                label10.Text = Convert.ToString(с_buf1);
            }
        }

        private void Graph(Int64[] buf, int w1, int x, Graphics grBack)
        {
           
            Int64 maxx = buf[0], minx = buf[0], b;
            for (int i = 0; i < w1; i++) { b = buf[i]; if (b < minx) minx = b; if (b > maxx) maxx = b;  }
            for (int i = 0; i < w1; i++) { buf[i] = (buf[i] - minx) * hh / (maxx - minx); }

            Font font = new Font("Arial", 12); //, GraphicsUnit.Pixel)Regular;, FontStyle.Regular
            //StringFormat drawFormat = new StringFormat(StringFormatFlags.NoClip); //   .  NoClip);
          
            Pen p1 = new Pen(Color.Black, 1);
            Pen p2 = new Pen(Color.Red, 1);
            Pen p3 = new Pen(Color.Green, 1);
            // ------------------------------------------------------------------------------------------------------------График 
            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            //  -----------------------------------------------------------------------------------------------------Ось x
            grBack.DrawLine(p1, x0, hh, w1 + x0, hh);
            for (int i = 0; i < w1; i += 8) grBack.DrawLine(p1, i + x0, hh, i + x0, hh + 8);
            for (int i = 0; i <= w1; i += 64)
            {
              
                string sx = i.ToString();
                grBack.DrawString(sx, drawFont, drawBrush, i+x0, hh + 11); //, drawFormat);
              
            }

            //  -----------------------------------------------------------------------------------------------------Ось y
            grBack.DrawLine(p1, x0, 8, x0, hh + 8);
            for (int i = 8; i < hh + 8; i += 8) grBack.DrawLine(p1, x0, i, x0 + 4, i);      

            double k = (hh) / 32;
            double kx = (maxx - minx) / k;
            double nf = minx;
            long kf;
            for (int i = 0; i <= hh; i += 32)
            {
                kf = (long)nf;
                string sx = kf.ToString();
                grBack.DrawString(sx, drawFont, drawBrush, 2, hh - i); //, drawFormat);
                nf += kx;
                grBack.DrawLine(p1, x0, i, x0 + w1, i);
            }


            grBack.DrawLine(p3, x + x0, 0, x + x0, hh + 9);                                                                     // Значение координаты


            for (int i = 0; i < w1 - 1; i++) grBack.DrawLine(p2, i + x0, hh - buf[i], i + 1 + x0, hh - buf[i + 1]);
          
        }

        
        }
    }

