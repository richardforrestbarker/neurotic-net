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
            ConnectionsPerNeuron = (int)Math.Floor(cells * interconectivity);// the number of neurons each cell connects to in the previoyus layer
            ConvolutionNeuralNetwork net = new ConvolutionNeuralNetwork();
            for (int l = 0; l < layers; l++)
            {
                var layer = new ConvolutionNeuralLayer();
                layer.Fill(() => new ConvolutionNeuron(null, new IPipe[] { new IPipe() }), cells);
                if (l == 0)
                {
                    ConnectPipesToLayersInputs(inp, layer);
                }
                else if (l == layers - 1)
                {
                    ConnectPipesToLayersOutputs(outp, layer);
                }
                else
                {
                    ConnectPipesToLayersInputs(net.ElementAt(l - 1).getOutPipes(), layer);
                }
                net.AddLast(layer);
            }

            return net;
        }



        private void ConnectPipesToLayersInputs(ICollection<IPipe> inp, ConvolutionNeuralLayer layer)
        {

            for (var index = 0; index < layer.Count; index++)
            {
                layer.ElementAt(index).setInPipes(inp.OffsetCenteredWrappedSubset(index, ConnectionsPerNeuron));
            }
        }

        private void ConnectPipesToLayersOutputs(ICollection<IPipe> inp, ConvolutionNeuralLayer layer)
        {

            for (var index = 0; index < layer.Count; index++)
            {
                layer.ElementAt(index).setOutPipes(inp.OffsetCenteredWrappedSubset(index, ConnectionsPerNeuron));
            }
        }

        private int ConnectionsPerNeuron { get; set; }

      
        private class ConvolutionNeuron : Neuron
        {
            public ConvolutionNeuron() : base(new List<IPipe>(), new List<IPipe>()) { }
            public ConvolutionNeuron(ICollection<IPipe> inp, ICollection<IPipe> outp) : base(inp, outp) { }
            public override void Calculate()
            {
                //get values from in pipes, and write result to all outpipes.
                throw new NotImplementedException();
            }
        }
        private class ConvolutionNeuralLayer : NeuralLayer
        {
            public ConvolutionNeuralLayer() : base(new LinkedList<INeuralInterface>()) { }
            public ConvolutionNeuralLayer(ICollection<INeuralInterface> neurons) : base(neurons) { }
        }
        private class ConvolutionNeuralNetwork : NeuralNetwork
        {
            public ConvolutionNeuralNetwork() : base(new LinkedList<INeuralInterface>()) { }
            public ConvolutionNeuralNetwork(ICollection<INeuralInterface> layers) : base(layers) { }
        }
    }
}
