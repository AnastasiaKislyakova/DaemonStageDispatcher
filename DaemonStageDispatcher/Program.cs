using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonStageDispatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var executedStages = new List<string>();
            var A = new TestDaemonStage("A", new string[] { }, new[] { "C", "D", "E" });
            var B = new TestDaemonStage("B", new[] { "A" }, new[] { "D", "E" });
            var C = new TestDaemonStage("C", new string[] { }, new[] { "D", "E" });
            var D = new TestDaemonStage("D", new[] { "A", "B", "C" }, new string[] { });
            var E = new TestDaemonStage("E", new[] { "C" }, new string[] { });
            var dispatcher = new DaemonStageDispatcher(new IDaemonStage[] { D, A, B, C, E });
            var names = dispatcher.OrderedDaemonStages.Select(it  => it.Name);
        }
    }
}
