using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neurotic;

namespace Neurotic.Factory
{
    public interface INeuralInterfaceFactory
    {
        INeuralInterface Construct(ICollection<IPipe> inp, ICollection<IPipe> outp);
    }

    public class ConvolutionNetworkFactory : INeuralInterfaceFactory
    {
        private int layers, cells;
        private double interconectivity;
        public ConvolutionNetworkFactory(double interconectivity, int layerCount, int cellCount)
        {
            if (interconectivity > 1 || interconectivity < 0) throw new ArgumentOutOfRangeException(nameof(interconectivity), "Should be in range 0 to 1 inclusive.");
            if (layerCount <= 0) throw new ArgumentOutOfRangeException(nameof(layerCount), "Should be greater than 0.");
            if (cellCount <= 0) throw new ArgumentOutOfRangeException(nameof(cellCount), "Should be greater than 0.");
            layers = layerCount;
            cells = cellCount;
            this.interconectivity = interconectivity;
        }
        public INeuralInterface Construct(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            ConvolutionNeuralNetwork net;
            LinkedList<INeuralInterface> layersMade = new LinkedList<INeuralInterface>();
            for (int l = 0; l < layers; l++)
            {
                var nextLayer = NextLayer(l, layersMade.Last?.Value, inp, outp);
                layersMade.AddLast(nextLayer);
            }
            return net = new ConvolutionNeuralNetwork(layersMade);
        }

        private LinkedList<INeuralInterface> CreateNewCells(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            LinkedList<INeuralInterface> neurons = new LinkedList<INeuralInterface>();
            var connectionsPerNeuron = Math.Floor(outp.Count * interconectivity);// the number of neurons each cell connects to in the previoyus layer
            for (int c = 0; c < cells; c++)
            {
                neurons.AddLast(new ConvolutionNeuron(null, null));
            }
            for (int c = 0; c < cells; c++)
            {
                ICollection<IPipe> i = inp, o = outp;
                var n = neurons.ElementAt(c);
                //wrap connections arround
                if (c == cells - 1)
                {
                    //last cell in the layer
                    var start = c - (int)Math.Floor(connectionsPerNeuron / 2);
                    var end = 0 + (int)Math.Floor(connectionsPerNeuron / 2);
                    i = inp.Skip(start).Concat(i.Take(end)).ToArray();
                }
                else if (c == 0)
                {
                    var newI = inp.Take((int)Math.Floor(connectionsPerNeuron / 2)).Concat(inp.Skip(cells - (int)Math.Floor(connectionsPerNeuron / 2)));
                    i = newI.ToArray();
                    o = new List<IPipe>();
                    o.Add(new IPipe());
                }
                else
                {
                    var start = c - (int)Math.Floor(connectionsPerNeuron / 2);
                    var newI = inp.Skip(start);
                    var end = c + (int)Math.Floor(connectionsPerNeuron / 2);
                    if (end > cells)
                    {
                        // going past the end, wrap around
                        newI = newI.Concat(inp.Take(end - cells));
                    }
                    else
                    {
                        newI = newI.Take((int)connectionsPerNeuron);
                    }
                    i = newI.ToArray();
                    o = new List<IPipe>();
                    o.Add(new IPipe());
                }
                n.setInPipes(i);
                n.setOutPipes(o);
                neurons.AddLast(n);
            }
            return neurons;
        }

        private INeuralInterface NextLayer(int l, INeuralInterface previousLayer, ICollection<IPipe> inp, ICollection<IPipe> outp)
        {

            LinkedList<INeuralInterface> neurons;
            if (l == 0)
            {
                neurons = CreateNewCells(inp, new List<IPipe>());
            }
            else if (l == layers - 1)
            {
                neurons = CreateNewCells(previousLayer.getOutPipes(), outp);
            }
            else
            {
                neurons = CreateNewCells(previousLayer.getOutPipes(), new List<IPipe>());
            }

            return new ConvolutionNeuralLayer(neurons);
        }

        private class ConvolutionNeuron : Neuron
        {

            public ConvolutionNeuron(ICollection<IPipe> inp, ICollection<IPipe> outp) : base(inp, outp) { }
            public override void Calculate()
            {
                //get values from in pipes, and write result to all outpipes.
                throw new NotImplementedException();
            }
        }
        private class ConvolutionNeuralLayer : NeuralLayer
        {
            public ConvolutionNeuralLayer(ICollection<INeuralInterface> neurons) : base(neurons) { }
        }
        private class ConvolutionNeuralNetwork : NeuralNetwork
        {
            public ConvolutionNeuralNetwork(ICollection<INeuralInterface> layers) : base(layers) { }
        }
    }
}
