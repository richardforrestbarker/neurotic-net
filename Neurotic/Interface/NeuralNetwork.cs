using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public abstract class NeuralNetwork : LinkedList<INeuralInterface>, INeuralInterface
    {
        public NeuralNetwork(ICollection<INeuralInterface> layers) : base(layers) { }
        public void Calculate()=>Array.ForEach(this.ToArray(), n => n.Calculate());
        public ICollection<IPipe> getInPipes()=> First.Value.getInPipes();
        public ICollection<IPipe> getOutPipes()=> Last.Value.getOutPipes();
        public void setInPipes(ICollection<IPipe> inPipes)=> First.Value.setInPipes(inPipes);
        public void setOutPipes(ICollection<IPipe> outPipes) => Last.Value.setOutPipes(outPipes);
        
    }
}
