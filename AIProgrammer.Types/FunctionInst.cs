using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer.Types
{
    /// <summary>
    /// Function specific settings for the interpreter. Includes the instruction pointer value.
    /// </summary>
    public class FunctionInst : Function
    {
        /// <summary>
        /// Starting instruction index for this function, within the program code.
        /// </summary>
        public int InstructionPointer { get; private set; }

        public FunctionInst(int instructionPointer, Function function)
            : base(function)
        {
            InstructionPointer = instructionPointer;
        }
    }
}
