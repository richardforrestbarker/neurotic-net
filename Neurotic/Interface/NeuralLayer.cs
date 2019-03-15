using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public abstract class NeuralLayer : INeuralInterface
    {
        private readonly ICollection<INeuralInterface> nodes;
        public NeuralLayer(ICollection<INeuralInterface> layers)=> nodes = layers;
        public void Calculate() => Array.ForEach(nodes.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=>nodes.SelectMany((n) => n.getInPipes()).Distinct().ToArray();
        public ICollection<IPipe> getOutPipes()=>nodes.SelectMany((n) => n.getOutPipes()).Distinct().ToArray();
        public void setInPipes(ICollection<IPipe> inPipes)=>Array.ForEach(nodes.ToArray(), n => n.setInPipes(inPipes));
        public void setOutPipes(ICollection<IPipe> outPipes)=> nodes.Last().setOutPipes(outPipes);
    }
}
