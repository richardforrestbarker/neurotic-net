
using System.Collections.Generic;

namespace Neurotic
{
    public interface INeuralCalculator
    {
        void Calculate();
    }

    public interface INeuralInterfaceIOAggregate : INeuralCalculator
    {
        ICollection<IPipe> getInput();
        ICollection<IPipe> getOutput();
        void setInput(ICollection<IPipe> inPipes);
        void setOutput(ICollection<IPipe> outPipe);
    }

    public interface INeuralInterfaceInputAggregate : INeuralCalculator
    {
        ICollection<IPipe> getInput();
        IPipe getOutput();
        void setInput(ICollection<IPipe> inPipes);
        void setOutput(IPipe outPipe);
    }

    public interface INeuralInterfaceOutputAggregate : INeuralCalculator
    {
        IPipe getInput();
        ICollection<IPipe> getOutput();
        void setInput(IPipe inPipe);
        void setOutput(ICollection<IPipe> outPipes);
    }
}
