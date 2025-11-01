using System.Collections.Generic;

namespace Neurotic.Factory
{

    public class ConvolutionNeuron : INeuralInterfaceInputAggregate
    {
        private Dictionary<IPipe, IBias> inp;
        private IPipe outp;
        private IBias outpBias;

        public ConvolutionNeuron() : this(new Dictionary<IPipe, IBias>(), new IPipe(), new PassthroughBias()) { }
        public ConvolutionNeuron(Dictionary<IPipe, IBias> inp, IPipe outp) : this(inp, outp, new PassthroughBias()) { }
        public ConvolutionNeuron(Dictionary<IPipe, IBias> inp, IPipe outp, IBias outputBias)
        {
            this.inp = inp;
            this.outp = outp;
            this.outpBias = outputBias ?? new PassthroughBias();
        }
        public void Calculate()
        {
            //get values from in pipes, and write result to all outpipes.
            double result = 0;
            foreach (var entry in inp)
            {
                IPipe pipe = entry.Key;
                IBias bias = entry.Value;
                result += bias.Bias(pipe.GetValue(), this);
            }
            // Apply output bias before setting the value
            result = outpBias.Bias(result, this);
            outp.SetValue(result);
        }

        public ICollection<IPipe> getInput()
        {
            return inp.Keys;
        }

        public Dictionary<IPipe, IBias> getInputWithBiases()
        {
            return new Dictionary<IPipe, IBias>(inp);
        }

        public IPipe getOutput()
        {
            return outp;
        }

        public IBias getOutputBias()
        {
            return outpBias;
        }

        public void setInput(ICollection<IPipe> inPipes)
        {
            inp.Clear();
            foreach (var pipe in inPipes)
            {
                inp[pipe] = null; // Will need to set biases later
            }
        }

        public void setInputWithBiases(Dictionary<IPipe, IBias> inputsWithBiases)
        {
            inp = inputsWithBiases;
        }

        public void setOutput(IPipe outPipe)
        {
            outp = outPipe;
        }

        public void setOutputBias(IBias bias)
        {
            outpBias = bias ?? new PassthroughBias();
        }
    }
}