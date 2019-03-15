using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public interface INeuralInterface
    {
        void Calculate();
        ICollection<IPipe> getInPipes();
        ICollection<IPipe> getOutPipes();
        void setInPipes(ICollection<IPipe> inPipes);
        void setOutPipes(ICollection<IPipe> outPipes);
    }
}
