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
            layers =layerCount;
            cells = cellCount;
            this.interconectivity = interconectivity;
        }
        public INeuralInterface Construct(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            ConnectionsPerNeuron = (int)Math.Floor(outp.Count * interconectivity);// the number of neurons each cell connects to in the previoyus layer
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

            neurons.Fill(() => new ConvolutionNeuron(inp, outp), cells);
            return neurons;
        }
        private double ConnectionsPerNeuron { get; set; }
        private void ConnectNeurons(INeuralInterface input, INeuralInterface output)
        {
            output.setInPipes(input.getOutPipes());
        }

        private INeuralInterface NextLayer(int l, INeuralInterface previousLayer, ICollection<IPipe> inp, ICollection<IPipe> outp)
        {

            LinkedList<INeuralInterface> neurons;
            var o = new List<IPipe>();
            o.Fill((i) => new IPipe(), 1, (i, p) => p.SetValue(l));
            if (l == 0)
            {  
                neurons = CreateNewCells(inp, o);
            }
            else if (l == layers - 1)
            {
                neurons = CreateNewCells(previousLayer.getOutPipes(), outp);
            }
            else
            {
                o.Fill((i) => new IPipe(), 1, (i, p) => p.SetValue(l));
                neurons = CreateNewCells(previousLayer.getOutPipes(), o);
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
