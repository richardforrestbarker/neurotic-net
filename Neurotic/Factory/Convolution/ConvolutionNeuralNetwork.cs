using System.Collections.Generic;
using System.Linq;

namespace Neurotic.Factory
{

    public class ConvolutionNeuralNetwork : LinkedList<ConvolutionNeuralLayer>, INeuralInterfaceIOAggregate
    {
        public ConvolutionNeuralNetwork() : base(new LinkedList<ConvolutionNeuralLayer>()) { }
        public ConvolutionNeuralNetwork(ICollection<ConvolutionNeuralLayer> layers) : base(layers) { }

        public void Calculate()
        {
            foreach (ConvolutionNeuralLayer layer in this)
            {
                layer.Calculate();
            }
        }

        public ICollection<IPipe> getInput()
        {
            return this.First().getInput();
        }

        public ICollection<IPipe> getOutput()
        {
            return this.Last().getOutput();
        }

        public void setInput(ICollection<IPipe> inPipes)
        {
            this.First().setInput(inPipes);
        }

        public void setOutput(ICollection<IPipe> outPipe)
        {
            this.Last().setOutput(outPipe);
        }
    }
}