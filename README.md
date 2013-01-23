AI-Programmer
=========

AI-Programmer is an experiment with using artificial intelligence and genetic algorithms to automatically generate a program, in the Brainf*** programming language, that writes a specific phrase to the console.

## Details

The Brainf*** programming language was selected due to the fact that it consists of only 8 instructions, while being Turing complete. Theoretically, it is capable of solving any computational problem. This makes it easy to develop an interpreter, capable of running the AI-generated programs in a simulated environment. In this manner, each generated program may be executed and its performance ranked as a fitness score. Since the AI is using a Turing complete programming language, the AI itself, is also theoretically capable of solving any computational problem. However, for this experiment, the AI will focus on outputting a simple string to the console.

## How It Works

AI-Programmer works as follows:

- A genome consists of an array of doubles.
- Each gene cooresponds to an instruction in the Brainf*** programming language.
- Start with a population of random genomes.
- Decode each genome into a resulting program by converting each double into its cooresponding instruction and execute the program.
- Get each program's fitness score, based upon the output it writes to the console (if any), and rank them.
- Mate the best genomes together using roulette selection, crossover, and mutation to produce a new generation.
- Repeat the process with the new generation until the target fitness score is achieved.

## The Fitness Method

The fitness method works by scoring the output of the generated program. The score is calculated by looking at each character ouput by the program and subtracting its value from the desired character:

```
fitness += 256 - Math.Abs(console[i] - targetString[i]);
```

## Interpreter Instruction Set

```
> 	Increment the pointer.
< 	Decrement the pointer.
+ 	Increment the byte at the pointer.
- 	Decrement the byte at the pointer.
. 	Output the byte at the pointer.
, 	Input a byte and store it in the byte at the pointer.
[ 	Jump forward past the matching ] if the byte at the pointer is zero.
] 	Jump backward to the matching [ unless the byte at the pointer is zero.
```

## Results?

Keep in mind, this is a proof of concept. So far, the program has successfully written a program in its target programming language to output "Hi!" after 4,559,681 generations in about 4 hours on an Intel Core 2 Quad 2.5GHz. More complex programs could likely be generated while using faster PCs.

## Author

Kory Becker
http://www.primaryobjects.com

View @ GitHub
https://github.com/primaryobjects/ai-programmer