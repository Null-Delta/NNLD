using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNLD
{
    [Serializable]
    class Neuron
    {
        //выход и дельта нейрона
        public float value, delta;

        public Neuron()
        {
            value = delta = 0;
        }

        public void restart()
        {
            value = delta = 0;
        }
    }
}
