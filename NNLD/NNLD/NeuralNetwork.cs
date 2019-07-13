using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NNLD
{
    [Serializable]
    public class NeuralNetwork
    {
        float spd = 0.1f;

        static Random rnd = new Random();

        List<List<Neuron>> neurons = new List<List<Neuron>> { };


        List<List<List<float>>> widths = new List<List<List<float>>> { };

        List<float> errors = new List<float> { };
        public List<float> graph = new List<float> { };

        //создание нейронной сети
        public NeuralNetwork(List<int> lays)
        {
            //Создание нейронов
            for (int i = 0; i < lays.Count; i++)
            {
                neurons.Add(new List<Neuron> { });
                for (int n = 0; n < lays[i]; n++)
                {
                    neurons[i].Add(new Neuron());
                }
            }

            for (int i = 0; i < neurons.Count - 1; i++)
            {
                widths.Add(new List<List<float>> { });
                for (int n = 0; n < neurons[i].Count; n++)
                {
                    widths[i].Add(new List<float> { });
                    for (int u = 0; u < neurons[i + 1].Count; u++)
                    {
                        widths[i][n].Add((float)(rnd.NextDouble() * 2.0 - 1));
                    }
                }
            }
        }

        public void learn(List<float> inputs, int corres)
        {
            think(inputs);
            
            //нужно для графика, не влияет на обучение
            float error = 0;

            //вычисление дельт для нейронов выходного слоя
            for (int i = 0; i < neurons[neurons.Count - 1].Count; i++)
            {
                if (i == corres)
                {
                    error += (float)Math.Pow((1 - neurons[neurons.Count - 1][i].value),2);
                    neurons[neurons.Count - 1][i].delta = 1 - neurons[neurons.Count - 1][i].value;
                }
                else
                {
                    error += (float)Math.Pow((0 - neurons[neurons.Count - 1][i].value), 2);
                    neurons[neurons.Count - 1][i].delta = 0 - neurons[neurons.Count - 1][i].value;
                }
            }            
                errors.Add(error);
           
            if(errors.Count == 100)
            {
                float sum = 0;
                for(int i = 0; i < errors.Count; i++){
                    sum += errors[i];
                }
                graph.Add(sum);
                errors.Clear();
            }

            //
            for (int i = neurons.Count - 1; i >= 1; i--)
            {
                //прохождение по предыдущему слою нейронов
                for (int n = 0; n < neurons[i - 1].Count; n++)
                {
                    //прохождение по текущему слою нейронов
                    for (int u = 0; u < neurons[i].Count; u++)
                    {
                        //вычисление дельты нейрона
                        neurons[i - 1][n].delta += neurons[i][u].delta * widths[(i - 1)][n][u];
                    }
                }
            }

            for (int i = 0; i < neurons.Count - 1; i++)
            {
                //прохождение по предыдущему слою нейронов
                for (int n = 0; n < neurons[i].Count; n++)
                {
                    //прохождение по текущему слою нейронов
                    for (int u = 0; u < neurons[i + 1].Count; u++)
                    {
                        widths[i][n][u] += spd * neurons[i + 1][u].delta * (neurons[i + 1][u].value * (1 - neurons[i + 1][u].value)) * neurons[i][n].value;
                    }
                }
            }
        }

        public List<float> think(List<float> inputs)
        {
            restart();
            List<float> outputs = new List<float> { };
            //занесение входных данных во входной слой
            for (int i = 0; i < neurons[0].Count; i++)
            {
                neurons[0][i].value = inputs[i];
            }
            //прохождение по всем слоям сети
            for (int i = 0; i < neurons.Count - 1; i++)
            {
                //прохождение по текущему слою нейронов
                for (int n = 0; n < neurons[i + 1].Count; n++)
                {
                    //прохождение по предыдущему слою нейронов
                    for (int u = 0; u < neurons[i].Count; u++)
                    {
                        //нахождение вывода нейрона
                        neurons[i + 1][n].value += neurons[i][u].value * widths[i][u][n];
                    }
                    //прогон вывода через функцию активации
                    neurons[i + 1][n].value = activate(neurons[i + 1][n].value);
                }
            }
            //занесние выходных значений в список
            for (int i = 0; i < neurons[neurons.Count - 1].Count; i++)
            {
                outputs.Add(neurons[neurons.Count - 1][i].value);
            }
            //возвращение выходов
            for(int i = 0; i < outputs.Count; i++)
            {
                //Console.WriteLine(outputs[i]);
            }
            //Console.WriteLine("*****");

            return outputs;
        }
        //обнуляем выходы и дельты всех нейронов
        public void restart()
        {
            for (int i = 0; i < neurons.Count; i++)
            {
                for (int n = 0; n < neurons[i].Count; n++)
                {
                    neurons[i][n].restart();
                }
            }
        }
        //сохранение нейронной сети в файл
        public void Save(string way)
        {
            Stream SaveFileStream = File.Create(way);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, this);
            SaveFileStream.Close();
            Console.WriteLine("File Net Saved.");
        }
        //загрузка нейронной сети из файла
        public static NeuralNetwork Load(string way)
        {
            NeuralNetwork net = null;

            if (File.Exists(way))
            {
                Stream openFileStream = File.OpenRead(way);
                BinaryFormatter deserializer = new BinaryFormatter();
                net = (NeuralNetwork)deserializer.Deserialize(openFileStream);
                openFileStream.Close();

                Console.WriteLine("Network is load");
            }

            return net;
        }
        //функция активации
        private float activate(float input)
        {
            return (float)(1.0 / (1.0 + Math.Exp(-input * 0.005)));
        }
    }
}
