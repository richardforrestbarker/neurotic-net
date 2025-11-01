using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neurotic;

namespace Neurotic.Factory
{
    public interface INeuralInterfaceFactory
    {
        INeuralCalculator Construct(ICollection<IPipe> inp, ICollection<IPipe> outp);
    }

    public partial class ConvolutionNetworkFactory : INeuralInterfaceFactory
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
        public ConvolutionNeuralNetwork Construct(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            ConnectionsPerNeuron = (int)Math.Floor(cells * interconectivity);// the number of neurons each cell connects to in the prevoyus layer
            ConvolutionNeuralNetwork net = new ConvolutionNeuralNetwork();
            for (int l = 0; l < layers; l++)
            {
                var layer = new ConvolutionNeuralLayer();
                var inputs = new Dictionary<IPipe, IBias>();
                inputs[new IPipe()] = new PassthroughBias();
                layer.Fill(() => new ConvolutionNeuron(inputs, new IPipe()), cells);
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
                    ConnectPipesToLayersInputs(net.ElementAt(l - 1).getOutput(), layer);
                }
                net.AddLast(layer);
            }

            return net;
        }



        private void ConnectPipesToLayersInputs(ICollection<IPipe> inp, ConvolutionNeuralLayer layer)
        {

            for (var index = 0; index < layer.Count; index++)
            {
                var inputPipes = inp.OffsetCenteredWrappedSubset(index, ConnectionsPerNeuron);
                var inputDict = new Dictionary<IPipe, IBias>();
                foreach (var pipe in inputPipes)
                {
                    inputDict[pipe] = new PassthroughBias(); // Default to passthrough
                }
                layer.ElementAt(index).setInputWithBiases(inputDict);
            }
        }

        private void ConnectPipesToLayersOutputs(ICollection<IPipe> inp, ConvolutionNeuralLayer layer)
        {

            for (var index = 0; index < layer.Count; index++)
            {
                layer.ElementAt(index).setOutput(inp.OffsetCenteredWrappedSubset(index, ConnectionsPerNeuron).First());
            }
        }

        INeuralCalculator INeuralInterfaceFactory.Construct(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            return Construct(inp, outp);
        }

        private int ConnectionsPerNeuron { get; set; }
    }
}
