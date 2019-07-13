using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
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
                Console.WriteLine("/make image <file name> : render image in floder \"Images\"");
                Console.WriteLine("/make video <file name> : render video in floder \"Videos\"");
                Console.WriteLine("----------");

            }
            else if (com == "/exit")
            {
                Environment.Exit(0);
            }
            else if (com.StartsWith("/make image"))
            {
                string fname = com.Replace("/make image", "");
                if(fname.Replace(" ","") == "")
                {
                    Console.WriteLine("---------- Please select image num : ----------");
                    DirectoryInfo d = new DirectoryInfo("Images");
                    for(int i = 0; i < d.GetFiles().Length; i++)
                    {
                        Console.WriteLine(i + " " + d.GetFiles()[i].Name);
                    }
                    Console.WriteLine("-1 exit");
                    Console.WriteLine("-----------------------------------------------");

                    int n = Convert.ToInt32(Console.ReadLine());
                    while (n < -1 || n > d.GetFiles().Length - 1) Console.WriteLine("Please select correct num.");

                    if (n == -1) return;
                    fname = d.GetFiles()[n].Name;
                }

                if (fname[0] == ' ') fname = fname.Remove(0, 1);

                if (File.Exists("Images/" + fname))
                {
                    remakeimg(fname);
                }
                else
                {
                    Console.WriteLine("File \"" + fname + "\" not found");
                }
            }
            else if (com.StartsWith("/make video"))
            {
                string fname = com.Replace("/make video ", "");
                if (File.Exists("Videos/" + fname))
                {
                    Console.WriteLine("---------- " + fname + " rendering ----------");

                    //Conversion conversion = new Conversion();

                    
                    //VideoFileWriter w = new VideoFileWriter();

                    //w.Open(fname.Replace(".mp4", "") + ".avi", 1280, 720, 30, VideoCodec.MPEG4);

                    //VideoFileReader f = new VideoFileReader();
                    //f.Open("Videos/" + fname);
                    //Console.WriteLine("Video loaded");
                    //Console.WriteLine("Vidoe frames Count :" + f.FrameCount);
                    //long all = f.Width * f.Height * f.FrameCount;

                    //for (int i = 0; i < f.FrameCount; i++)
                    //{
                    //    Bitmap b = f.ReadVideoFrame();
                    //    b = remakeimg(b);
                    //    w.WriteVideoFrame(b);
                    //    b.Dispose();

                    //    Console.Write('\r');

                    //    string progressBar = "[";
                    //    for (int n = 0; n < (int)(20 * (i / (float)f.FrameCount)); n++) progressBar += "#";

                    //    for (int n = 0; n < (int)(20 - 20 * (i / (float)f.FrameCount)); n++) progressBar += "-";

                    //    progressBar += "]";
                    //    Console.Write("Rendering : " + progressBar + " " + (int)(10000 * (i / (float)f.FrameCount)) / 100f + "%      ");
                    //}

                    //f.Close();
                    //w.Close();
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

        public static void remakeimg(string fname)
        {
            Console.CursorVisible = false;
            int titlelenght = 0;
            titlelenght = ("---------- " + fname + " rendering ----------").Length;

            Console.WriteLine("---------- " + fname + " rendering ----------");
            Bitmap img = (Bitmap)Bitmap.FromFile("Images/" + fname);
            Console.WriteLine("Image Loaded");
            //img = ImageBD((Bitmap)img);


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
                    string progressBar = "[";
                    for (int i = 0; i < (int)(20 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))); i++) progressBar += "#";

                    for (int i = 0; i < (int)(20 - 20 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))); i++) progressBar += "-";

                    progressBar += "]";
                    Console.Write("Rendering : " + progressBar + " " + (int)(10000 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))) / 100f + "%      ");

                    inputs.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            inputs.Add(((img.GetPixel(x + i, y + n).R + img.GetPixel(x + i,y + n).G + img.GetPixel(x + i,y + n).B) / 3f) / 255f);
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
            Console.WriteLine("Rendering : [####################] 100%");
            Console.WriteLine("Done.");
            for (int i = 0; i < titlelenght; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }


        public static Bitmap remakeimg(Bitmap btm)
        {
            Bitmap newimg = new Bitmap(btm.Width, btm.Height);
            for (int i = 0; i < newimg.Width; i++)
                for (int u = 0; u < newimg.Height; u++)
                {
                    newimg.SetPixel(i, u, Color.White);
                }

            Graphics g = Graphics.FromImage(newimg);

            Pen p = new Pen(Color.FromArgb(100, 0, 0, 0));
            List<float> inputs = new List<float>();

            for (int x = 0; x < btm.Width - 4; x++)
                for (int y = 0; y < btm.Height - 4; y++)
                {
                    inputs.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            inputs.Add(((btm.GetPixel(x + i, y + n).R + btm.GetPixel(x + i, y + n).G + btm.GetPixel(x + i, y + n).B) / 3f) / 255f);
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
            return newimg;
        }
    }
}
