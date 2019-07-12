using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Console.WriteLine("/load NN <file name> : Load Neural Network");
                Console.WriteLine("/make images");
                Console.WriteLine("/make videos");
                Console.WriteLine("/make image <file name>");
                Console.WriteLine("/make videos <file name>");
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
            Bitmap newimg = new Bitmap(img.Width, img.Height);

            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    int clr = (img.GetPixel(x, y).R + img.GetPixel(x, y).G + img.GetPixel(x, y).B) / 3;
                    newimg.SetPixel(x, y, Color.FromArgb(clr, clr, clr));

                }

            return newimg;
        }

        public static void remakeimg(string fname)
        {
            
            Bitmap img = (Bitmap)Image.FromFile("Images/" + fname);
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
            newimg.Save("New Images/" + fname);
        }
    }
}
