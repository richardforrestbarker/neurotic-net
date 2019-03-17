using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public abstract class NeuralLayer : INeuralInterface
    {
        public readonly ICollection<INeuralInterface> neurons;
        public NeuralLayer(ICollection<INeuralInterface> neurons)=> this.neurons = neurons;
        public void Calculate() => Array.ForEach(neurons.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=>neurons.SelectMany((n) => n.getInPipes()).Distinct().ToArray();
        public ICollection<IPipe> getOutPipes()=>neurons.SelectMany((n) => n.getOutPipes()).Distinct().ToArray();
        public void setInPipes(ICollection<IPipe> inPipes)=>Array.ForEach(neurons.ToArray(), n => n.setInPipes(inPipes));
        public void setOutPipes(ICollection<IPipe> outPipes)=> Array.ForEach(neurons.ToArray(), n => n.setOutPipes(outPipes));
    }
}
