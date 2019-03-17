using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public abstract class NeuralNetwork : INeuralInterface
    {
        public readonly ICollection<INeuralInterface> layers;
        public NeuralNetwork(ICollection<INeuralInterface> layers) => this.layers = layers;
        public void Calculate()=>Array.ForEach(layers.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=>  layers.First().getInPipes();
        public ICollection<IPipe> getOutPipes()=>  layers.Last().getOutPipes();
        public void setInPipes(ICollection<IPipe> inPipes)=>  layers.First().setInPipes(inPipes);
        public void setOutPipes(ICollection<IPipe> outPipes) => layers.Last().setOutPipes(outPipes);
        
    }
}
