using System;

namespace DaemonStageDispatcher
{
    public class TestDaemonStage : IDaemonStage
    {
        public string Name { get; }
        public string[] StagesBefore { get; }
        public string[] StagesAfter { get; }
        public bool Executed { get; private set; }

        public TestDaemonStage(string name, string[] stagesBefore, string[] stagesAfter)
        {
            Name = name;
            StagesBefore = stagesBefore;
            StagesAfter = stagesAfter;
        }

        public void Execute(IPsiSourceFile sourceFile, Func<bool> checkForInterrupt)
        {
            if (checkForInterrupt()) return;
            Console.WriteLine("Run " + Name);
            Executed = true;
        }
    }
}
