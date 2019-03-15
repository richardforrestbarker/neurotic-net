using System;
using System.Collections.Generic;
using System.Text;
using Neurotic;

namespace Neurotic.Factory
{
    public interface INeuralInterfaceFactory{
        void Construct(INeuralInterface network, ICollection<IPipe> inp, ICollection<IPipe> outp);
    }

    public class ConvolutionNetworkFactory : INeuralInterfaceFactory
    {
        
    }
}
