AI-Programmer
=========

Read the full article at:
http://www.primaryobjects.com/CMS/Article149.aspx
 
AI-Programmer is an experiment with using artificial intelligence and genetic algorithms to automatically generate a program, in the Brainf*** programming language, that writes a specific phrase to the console. In short, it's an AI genetic algorithm implementation with self modifying code.

## Motivation

Is it possible for a computer to write its own programs? Need a word processor? Let the computer create one for you. Need a screen capture tool? Let the computer create one for you. Take it a step further, and let the computer create programs that simplify your life, that you didn't even know you needed!

This is the idea behind the AI-Programmer experiment. The goal is to ultimately create a computer program that can write its own computer programs to solve specific computational problems. While the capability of a computer deciding what type of program to write is beyond our current means, we can still have a computer generate programs to solve very specific tasks, such as outputting the text, "Hello World". First, we need to choose a target programming language. For this experiment, we'll choose [Brainf***](http://en.wikipedia.org/wiki/Brainfuck).

## Details

The Brainf*** programming language was selected due to the fact that it consists of only 8 instructions, while being Turing complete. Theoretically, it is capable of solving any computational problem. This makes it easy to develop an interpreter, capable of running the AI-generated programs in a simulated environment. In this manner, each generated program may be executed and its performance ranked as a fitness score. Since the AI is using a Turing complete programming language, the AI itself, is also theoretically capable of solving any computational problem. However, for this experiment, the AI will focus on outputting a simple string to the console.

## How It Works

AI-Programmer works as follows:

- A genome consists of an array of doubles.
- Each gene corresponds to an instruction in the Brainf*** programming language.
- Start with a population of random genomes.
- Decode each genome into a resulting program by converting each double into its corresponding instruction and execute the program.
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

Keep in mind, this is a proof of concept. So far, the program has successfully written several programs in its target programming language. The tests were ran on an Intel Core 2 Quad 2.5GHz.

## hi

The AI successfully wrote a program to output "hi" after 5,700 generations in about 1 minute. It produced the following code:

```
+[+++++-+>++>++-++++++<<]>++.[+.]-.,-#>>]<]
```

While the above code contains parsing errors, such as non-matching brackets, our simulation interpreter computes the result up until the program fails, so in the above case, the syntax error (which is later on in the code, after a solution is found) doesn't impact the fitness.

You can try pasting the above code into a Brainf*** [interpreter](http://www.iamcal.com/misc/bf_debug/). Click "Start Debugger", ignore the warnings, then click Run To Breakpoint. Note the output.

If we trim off the excess code, we see the following syntactically-valid code:

```
+[+++++-+>++>++-++++++<<]>++.[+.]
```

## hello

The AI successfully wrote a program to output "hello" after 252,0000 generations in about 29 minutes. It produced the following code:

```
+-+-+>-<[++++>+++++<+<>++]>[-[---.--[[-.++++[+++..].+]],]<-+<+,.+>[[.,],+<.+-<,--+.]],+][[[.+.,,+].-
```

During the generation process, the AI came pretty close to a solution, but a couple letters were bound to each other, within a loop. The AI was able to overcome this by creating an inner-loop, within the problematic one, that successfully output the correct character, and continued processing.

## Hi!

In another example, the AI successfully wrote a program to output "Hi!" after 1,219,400 generations in about 2 hours and 7 minutes. It produced the following code:

```
>-----------<++[[++>++<+][]>-.+[+++++++++++++++++++++++++++++><+++.<><-->>>+].]
```

## I love all humans

The AI successfully wrote a program to output "I love all humans" after 6,057,200 generations in about 10 hours. It produced the following code:

```
+[>+<+++]+>------------.+<+++++++++++++++++++++++++++++++.>++++++++++++++++++++++++++++++++++.+++.+++++++.-----------------.--<.>--.+++++++++++..---<.>-.+++++++++++++.--------.------------.+++++++++++++.+++++.
```

More complex programs could likely be generated while using faster PCs.

## Author

Kory Becker
http://www.primaryobjects.com/kory-becker.aspx

Read @ Primary Objects
http://www.primaryobjects.com/CMS/Article149.aspx

View @ GitHub
https://github.com/primaryobjects/ai-programmer