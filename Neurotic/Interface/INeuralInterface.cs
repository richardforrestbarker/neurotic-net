
using System.Collections.Generic;

namespace Neurotic
{
    public interface INeuralInterface
    {
        void Calculate();
        ICollection<IPipe> getInPipes();
        ICollection<IPipe> getOutPipes();
        void setInPipes(ICollection<IPipe> inPipes);
        void setOutPipes(ICollection<IPipe> outPipe);
    }
}
