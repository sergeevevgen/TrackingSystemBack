using BenchmarkDotNet.Running;
using PerfomanceTest;

namespace PerformanceTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PerformanceTests>();
        }
    }
}