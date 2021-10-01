using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic.Factory
{
    public class ConvolutionNeuralLayer : LinkedList<ConvolutionNeuron>, INeuralInterfaceIOAggregate
    {

        public ConvolutionNeuralLayer() : base() { }
        public ConvolutionNeuralLayer(ICollection<ConvolutionNeuron> neurons) : base(neurons) { }

        public void Calculate()
        {
            foreach (INeuralCalculator neuron in this)
            {
                neuron.Calculate();
            }
        }

        public ICollection<IPipe> getInput()
        {
            return this.SelectMany(neuron => neuron.getInput()).Distinct().ToList();
        }

        public ICollection<IPipe> getOutput()
        {
            return this.Select(neuron => neuron.getOutput()).Distinct().ToList();
        }

        public void setInput(ICollection<IPipe> inPipes)
        {
            foreach (ConvolutionNeuron neuron in this)
            {
                neuron.setInput(inPipes);
            }
        }

        public void setOutput(ICollection<IPipe> outPipe)
        {
            if (outPipe.Count != this.Count) throw new ArgumentOutOfRangeException();
            Queue<IPipe> outs = new Queue<IPipe>(outPipe);
            foreach (ConvolutionNeuron neuron in this)
            {
                IPipe pipe = outs.Dequeue();
                neuron.setOutput(pipe);
                outs.Enqueue(pipe);
            }
        }
    }
}