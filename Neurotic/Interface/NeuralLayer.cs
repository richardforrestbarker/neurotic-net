using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public abstract class NeuralLayer :LinkedList<INeuralInterface>, INeuralInterface
    {
        public NeuralLayer(ICollection<INeuralInterface> neurons) : base(neurons) { }
        public void Calculate() => Array.ForEach(this.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=>this.SelectMany((n) => n.getInPipes()).Distinct().ToArray();
        public ICollection<IPipe> getOutPipes()=>this.SelectMany((n) => n.getOutPipes()).Distinct().ToArray();
        public void setInPipes(ICollection<IPipe> inPipes)=>Array.ForEach(this.ToArray(), n => n.setInPipes(inPipes));
        public void setOutPipes(ICollection<IPipe> outPipes)=> Array.ForEach(this.ToArray(), n => n.setOutPipes(outPipes));
    }
}
