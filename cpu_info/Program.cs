using System;
using System.IO;
namespace cpu_info
{
    class getcpudata
    {
        //has a object for each thread, this containing old usage and new usage and cpu speed
        float[,] thread_array;
        public getcpudata(int threadcount)
        {
            thread_array = new float[threadcount, 3];
        }

        public void gatherdata()
        {
            //get cpu speed for each thread
            string[] data = File.ReadAllLines("/proc/cpuinfo");
            int line = 7;
            int count = 0;
            while(line < data.Length)
            {
                Console.WriteLine(data[line].Substring(13));
                thread_array[count, 2] = Convert.ToSingle(data[line].Substring(11));
                count++;
                line += 28;
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
            data.gatherdata();
            float[,] gathereddata = data.returndata();
            foreach(float i in gathereddata)
            {
                Console.WriteLine(i);
            }
        }
    }
}
