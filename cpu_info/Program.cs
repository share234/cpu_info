using System;
using System.IO;
namespace cpu_info
{
    class printcpudata
    {
        float[,] thread_data;
        public void getdata(float[,]data)
        {
            thread_data = data;
        }
        public void printdata()
        { 
            for (int i = 0; i < thread_data.GetLength(0);i++)
            {
                Console.WriteLine($"the usage for thread {i} is {thread_data[i,4]} and the speed is {thread_data[i, 5]}");
            }
        }
    }
    class getcpudata
    {
        //has a object for each thread, this containing old value, new value, old idle, new idle, usage and cpu speed
        float[,] thread_array;
        public getcpudata(int threadcount)
        {
            thread_array = new float[threadcount, 6];
        }

        public void gatherdata()
        {
            //get cpu speed for each thread
            string[] data = File.ReadAllLines("/proc/cpuinfo");
            int line = 7;
            int count = 0;
            while(line < data.Length)
            { 
            thread_array[count, 5] = Convert.ToSingle(data[line].Substring(11));
                count++;
                line += 28;
            }
            //get cpu usage for each thread
            data = File.ReadAllLines("/proc/stat");
            string[][] split_data = new string[data.Length][];
            count = 0;
            foreach(string i in data)
            {
                split_data[count] = i.Split(' ');
                count++;
            }
            for(int i = 1; i < 12;i++)
            {
                thread_array[i - 1,1] = Convert.ToSingle(split_data[i][1]) +
                                        Convert.ToSingle(split_data[i][2]) +
                                        Convert.ToSingle(split_data[i][3]) +
                                        Convert.ToSingle(split_data[i][4]);
                thread_array[i - 1, 3] = Convert.ToSingle(split_data[i][4]);
                thread_array[i - 1, 4] = (100 * (thread_array[i - 1, 0] - thread_array[i - 1, 1] - thread_array[i - 1, 2] + thread_array[i - 1, 3])) / (thread_array[i - 1, 0] - thread_array[i - 1, 1]);
                Console.WriteLine((100 * (thread_array[i - 1, 0] - thread_array[i - 1, 1] - thread_array[i - 1, 2] + thread_array[i - 1, 3])) / (thread_array[i - 1, 0] - thread_array[i - 1, 1]));
                thread_array[i - 1, 0] = thread_array[i - 1, 1];
                thread_array[i - 1, 2] = thread_array[i - 1, 3];
            }

        }
        public float[,] returndata()
        {
            return thread_array;
        }
    }


    class MainClass
    {
        public static void Main(string[] args)
        {
            getcpudata data = new getcpudata(12);
            printcpudata yes = new printcpudata();
            data.gatherdata();
            data.gatherdata();
            data.gatherdata();
            yes.getdata(data.returndata());
            yes.printdata();

        }
    }
}
