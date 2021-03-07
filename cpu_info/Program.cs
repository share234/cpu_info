using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Renci.SshNet;
namespace cpu_info
{
    class hostinfo
    {
        public string hostname { get; set;}
        public string username { get; set; }
        public string password { get; set; }
        public hostinfo(string a1, string a2, string a3)
        {
            hostname = a1;
            username = a2;
            password = a3;
        }
    }
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
                Console.WriteLine($"the usage for thread {i} is {Math.Round(thread_data[i,4], 2)}");// and the speed is {thread_data[i, 5]}");
            }
            Console.WriteLine("\n");
        }
    }

    class getcpudata
    {
        //has a object for each thread, this containing old value, new value, old idle, new idle, usage and cpu speed
        float[,] thread_array;
        SshClient ssh;
        public getcpudata(int threadcount, hostinfo info)
        {
            ssh = new SshClient(info.hostname, info.username, info.password);
            ssh.Connect();
            thread_array = new float[threadcount, 6];
        }

        public void gatherdata()
        {
            //get cpu speed for each thread
            //string[] data = File.ReadAllLines("/proc/cpuinfo");
            //int line = 7;
            //int count = 0;
            //while(line < data.Length)
            //{ 
            //thread_array[count, 5] = Convert.ToSingle(data[line].Substring(11));
            //    count++;
            //    line += 28;
            //}
            //get cpu usage for each thread
            var result = ssh.RunCommand("cat /proc/stat");
            string[] data = result.Result.Split('\n');
            string[][] split_data = new string[data.Length][];
            int count = 0;
            foreach(string j in data)
            {
                split_data[count] = j.Split(' ');
                count++;
            }
            int i = 0;
            while( (i < thread_array.GetLength(0)) && split_data[i + 1][0].Contains("cpu"))
            //for(int i = 1; i <= thread_array.GetLength(0); i++)
            {
                //takes all times and adds them together, this is user time, low user time, system time and idle time
                thread_array[i,1] = Convert.ToSingle(split_data[i + 1][1]) +
                                        Convert.ToSingle(split_data[i + 1][2]) +
                                        Convert.ToSingle(split_data[i + 1][3]) +
                                        Convert.ToSingle(split_data[i + 1][4]);
                thread_array[i, 3] = Convert.ToSingle(split_data[i + 1][4]);
                if (thread_array[i, 1] != thread_array[i, 0])
                {
                    //this takes the times, checks if they are different and then works out the delta usage for the gap, this is either the delay the user inputs or time between updates on that file
                    thread_array[i, 4] = 100 * (1 - ((thread_array[i, 3] - thread_array[i, 2]) / (thread_array[i, 1] - thread_array[i, 0])));
                    thread_array[i, 0] = thread_array[i, 1];
                    thread_array[i, 2] = thread_array[i, 3];
                }
                i++;

            }
        }
        public float[,] returndata()
        {
            return thread_array;
        }
    }

    class getmemdata
    {

    }
    //todo
    //1. get mem working - meminfo
    //2. get disk working - diskinfo
    //3. get a GUI working
    //4. maybe get network stuff working

    class MainClass
    {
        public static void Main(string[] args)
        {
            var fs = new StreamReader("./hosts.json");
            string jsonValue = fs.ReadToEnd();
            hostinfo info = JsonConvert.DeserializeObject<hostinfo>(jsonValue);
            getcpudata data = new getcpudata(1 , info);
            printcpudata yes = new printcpudata();
            data.gatherdata();
            Thread.Sleep(1000);
            data.gatherdata();
            while (true)
            {
                Thread.Sleep(1000);
                data.gatherdata();
                yes.getdata(data.returndata());
                yes.printdata();
            }
        }
    }
}
