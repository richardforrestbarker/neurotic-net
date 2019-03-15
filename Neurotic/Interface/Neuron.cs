using System.Collections.Generic;

namespace Neurotic
{
    public abstract class Neuron : INeuralInterface
    {
        private ICollection<IPipe> inputs, outputs;

        public Neuron(ICollection<IPipe> inp, ICollection<IPipe> outp)
        {
            setInPipes(inp);
            setOutPipes(outp);
        }

        public abstract void Calculate();
        public ICollection<IPipe> getInPipes() => inputs;
        public ICollection<IPipe> getOutPipes() => outputs;
        public void setInPipes(ICollection<IPipe> inPipes) => inputs = inPipes;
        public void setOutPipes(ICollection<IPipe> outPipes) => outputs = outPipes;

    }
}
