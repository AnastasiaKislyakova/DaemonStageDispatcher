using System;
using System.Collections.Generic;

namespace DaemonStageDispatcher
{
    public class GraphWalker
    {
        private readonly IDictionary<string, ISet<string>> _graph;

        private readonly IDictionary<string, VisitStatus> _nodeStatus = new Dictionary<string, VisitStatus>();
        private readonly IDictionary<string, string> _parent = new Dictionary<string, string>();
        private string _cycleStart = string.Empty;
        private string _cycleEnd = string.Empty;

        private readonly List<string> _sortedGraph = new List<string>();

        public GraphWalker(IDictionary<string, ISet<string>> graph)
        {
            _graph = graph;
            foreach (var node in graph.Keys)
            {
                _nodeStatus.Add(node, VisitStatus.NotVisited);
            }
        }

        public List<string> GetSortedGraph()
        {

            foreach (var node in _graph)
            {
                var name = node.Key;
                if (_nodeStatus[name] != VisitStatus.NotVisited) continue;

                var foundCycle = Dfs(name);

                if (foundCycle)
                    break;
            }

            if (_cycleStart == string.Empty)
            {
                _sortedGraph.Reverse();
                return _sortedGraph;
            }

            var cycle = new List<string> {_cycleStart};
            for (var v = _cycleEnd; v != _cycleStart; v = _parent[v])
            {
                cycle.Add(v);
            }

            cycle.Add(_cycleStart);
            cycle.Reverse();

            var errorMessage = "Found cycle: " + string.Join("->", cycle);
            throw new StagesCycleException(errorMessage);
        }

        private bool Dfs(string v)
        {
            _nodeStatus[v] = VisitStatus.Visiting;
            foreach (var to in _graph[v])
            {
                switch (_nodeStatus[to])
                {
                    case VisitStatus.NotVisited:
                    {
                        _parent[to] = v;
                        var foundCycle = Dfs(to);
                        if (foundCycle) return true;

                        break;
                    }

                    case VisitStatus.Visiting:
                    {
                        _cycleEnd = v;
                        _cycleStart = to;
                        return true;
                    }

                    case VisitStatus.Visited:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Not found " + to + " in color array");
                }
            }

            _nodeStatus[v] = VisitStatus.Visited;
            _sortedGraph.Add(v);
            return false;
        }


        private enum VisitStatus
        {
            NotVisited,
            Visiting,
            Visited
        }
    }
}
