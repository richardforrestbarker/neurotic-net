using System.Collections.Generic;

namespace Neurotic.Factory
{

    public class ConvolutionNeuron : INeuralInterfaceInputAggregate
    {
        private ICollection<IPipe> inp;
        private IPipe outp;

        public ConvolutionNeuron() : this(new List<IPipe>(), new IPipe()) { }
        public ConvolutionNeuron(ICollection<IPipe> inp, IPipe outp)
        {
            this.inp = inp;
            this.outp = outp;
        }
        public void Calculate()
        {
            //get values from in pipes, and write result to all outpipes.
            float result = 0;
            Queue<IPipe> inputs = new Queue<IPipe>(inp);
            IPipe current;
            while (inputs.TryDequeue(out current))
            {
                float weight = current.GetWeightBais().Item1;
                float bias = current.GetWeightBais().Item2;
                result += (current.GetValue() + bias) * weight;
            }
            outp.SetValue(result);
        }

        public ICollection<IPipe> getInput()
        {
            return inp;
        }

        public IPipe getOutput()
        {
            return outp;
        }

        public void setInput(ICollection<IPipe> inPipes)
        {
            inp = inPipes;
        }

        public void setOutput(IPipe outPipe)
        {
            outp = outPipe;
        }
    }
}