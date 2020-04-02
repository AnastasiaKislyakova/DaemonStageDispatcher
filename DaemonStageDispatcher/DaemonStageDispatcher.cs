using System;
using System.Collections.Generic;
using System.Linq;

namespace DaemonStageDispatcher
{
    public class DaemonStageDispatcher
    {
        private readonly IDaemonStage[] _daemonStages;

        public IDaemonStage[] OrderedDaemonStages { get; private set; }

        public DaemonStageDispatcher(IDaemonStage[] daemonStages)
        {
            _daemonStages = daemonStages;
            SortDaemonStages();
        }

        /**
         * This function is called when the file needs to be re-analyzed
         * <param name="sourceFile"> File to analyzed </param>
         * <param name="checkForInterrupt"> function that returns "true" when analysis should be interrupted. It is recommended
         * to check this function ofter and stop file processing as soon as possible if it returns "true" </param>
         */
        public void Execute(IPsiSourceFile sourceFile, Func<bool> checkForInterrupt)
        { 

            foreach (var daemonStage in OrderedDaemonStages)
            {
                if (checkForInterrupt()) return;
                daemonStage.Execute(sourceFile, checkForInterrupt);
            }
        }

        private void SortDaemonStages()
        {
            var stagesByName = new Dictionary<string, IDaemonStage>();
            var graph = new Dictionary<string, ISet<string>>();

            foreach (var daemonStage in _daemonStages)
            {
                stagesByName[daemonStage.Name] = daemonStage;
                graph[daemonStage.Name] = new HashSet<string>();

            }

            foreach (var daemonStage in _daemonStages)
            {
                foreach (var stageAfter in daemonStage.StagesAfter)
                {
                    graph[daemonStage.Name].Add(stageAfter);
                }

                foreach (var stageBefore in daemonStage.StagesBefore)
                {
                    graph[stageBefore].Add(daemonStage.Name);
                }
            }

            var graphWalker = new GraphWalker(graph);
            OrderedDaemonStages = graphWalker.GetSortedGraph().Select(it => stagesByName[it]).ToArray();
        }

    }
}
