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
    /// 
    /// Extended commands, included in BrainPlus.
    /// !   Exits the program.
    /// &   Defines a new function a,b,c .. z.
    /// %   Return to last position in main program and restore state. Current memory value of function is set in current program memory value.
    /// a,b Call function a - z.
    /// 0-F Sets the value of the current memory pointer to a multiple of 16.
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// Object used to swap state for a function call. This data is restored when the function terminates.
        /// </summary>
        public class FunctionCallObj
        {
            public int InstructionPointer { get; set; }
            public int DataPointer { get; set; }
            public Stack<int> CallStack { get; set; }
            public bool ExitLoop { get; set; }
            public int Ticks { get; set; }
        };

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
        /// The list of functions and their starting instruction index.
        /// </summary>
        private readonly Dictionary<char, int> m_Functions = new Dictionary<char, int>(26);

        /// <summary>
        /// Identifier for next function. Will serve as the instruction to call this function.
        /// </summary>
        private char m_NextFunctionCharacter = 'a';

        /// <summary>
        /// The function "call stack".
        /// </summary>
        private readonly Stack<FunctionCallObj> m_FunctionCallStack = new Stack<FunctionCallObj>();

        /// <summary>
        /// Pointer to the current call stack (m_FunctionCallStack or m_CallStack).
        /// </summary>
        private Stack<int> m_CurrentCallStack;

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
            
            m_CurrentCallStack = m_CallStack;

            // Create the instruction set (lol)
            this.m_InstructionSet.Add('+', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer]++; });
            this.m_InstructionSet.Add('-', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer]--; });

            this.m_InstructionSet.Add('>', () => { if (!m_ExitLoop) this.m_DataPointer++; });
            this.m_InstructionSet.Add('<', () => { if (!m_ExitLoop) this.m_DataPointer--; });

            this.m_InstructionSet.Add('.', () => { if (!m_ExitLoop) this.m_Output(this.m_Memory[this.m_DataPointer]); });
            this.m_InstructionSet.Add(',', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = this.m_Input(); });

            this.m_InstructionSet.Add('0', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 0; });
            this.m_InstructionSet.Add('1', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 16; });
            this.m_InstructionSet.Add('2', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 32; });
            this.m_InstructionSet.Add('3', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 48; });
            this.m_InstructionSet.Add('4', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 64; });
            this.m_InstructionSet.Add('5', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 80; });
            this.m_InstructionSet.Add('6', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 96; });
            this.m_InstructionSet.Add('7', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 112; });
            this.m_InstructionSet.Add('8', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 128; });
            this.m_InstructionSet.Add('9', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 144; });
            this.m_InstructionSet.Add('A', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 160; });
            this.m_InstructionSet.Add('B', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 176; });
            this.m_InstructionSet.Add('C', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 192; });
            this.m_InstructionSet.Add('D', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 208; });
            this.m_InstructionSet.Add('E', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 224; });
            this.m_InstructionSet.Add('F', () => { if (!m_ExitLoop) this.m_Memory[this.m_DataPointer] = 240; });

            this.m_InstructionSet.Add('!', () => { this.m_Stop = true; });
            this.m_InstructionSet.Add('&', () => { m_Functions.Add(m_NextFunctionCharacter++, this.m_InstructionPointer); });
            this.m_InstructionSet.Add('%', () =>
            {
                var temp = m_FunctionCallStack.Pop();
                
                // Get the result from the function call.
                var result = this.m_Memory[this.m_DataPointer];

                // Restore the data pointer.
                this.m_DataPointer = temp.DataPointer;
                // Set the value of memory equal to the function result.
                this.m_Memory[this.m_DataPointer] = result;
                // Restore the call stack.
                this.m_CurrentCallStack = temp.CallStack;
                // Restore exit loop status.
                this.m_ExitLoop = temp.ExitLoop;
                // Restore ticks.
                this.m_Ticks = temp.Ticks;
                // Restore the instruction pointer.
                this.m_InstructionPointer = temp.InstructionPointer;
            });

            for (char inst = 'a'; inst <= 'f'; inst++)
            {
                char instruction = inst; // closure
                this.m_InstructionSet.Add(instruction, () =>
                {
                    // Store the current instruction pointer and data pointer before we move to the function.
                    var functionCallObj = new FunctionCallObj { InstructionPointer = this.m_InstructionPointer, DataPointer = this.m_DataPointer, CallStack = this.m_CurrentCallStack, ExitLoop = this.m_ExitLoop, Ticks = this.m_Ticks };
                    this.m_FunctionCallStack.Push(functionCallObj);

                    // Give the function a fresh call stack.
                    this.m_CurrentCallStack = new Stack<int>();
                    this.m_ExitLoop = false;

                    // Get current memory value to use as input for the function.
                    var inputValue = this.m_Memory[this.m_DataPointer];

                    // Set the data pointer to the functions starting memory address.
                    this.m_DataPointer = 1000 * (instruction - 96); // each function gets a space of 1000 memory slots.

                    // Clear function memory.
                    Array.Clear(this.m_Memory, this.m_DataPointer, 1000);

                    // Copy the input value to the function's starting memory address.
                    this.m_Memory[this.m_DataPointer] = inputValue;

                    // Set the instruction pointer to the beginning of the function.
                    this.m_InstructionPointer = m_Functions[instruction];
                });
            }

            this.m_InstructionSet.Add('[', () =>
            {
                if (!m_ExitLoop && this.m_Memory[this.m_DataPointer] == 0)
                {
                    // Jump forward to the matching ] and exit this loop (skip over all inner loops).
                    m_ExitLoop = true;

                    // Remember this instruction pointer, so when we get past all inner loops and finally pop this one off the stack, we know we're done.
                    m_ExitLoopInstructionPointer = this.m_InstructionPointer;
                }

                this.m_CurrentCallStack.Push(this.m_InstructionPointer);
            });
            this.m_InstructionSet.Add(']', () =>
            {
                var temp = this.m_CurrentCallStack.Pop();

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

            ScanFunctions(programCode);
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
                    if (m_FunctionCallStack.Count > 0)
                    {
                        // We're inside a function, but ran out of instructions. Exit the function, but continue.
                        if (this.m_InstructionSet.TryGetValue('%', out action))
                        {
                            action();
                            this.m_InstructionPointer++;
                        }
                    }
                    else
                    {
                        break;
                    }
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

        /// <summary>
        /// Pre-scan the program code to record function instruction pointers.
        /// </summary>
        private void ScanFunctions(string source)
        {
            this.m_InstructionPointer = source.IndexOf('&');
            while (this.m_InstructionPointer > -1 && this.m_InstructionPointer < source.Length && !m_Stop)
            {
                // Fetch the next instruction
                char instruction = this.m_Source[this.m_InstructionPointer];

                Action action;
                if (this.m_InstructionSet.TryGetValue(instruction, out action))
                {
                    // Store the function.
                    action();
                }

                this.m_InstructionPointer = source.IndexOf('&', this.m_InstructionPointer + 1);
            }

            this.m_InstructionPointer = 0;
        }
    }
}
