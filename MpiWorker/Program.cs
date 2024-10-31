using MpiWorker;
using System;
using System.Linq;

MatrixWorker.Args = args;

const string ARG_N_COUNT = "-n";
const string ARG_PRINT = "-p";
const string ARG_HELP = "-help";

int N = 10;

for (int i = 0; i < args.Length; i ++)
    switch (args[i])
    {

        case ARG_HELP:

            Console.WriteLine("Usage \"mpiexec -n [COUNT OF THREADS] MpiWorker.exe [ARGUMENTS]\"\n" +
                "[COUNT OF THREADS] is the number of threads that could be running\n" +
                "[ARGUMENTS]:\n" +
                $" {ARG_N_COUNT} [count]\n" +
                "Defines the number of rows and columns of the matrix\n" +
                $" {ARG_PRINT}\n" +
                "Allows to print debugging messages\n");

            return;

        case ARG_N_COUNT:

            if (!int.TryParse(args[++i], out N))
                Console.WriteLine($"[ERROR] {args[i]} is not a number");

            break;

        case ARG_PRINT:

            MatrixWorker.PrintAllow = true;

            break;

    }

int[,] a = new int[N, N], b = new int[N, N];

for (int i = 0; i < N; i++)
    for (int j = 0; j < N; j++)
    {
        a[i, j] = i + j + 1;
        b[i, j] = i + j + 1;
    }

var sa = MatrixWorker.SerializeObject(a);
var sb = MatrixWorker.SerializeObject(b);

MatrixWorker.Execute(sa, sb);