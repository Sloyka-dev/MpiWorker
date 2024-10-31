using MPI;
using System;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace MpiWorker
{

    internal class MatrixWorker
    {

        public static string[] Args;
        public static bool PrintAllow = false;

        public static void Execute(string aInfo, string bInfo)
        {

            MPI.Environment.Run(ref Args, comm =>
            {

                var a = DserializeObject(aInfo);
                var b = DserializeObject(bInfo);


                if (comm.Size == 1) throw new Exception("Too few workers");
                int[,] r = new int[a.GetLength(0), b.GetLength(1)];
                byte[] data = Encoding.UTF8.GetBytes(SerializeObject(r));

                if (comm.Rank == 0)
                {
                    //delegate work

                    if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Matrices cannot be multiplied");

                    int i = 0;
                    int n = a.GetLength(0) / (comm.Size - 1) + (a.GetLength(0) % (comm.Size - 1) >= 1 ? 1 : 0);

                    for (int l = 1; l < comm.Size; l++)
                    {

                        int workRes = a.GetLength(0) / (comm.Size - 1) + (a.GetLength(0) % (comm.Size - 1) >= l ? 1 : 0);
                        Print(comm, $"Send count of work {workRes} to node {l}", PrintTag.Send);
                        comm.Send<int>(workRes, l, 10);

                    }

                    do
                    {

                        Print(comm, "Master node do a delegate work cycle");

                        for (int l = 1; l < comm.Size; i++, l++)
                        {

                            if (i >= a.GetLength(0)) break;

                            Print(comm, $"Send {i} to node {l}", PrintTag.Send);
                            comm.Send<int>(i, l, 0);

                        }

                    } while (i < a.GetLength(0));

                    var res = comm.Gather<byte[]>(data, 0);
                    Print(comm, $"Receive result", PrintTag.Get);

                    int[,] resArr;

                    Print(comm, "Merge result");

                    for (int resI = 0; resI < r.GetLength(0); resI++)
                        for (int resL = 0; resL < r.GetLength(1); resL++)
                            for (int k = 0; k < res.Length; k++)
                            {

                                resArr = DserializeObject(Encoding.UTF8.GetString(res[k]));
                                r[resI, resL] += resArr[resI, resL];

                            }

                    Print(comm, "Print result");

                    for (int resI = 0; resI < r.GetLength(0); resI++)
                    {

                        Console.Write("|");
                        for (int resL = 0; resL < r.GetLength(1); resL++)
                        {

                            Console.Write("{0,5}|", r[resI, resL]);

                        }

                        Console.WriteLine();
                    
                    }

                }
                else
                {
                    //work

                    int n;
                    comm.Receive<int>(0, 10, out n);
                    Print(comm, $"Receive count of work {n} to node {comm.Rank}", PrintTag.Get);
                    if (n == 0)
                    {
                        comm.Gather<byte[]>(data, 0);
                        return;
                    }

                    int i;

                    for (int m = 0; m < n; m++)
                    {

                        comm.Receive<int>(0, 0, out i);
                        Print(comm, $"Receive {i} to node {comm.Rank}", PrintTag.Get);

                        for (int j = 0; j < b.GetLength(1); j++)
                            for (int k = 0; k < b.GetLength(0); k++)
                                r[i, j] += a[i, k] * b[k, j];

                    }

                    data = Encoding.UTF8.GetBytes(SerializeObject(r));
                    Print(comm, $"Send result for {comm.Rank}", PrintTag.Send);
                    comm.Gather<byte[]>(data, 0);

                }

            });

        }

        private const char DELIMITER = '|';
        private const char ROW_DELIMITER = ';';
        private const char COLUMN_DELIMITER = ',';

        private enum PrintTag
        {

            Send,
            Get,
            Info

        }

        private static void Print<T>(Intracommunicator comm, T mess, PrintTag tag = PrintTag.Info)
        {

            if(PrintAllow) Console.WriteLine("[{0, 4}-{1, 2}] {2}", tag.ToString(), comm.Rank, mess);

        }

        public static string SerializeObject(int[,] source)
        {

            string res = "";
            res += source.GetLength(0).ToString() + DELIMITER;
            res += source.GetLength(1).ToString() + DELIMITER;

            for (int i = 0; i < source.GetLength(0); i++)
            {

                for (int l = 0; l < source.GetLength(1); l++)
                {

                    res += source[i, l];
                    if (source.GetLength(1) != l + 1) res += COLUMN_DELIMITER;

                }

                if (source.GetLength(0) != i + 1) res += ROW_DELIMITER;

            }

            return res;

        }

        private static int[,] DserializeObject(string source)
        {

            int n, m;
            var sourceInfo = source.Split(DELIMITER);
            n = int.Parse(sourceInfo[0]);
            m = int.Parse(sourceInfo[1]);

            var arrInfo = sourceInfo[2].Split(ROW_DELIMITER);
            string[] columnInfo;
            int[,] res = new int[n, m];

            for (int i = 0; i < n; i++)
            {

                columnInfo = arrInfo[i].Split(COLUMN_DELIMITER);

                for (int l = 0; l < m; l++)
                    res[i, l] = int.Parse(columnInfo[l]);

            }

            return res;

        }

    }

}