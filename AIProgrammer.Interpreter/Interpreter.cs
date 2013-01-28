using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProgrammer
{
    /// <summary>
    /// This the brainfuck interpreter
    /// 
    /// > 	Increment the pointer.
    /// < 	Decrement the pointer.
    /// + 	Increment the byte at the pointer.
    /// - 	Decrement the byte at the pointer.
    /// . 	Output the byte at the pointer.
    /// , 	Input a byte and store it in the byte at the pointer.
    /// [ 	Jump forward past the matching ] if the byte at the pointer is zero.
    /// ] 	Jump backward to the matching [ unless the byte at the pointer is zero.
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// The "call stack"
        /// </summary>
        private readonly Stack<int> m_CallStack = new Stack<int>();

        /// <summary>
        /// The input function
        /// </summary>
        private readonly Func<byte> m_Input;

        /// <summary>
        /// The instruction set
        /// </summary>
        private readonly IDictionary<char, Action> m_InstructionSet =
            new Dictionary<char, Action>();

        /// <summary>
        /// The memory of the program
        /// </summary>
        private readonly byte[] m_Memory = new byte[32768];

        /// <summary>
        /// The output function
        /// </summary>
        private readonly Action<byte> m_Output;

        /// <summary>
        /// The program code
        /// </summary>
        private readonly char[] m_Source;

        /// <summary>
        /// The data pointer
        /// </summary>
        private int m_DataPointer;

        /// <summary>
        /// The instruction pointer
        /// </summary>
        private int m_InstructionPointer;

        /// <summary>
        /// Boolean flag to indicate if we should skip the loop and continue execution at the next valid instruction. Used if the pointer is zero and a begin loop [ instruction is read, in which case we jump forward past the matching ].
        /// </summary>
        private bool m_ExitLoop;

        /// <summary>
        /// Count of number of instructions executed.
        /// </summary>
        public int m_Ticks;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public Interpreter(string programCode, Func<byte> input, Action<byte> output)
        {
            // Save the program code
            this.m_Source = programCode.ToCharArray();

            // Store the i/o delegates
            this.m_Input = input;
            this.m_Output = output;

            // Create the instruction set (lol)
            this.m_InstructionSet.Add('+', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer]++; });
            this.m_InstructionSet.Add('-', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer]--; });

            this.m_InstructionSet.Add('>', () => { if (!m_ExitLoop) this.m_DataPointer++; });
            this.m_InstructionSet.Add('<', () => { if (!m_ExitLoop) this.m_DataPointer--; });

            this.m_InstructionSet.Add('[', () => 
                {
                    if (!m_ExitLoop)
                    {
                        if (this.m_Memory[this.m_DataPointer] == 0)
                        {
                            m_ExitLoop = true;
                        }
                        else
                        {
                            this.m_CallStack.Push(this.m_InstructionPointer);
                        }
                    }
                });
            this.m_InstructionSet.Add(
                ']',
                () =>
                {
                    if (!m_ExitLoop)
                    {
                        var temp = this.m_CallStack.Pop() - 1;
                        this.m_InstructionPointer = this.m_Memory[this.m_DataPointer] != 0
                            ? temp
                            : this.m_InstructionPointer;
                    }
                    else
                    {
                        // Continue executing after loop.
                        m_ExitLoop = false;
                    }
                });

            this.m_InstructionSet.Add('.', () => this.m_Output(this.m_Memory[this.m_DataPointer]));
            this.m_InstructionSet.Add(',', () => this.m_Memory[this.m_DataPointer] = this.m_Input());
        }

        /// <summary>
        /// Run the program
        /// </summary>
        public void Run(int maxInstructions = 0)
        {
            // Initialize tick counter (number of instructions executed).
            this.m_Ticks = 0;

            if (maxInstructions > 0)
            {
                RunLimited(maxInstructions);
            }
            else
            {
                RunUnlimited();
            }
        }

        /// <summary>
        /// Run the program with a maximum number of instructions before throwing an exception. Avoids infinite loops.
        /// </summary>
        /// <param name="maxInstructions">Max number of instructions to execute</param>
        private void RunLimited(int maxInstructions)
        {
            // Iterate through the whole program source
            while (this.m_InstructionPointer < this.m_Source.Length)
            {
                // Fetch the next instruction
                char instruction = this.m_Source[this.m_InstructionPointer];

                // See if that IS an instruction and execute it if so
                Action action;
                if (this.m_InstructionSet.TryGetValue(instruction, out action))
                {
                    // Yes, it was - execute
                    action();
                }

                // Next instruction
                this.m_InstructionPointer++;

                // Have we exceeded the max instruction count?
                if (maxInstructions > 0 && m_Ticks > maxInstructions)
                {
                    break;
                }

                // Increment number of instructions executed.
                m_Ticks++;
            }
        }

        /// <summary>
        /// Run the program
        /// </summary>
        private void RunUnlimited()
        {
            // Iterate through the whole program source
            while (this.m_InstructionPointer < this.m_Source.Length)
            {
                // Fetch the next instruction
                char instruction = this.m_Source[this.m_InstructionPointer];

                // See if that IS an instruction and execute it if so
                Action action;
                if (this.m_InstructionSet.TryGetValue(instruction, out action))
                {
                    // Yes, it was - execute
                    action();
                }

                // Next instruction
                this.m_InstructionPointer++;

                // Increment number of instructions executed.
                m_Ticks++;
            }
        }
    }
}
