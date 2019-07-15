using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using AForge.Video.FFMPEG;
namespace NNLD
{

    class Program
    {
        public static NeuralNetwork net;

        static void Main(string[] args)
        {
            Console.WriteLine("Neural Network Line Drawer - a program for creating images/video with the selection of contours.\nFor information about the commands write \"/help\"");
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
                Console.WriteLine("/load NN <file name> : Load Neural Network in floder NN");
                Console.WriteLine("/make image <file name> : render image in floder \"Images\"");
                Console.WriteLine("/make video <file name> : render video in floder \"Videos\"");
                Console.WriteLine("/exit : exit from program");
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
                    while (n < -1 || n > d.GetFiles().Length - 1)
                    {
                        Console.WriteLine("Please select correct num.");
                        n = Convert.ToInt32(Console.ReadLine());
                    }
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
                string fname = com.Replace("/make video", "");
                if (fname.Replace(" ", "") == "")
                {
                    Console.WriteLine("---------- Please select video num : ----------");
                    DirectoryInfo d = new DirectoryInfo("Videos");
                    for (int i = 0; i < d.GetFiles().Length; i++)
                    {
                        Console.WriteLine(i + " " + d.GetFiles()[i].Name);
                    }
                    Console.WriteLine("-1 exit");
                    Console.WriteLine("-----------------------------------------------");

                    int n = Convert.ToInt32(Console.ReadLine());
                    while (n < -1 || n > d.GetFiles().Length - 1)
                    {
                        Console.WriteLine("Please select correct num.");
                        n = Convert.ToInt32(Console.ReadLine());
                    }
                    if (n == -1) return;
                    fname = d.GetFiles()[n].Name;
                }

                if (fname[0] == ' ') fname = fname.Remove(0, 1);

                if (File.Exists("Videos/" + fname))
                {
                    Console.CursorVisible = false;

                    Console.WriteLine("---------- " + fname + " rendering ----------");

                    VideoFileWriter w = new VideoFileWriter();

                    w.Open("New Videos/" + fname.Replace(".mp4", "") + ".avi", 1280, 720, 30, VideoCodec.MPEG4);

                    VideoFileReader f = new VideoFileReader();
                    f.Open("Videos/" + fname);
                    Console.WriteLine("Video loaded");
                    Console.WriteLine("Video frames Count :" + f.FrameCount);
                    long all = f.Width * f.Height * f.FrameCount;

                    Console.WriteLine("Rendering video : 0%      ");
                    Console.Write("Rendering frame 0/" + (f.FrameCount - 1) + " : 0%      ");

                    for (int i = 0; i < f.FrameCount; i++)
                    {
                        Bitmap b = f.ReadVideoFrame();
                        b = remakeimg(b, i, f.FrameCount);
                        w.WriteVideoFrame(b);
                        b.Dispose();
                    }

                    f.Close();
                    w.Close();

                    Console.CursorVisible = true;
                }
                else
                {
                    Console.WriteLine("File \"" + fname + "\" not found");
                }
            }
            else if (com.StartsWith("/load NN"))
            {
                string fname = com.Replace("/load NN", "");
                if (fname.Replace(" ", "") == "")
                {
                    Console.WriteLine("---------- Please select Neural Network num : ----------");
                    DirectoryInfo d = new DirectoryInfo("NN");
                    for (int i = 0; i < d.GetFiles().Length; i++)
                    {
                        Console.WriteLine(i + " " + d.GetFiles()[i].Name);
                    }
                    Console.WriteLine("-1 exit");
                    Console.WriteLine("--------------------------------------------------------");

                    int n = Convert.ToInt32(Console.ReadLine());
                    while (n < -1 || n > d.GetFiles().Length - 1)
                    {
                        Console.WriteLine("Please select correct num.");
                        n = Convert.ToInt32(Console.ReadLine());
                    }

                    if (n == -1) return;
                    fname = d.GetFiles()[n].Name;
                }

                if (fname[0] == ' ') fname = fname.Remove(0, 1);

                if (File.Exists("NN/" + fname))
                {
                    net = NeuralNetwork.Load("NN/" + fname);
                }
                else
                {
                    Console.WriteLine("File \"" + fname + "\" not found");
                }

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

            string lt1 = "";
            for (int x = 0; x < img.Width - 4; x++)
                for (int y = 0; y < img.Height - 4; y++)
                {
                    if (!lt1.Equals("Rendering : " + (int)((int)(10000 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))) / 100f) + "%      "))
                    {
                        Console.Write('\r');
                        lt1 = "Rendering : "  + (int)((10000 * (((img.Height - 4) * x + y) / (float)((img.Width - 4) * (img.Height - 4)))) / 100f) + "%      ";
                        Console.Write(lt1);
                    }




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
            Console.WriteLine("Rendering : 100%");
            Console.WriteLine("Done.");
            for (int i = 0; i < titlelenght; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }


        public static Bitmap remakeimg(Bitmap btm, int fn, long fc)
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

            string lt1= "",lt2 = "";

            for (int x = 0; x < btm.Width - 4; x++)
                for (int y = 0; y < btm.Height - 4; y++)
                {

                    if (!lt1.Equals("Rendering Video : " + ((float)(10000 * (double)((fn * (btm.Width - 4) * (btm.Height - 4) + x * (btm.Height - 4)) / (double)((fc - 1) * (btm.Width - 4) * (btm.Height - 4)))) / 100f).ToString("0.0") + "%      ") ||
                       !lt2.Equals("Rendering frame " + fn + "/" + (fc - 1) + " : " + (int)((int)(10000 * (((btm.Height - 4) * x + y) / (float)((btm.Width - 4) * (btm.Height - 4)))) / 100f) + "%      "))
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);

                        Console.Write('\r');
                        lt1 = "Rendering Video : " + ((float)(10000 * (double)((fn * (btm.Width - 4) * (btm.Height - 4) + x * (btm.Height - 4)) / (double)((fc - 1) * (btm.Width - 4) * (btm.Height - 4)))) / 100f).ToString("0.0") + "%      ";
                        Console.WriteLine(lt1);

                        Console.Write('\r');
                        lt2 = "Rendering frame " + fn + "/" + (fc - 1) + " : " + (int)((int)(10000 * (((btm.Height - 4) * x + y) / (float)((btm.Width - 4) * (btm.Height - 4)))) / 100f) + "%      ";
                        Console.Write(lt2);
                        }

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

                        //Console.WriteLine(x + " " + y + " ");
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
