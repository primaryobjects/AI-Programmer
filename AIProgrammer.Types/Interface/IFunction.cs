using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Types.Interface
{
    public interface IFunction
    {
        string Generate(IGeneticAlgorithm ga);
    }
}
