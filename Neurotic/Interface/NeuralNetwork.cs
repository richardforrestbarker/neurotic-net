using System;
using System.Collections.Generic;

namespace Neurotic
{
    public abstract class NeuralNetwork : INeuralInterface
    {
        private readonly ICollection<INeuralInterface> nodes;
        public NeuralNetwork(ICollection<INeuralInterface> layers) => nodes = layers;
        public void Calculate()=>Array.ForEach(nodes.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=>  nodes.First().getInPipes();
        public ICollection<IPipe> getOutPipes()=>  nodes.Last().getOutPipes();
        public void setInPipes(ICollection<IPipe> inPipes)=>  nodes.First().setInPipes(inPipes);
        public void setOutPipes(ICollection<IPipe> outPipes) => nodes.Last().setOutPipes(outPipes);
        
    }
}
