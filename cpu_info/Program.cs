using System;

namespace cpu_info
{
    class getcpudata
    {
        float[,] thread_array;
        public getcpudata(int threadcount)
        {
            thread_array = new float[threadcount, 2];
        }
    }


    class MainClass
    {
        public static void Main(string[] args)
        {
        }
    }
}
