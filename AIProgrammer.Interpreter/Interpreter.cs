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
        /// Holds the instruction pointer for the start of the loop. Used to bypass all inner-loops when searching for the end of the current loop.
        /// </summary>
        private int m_ExitLoopInstructionPointer;

        /// <summary>
        /// Number of instructions executed.
        /// </summary>
        public int m_Ticks;

        /// <summary>
        /// Flag to stop execution of the program.
        /// </summary>
        public bool m_Stop;

        /// <summary>
        /// Read-only access to the current data pointer index in memory.
        /// </summary>
        public int m_CurrentDataPointer { get { return m_DataPointer; } }

        /// <summary>
        /// Read-only access to the current instruction pointer index.
        /// </summary>
        public int m_CurrentInstructionPointer { get { return m_InstructionPointer; } }

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

            this.m_InstructionSet.Add('.', () => { if (!m_ExitLoop) this.m_Output(this.m_Memory[this.m_DataPointer]); });
            this.m_InstructionSet.Add(',', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = this.m_Input(); });

            this.m_InstructionSet.Add('[', () =>
            {
                if (!m_ExitLoop && this.m_Memory[this.m_DataPointer] == 0)
                {
                    // Jump forward to the matching ] and exit this loop (skip over all inner loops).
                    m_ExitLoop = true;

                    // Remember this instruction pointer, so when we get past all inner loops and finally pop this one off the stack, we know we're done.
                    m_ExitLoopInstructionPointer = this.m_InstructionPointer;
                }

                this.m_CallStack.Push(this.m_InstructionPointer);
            });
            this.m_InstructionSet.Add(']', () =>
            {
                var temp = this.m_CallStack.Pop();

                if (!m_ExitLoop)
                {
                    this.m_InstructionPointer = this.m_Memory[this.m_DataPointer] != 0
                        ? temp - 1
                        : this.m_InstructionPointer;
                }
                else
                {
                    // Continue executing after loop.
                    if (temp == m_ExitLoopInstructionPointer)
                    {
                        // We've finally exited the loop.
                        m_ExitLoop = false;
                        m_ExitLoopInstructionPointer = 0;
                    }
                }
            });
        }

        /// <summary>
        /// Run the program
        /// </summary>
        public void Run(int maxInstructions = 0)
        {
            m_Ticks = 0;
            m_Stop = false;

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
            while (this.m_InstructionPointer < this.m_Source.Length && !m_Stop)
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
                if (maxInstructions > 0 && m_Ticks >= maxInstructions)
                {
                    break;
                }

                m_Ticks++;
            }
        }

        /// <summary>
        /// Run the program
        /// </summary>
        private void RunUnlimited()
        {
            // Iterate through the whole program source
            while (this.m_InstructionPointer < this.m_Source.Length && !m_Stop)
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

                m_Ticks++;
            }
        }
    }
}
