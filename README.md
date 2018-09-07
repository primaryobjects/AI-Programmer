AI-Programmer
=========

Read the full article [Using Artificial Intelligence to Write Self-Modifying/Improving Programs](http://www.primaryobjects.com/2013/01/27/using-artificial-intelligence-to-write-self-modifying-improving-programs/)

Read the research paper [AI Programmer: Autonomously Creating
Software Programs Using Genetic Algorithms](https://arxiv.org/pdf/1709.05703.pdf "AI Programmer: Autonomously Creating
Software Programs Using Genetic Algorithms").

AI-Programmer is an experiment with using artificial intelligence and genetic algorithms to automatically generate programs. Successfully created [programs](https://github.com/primaryobjects/AI-Programmer/tree/master/Results) by the AI include: hello world, hello <name>, addition, subtraction, reversing a string, fibonnaci sequence, 99 bottles of beer on the wall, and more. It's getting smarter. In short, it's an AI genetic algorithm implementation with self modifying code.

## Motivation

Is it possible for a computer to write its own programs? Need a word processor? Let the computer create one for you. Need a screen capture tool? Let the computer create one for you. Take it a step further, and let the computer create programs that simplify your life, that you didn't even know you needed!

This is the idea behind the AI-Programmer experiment. The goal is to ultimately create a computer program that can write its own computer programs to solve specific computational problems. While the capability of a computer deciding what type of program to write is beyond our current means, we can still have a computer generate programs to solve very specific tasks, such as outputting the text, "Hello World". AI Programmer uses an esoteric programming language for generating software programs.

## Details

The underlying programming language consists of only 8 instructions, while being Turing complete. Theoretically, it is capable of solving any computational problem. This makes it easy to develop an interpreter, capable of running the AI-generated programs in a simulated environment. In this manner, each generated program may be executed and its performance ranked as a fitness score. Since the AI is using a Turing complete programming language, the AI itself, is also theoretically capable of solving any computational problem. However, for this experiment, the AI will focus on outputting a simple string to the console.

## How It Works

AI-Programmer works as follows:

- A genome consists of an array of doubles.
- Each gene corresponds to an instruction in the programming language.
- Start with a population of random genomes.
- Decode each genome into a resulting program by converting each double into its corresponding instruction and execute the program.
- Get each program's fitness score, based upon the output it writes to the console (if any), and rank them.
- Mate the best genomes together using roulette selection, crossover, and mutation to produce a new generation.
- Repeat the process with the new generation until the target fitness score is achieved.

## The Fitness Method

The fitness method works by scoring the output of the generated program. The score is calculated by looking at each character output by the program and subtracting its value from the desired character:

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

Keep in mind, this is a proof of concept. So far, the program has successfully written several programs in its target programming language. You can view screenshots of all the results in the [/Results](https://github.com/primaryobjects/AI-Programmer/tree/master/Results) folder. These tests were ran on an Intel Core 2 Quad 2.5GHz.

## hi

The AI successfully wrote a program to output "hi" after 5,700 generations in about 1 minute. It produced the following code:

```
+[+++++-+>++>++-++++++<<]>++.[+.]-.,-#>>]<]
```

While the above code contains parsing errors, such as non-matching brackets, our simulation interpreter computes the result up until the program fails, so in the above case, the syntax error (which is later on in the code, after a solution is found) doesn't impact the fitness.

You can try pasting the above code into an online [interpreter](http://www.iamcal.com/misc/bf_debug/). Click "Start Debugger", ignore the warnings, then click Run To Breakpoint. Note the output.

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

More complex programs could likely be generated while using faster PCs. Next steps include attempting to accept user input and process results.

## Quick-Start Guide to Using the Code

By default, the code is configured to use the Classic instruction set and to write a program to output a string. To change the string that is generated, simply expand the "Private Variables" section and change the text for TargetString to your desired value.

```
private static TargetParams _targetParams = new TargetParams { TargetString = "hello world" };
```

To change the type of program that the AI writes, change the fitness method inside GetFitnessMethod().

```
private static IFitness GetFitnessMethod()
{
	return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
}
```

You can change this to any class within the AI.Programmer.Fitness/Concrete project. Examples:

```
return new StringStrictFitness(_ga, _maxIterationCount, _targetParams.TargetString, _appendCode);
return new AddFitness(_ga, _maxIterationCount);
return new SubtractFitness(_ga, _maxIterationCount);
return new ReverseStringFitness(_ga, _maxIterationCount);
return new HelloUserFitness(_ga, _maxIterationCount, _targetString);
```

To use sub-routines, you'll need to enable functions. This will let the AI produce programs much faster. Uncomment the code for the functionGenerator, as follows:

```
private static IFunction _functionGenerator = new StringFunction(() => GetFitnessMethod(), _bestStatus, fitnessFunction, OnGeneration, _crossoverRate, _mutationRate, _genomeSize, _targetParams);
```

Set the App.config to use BrainPlus, so the AI has access to sub-routine instructions:

```
<appSettings>
	<add key="BrainfuckVersion" value="2"/> <!-- 1 = BF Classic, 2 = BrainPlus (BF Extended Type 3, Functions, Faster) -->
</appSettings>
```

When using sub-routines, less code is required. So, you can use a smaller genomeSize for speed. Expand the Genetic Algorithm Settings section and change the _genomeSize to a smaller value:

```
private static int _genomeSize = 50;
```

Experiment and have fun!

## Author

Kory Becker
http://www.primaryobjects.com/kory-becker

[Using Artificial Intelligence to Write Self-Modifying/Improving Programs](http://www.primaryobjects.com/CMS/Article149)

[Pushing the Limits of Self-Programming Artificial Intelligence](http://www.primaryobjects.com/CMS/Article150)

[Self-Programming Artificial Intelligence Learns to Use Functions](http://www.primaryobjects.com/CMS/Article163)

[BF-Programmer: A Counterintuitive Approach to Autonomously Building Simplistic Programs Using Genetic Algorithms](http://www.primaryobjects.com/bf-programmer-2017.pdf)

View @ GitHub
https://github.com/primaryobjects/ai-programmer

## Copyright

Copyright (c) 2018 Kory Becker http://primaryobjects.com/kory-becker
