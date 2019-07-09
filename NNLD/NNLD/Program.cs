using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using AForge.Video.FFMPEG;
using CursesSharp;
namespace NNLD
{

    class Program
    {
        public static NeuralNetwork net;
       
        static void Main(string[] args)
        {
            string com = "";
            while (true)
            {
                com = Console.ReadLine();
                Command(com);
            }
        }

        public static void Command(string com)
        {
            if (com == "/help")
            {
                Console.WriteLine("----------");
                Console.WriteLine("Commands List:");
                Console.WriteLine("/exit");
                Console.WriteLine("/load NN <file name> : Load Neural Network in floder NN");
                Console.WriteLine("/make image <file name>");
                Console.WriteLine("/make video <file name>");
                Console.WriteLine("----------");

            }
            else if (com == "/exit")
            {
                return;
            }
            else if (com.StartsWith("/make image"))
            {
                string fname = com.Replace("/make image ","");
                if (File.Exists("Images/" + fname))
                {
                    remakeimg(fname);
                } else
                {
                    Console.WriteLine("File \"" + fname + "\" not found");
                }
            }
            else if (com.StartsWith("/make video"))
            {
                string fname = com.Replace("/make video ", "");
                if (File.Exists("Videos/" + fname))
                {
                    VideoFileReader f = new VideoFileReader();
                    f.Open("Videos/" + fname);

                    Console.WriteLine("Vidoe frames Count :" + f.FrameCount);
                    for(int i = 0; i < 10; i++)
                    {
                        //VideoFrameRender vfr = new VideoFrameRender(f.ReadVideoFrame(),i);
                    }
                }
                else
                {
                    Console.WriteLine("File \"" + fname + "\" not found");
                }
            }

            else if (com.StartsWith("/load NN"))
            {
                string fname = com.Replace("/load NN ", "");
                net = NeuralNetwork.Load("NN/" + fname);
            }
            else
            {
                Console.WriteLine("Command not found");
            }

        }

        public static Bitmap ImageBD(Bitmap img)
        {
            //Bitmap newimg = new Bitmap(img.Width, img.Height);

            //for (int x = 0; x < img.Width; x++)
            //    for (int y = 0; y < img.Height; y++)
            //    {
            //        int clr = (img.GetPixel(x, y).R + img.GetPixel(x, y).G + img.GetPixel(x, y).B) / 3;
            //        newimg.SetPixel(x, y, Color.FromArgb(clr, clr, clr));

            //    }

            //return newimg;


            if (img != null) // если изображение в pictureBox1 имеется
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(img.Width, img.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < img.Height; j++)
                    for (int i = 0; i < img.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(img.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                return output;
            }
            return null;
        }

        public static void remakeimg(string fname)
        {
            Console.CursorVisible = false;
            int titlelenght = 0;
            titlelenght =( "---------- " + fname + " rendering ----------").Length;

            Console.WriteLine("---------- " + fname + " rendering ----------");
            Bitmap img = (Bitmap)Image.FromFile("Images/" + fname);
            Console.WriteLine("Image Loaded");
            img = ImageBD((Bitmap)img);
            Console.WriteLine("Image Refactored in White-Black");


            Bitmap newimg = new Bitmap(img.Width, img.Height);
            for (int i = 0; i < newimg.Width; i++)
                for (int u = 0; u < newimg.Height; u++)
                {
                    newimg.SetPixel(i, u, Color.White);
                }

            Graphics g = Graphics.FromImage(newimg);

            Pen p = new Pen(Color.FromArgb(75, 0, 0, 0));
            List<float> inputs = new List<float>();
            List<float> outputs = new List<float>();
           

            for (int x = 0; x < img.Width - 4; x++)
                for (int y = 0; y < img.Height - 4; y++)
                {
                    Console.Write('\r');
                    //Console.Write("[");
                    //for (int i = 0; i < (int)(20 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))); i++) Console.Write("#");
                    //for (int i = 0; i < 20 - (int)(20 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))); i++) Console.Write("-");
                    //Console.Write("]");
                    Console.Write("Rendering : " + (int)(10000 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))) / 100f + "%  00   "); 

                    inputs.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            inputs.Add(img.GetPixel(x + i, y + n).R / 255f);
                        }
                    }
                    outputs = net.think(inputs);
                    if (outputs.Max() != outputs[4])
                    {

                        if (outputs.Max() == outputs[0])
                        {
                            g.DrawLine(p, x + 2, y + 1, x + 2, y + 3);

                        }
                        if (outputs.Max() == outputs[1])
                        {
                            g.DrawLine(p, x + 1, y + 2, x + 3, y + 2);

                        }
                        if (outputs.Max() == outputs[2])
                        {
                            g.DrawLine(p, x + 1, y + 1, x + 3, y + 3);

                        }
                        if (outputs.Max() == outputs[3])
                        {
                            g.DrawLine(p, x + 3, y + 1, x + 1, y + 3);

                        }

                       
                    }

                }
            newimg.Save("New Images/" + fname);
            Console.CursorVisible = true;
            Console.Write('\r');
            Console.WriteLine("Rendering : 100%     ");
            Console.WriteLine("Done.");
            for(int i = 0; i < titlelenght; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }


        public static void remakeimg(Bitmap btm,string fname)
    {

        Bitmap img = btm;
        img = ImageBD((Bitmap)img);

        Bitmap newimg = new Bitmap(img.Width, img.Height);
        for (int i = 0; i < newimg.Width; i++)
            for (int u = 0; u < newimg.Height; u++)
            {
                newimg.SetPixel(i, u, Color.White);
            }

        Graphics g = Graphics.FromImage(newimg);

        Pen p = new Pen(Color.FromArgb(100, 0, 0, 0));
        List<float> inputs = new List<float>();

        for (int x = 0; x < img.Width - 4; x++)
            for (int y = 0; y < img.Height - 4; y++)
            {
                inputs.Clear();
                for (int i = 0; i < 4; i++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        inputs.Add(img.GetPixel(x + i, y + n).R / 255f);
                    }
                }
                List<float> outputs = net.think(inputs);
                if (outputs.Max() != outputs[4])
                {

                    Console.WriteLine(x + " " + y + " ");
                    if (outputs.Max() == outputs[0])
                    {
                        g.DrawLine(p, x + 2, y + 1, x + 2, y + 3);

                    }
                    if (outputs.Max() == outputs[1])
                    {
                        g.DrawLine(p, x + 1, y + 2, x + 3, y + 2);

                    }
                    if (outputs.Max() == outputs[2])
                    {
                        g.DrawLine(p, x + 1, y + 1, x + 3, y + 3);

                    }
                    if (outputs.Max() == outputs[3])
                    {
                        g.DrawLine(p, x + 3, y + 1, x + 1, y + 3);

                    }


                }

            }
        newimg.Save("New Videos/" + fname);
    }
}
}
