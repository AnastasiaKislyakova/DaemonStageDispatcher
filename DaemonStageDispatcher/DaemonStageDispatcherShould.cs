using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;

namespace DaemonStageDispatcher
{
    public class DaemonStageDispatcherShould
    {
        private IPsiSourceFile sourceFile;

        [Test]
        public void Should_order_Stages()
        {
            var stages = GetDaemonStages();
            var dispatcher = new DaemonStageDispatcher(stages);

            bool CheckForInterrupt() => false;

            dispatcher.Execute(sourceFile, CheckForInterrupt);
            AssertOrderedStages(dispatcher.OrderedDaemonStages);
        }

        [Test]
        public void Should_execute_stages_and_interrupt()
        {
            var stopCount = 3;
            var stages = GetDaemonStages();
            var dispatcher = new DaemonStageDispatcher(stages);

            var interruptCounter = new InterruptCounter(stopCount * 2);
            Func<bool> checkForInterrupt = interruptCounter.Inter;
            dispatcher.Execute(sourceFile, checkForInterrupt);

            dispatcher
                .OrderedDaemonStages
                .Count(it => ((TestDaemonStage)it).Executed)
                .ShouldBeEquivalentTo(stopCount);
        }


        private IDaemonStage[] GetDaemonStages()
        {
            var A = new TestDaemonStage("A", Array.Empty<string>(), new[] {"C", "D", "E"});
            var B = new TestDaemonStage("B", new[] {"A"}, new[] {"D", "E"});
            var C = new TestDaemonStage("C", Array.Empty<string>(), new[] {"D", "E"});
            var D = new TestDaemonStage("D", new[] {"A", "B", "C"}, Array.Empty<string>());
            var E = new TestDaemonStage("E", new[] {"C"}, Array.Empty<string>());
            return new IDaemonStage[] {D, A, B, C, E};
        }

        private void AssertOrderedStages(IDaemonStage[] stages)
        {
            var index = new Dictionary<string, int>();
            for (int i = 0; i < stages.Length; i++)
            {
                index.Add(stages[i].Name, i);
            }

            bool isCorrect = true;

            foreach (var stage in stages)
            {
                var stageName = stage.Name;
                foreach (var stageBefore in stage.StagesBefore)
                {
                    if (index[stageBefore] >= index[stageName])
                    {
                        isCorrect = false;
                        break;
                    }
                }

                foreach (var stageAfter in stage.StagesAfter)
                {
                    if (index[stageAfter] <= index[stageName])
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            isCorrect.Should().BeTrue();
        }
    }

    class InterruptCounter
    {
        private int _stopCount;
        private int _counter = 0;

        public InterruptCounter(int stopCount)
        {
            _stopCount = stopCount;
        }

        public bool Inter()
        {
            if (_counter == _stopCount) return true;
            _counter++;
            return false;
        }
    }
}