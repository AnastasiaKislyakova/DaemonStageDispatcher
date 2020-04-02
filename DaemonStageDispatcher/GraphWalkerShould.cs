using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace DaemonStageDispatcher
{
    public class GraphWalkerShould
    {
        private const string A = "A";
        private const string B = "B";
        private const string C = "C";
        private const string D = "D";
        private const string E = "E";

        [Test]
        public void Should_sort_directed_acyclic_graph()
        {
            var graph = GetDirectedAcyclicGraph();
            var graphWalker = new GraphWalker(graph);
            var sortedNodes = graphWalker.GetSortedGraph().ToArray();
            AssertGraphNodes(new[] {A, B, C, D, E}, sortedNodes);
        }

        [Test]
        public void Should_sort_complete_acyclic_graph()
        {
            var graph = GetCompleteAcyclicGraph();
            var graphWalker = new GraphWalker(graph);
            var sortedNodes = graphWalker.GetSortedGraph().ToArray();
            AssertGraphNodes(new[] {A, B, D, C}, sortedNodes);
        }

        [Test]
        public void Should_throw_exception_if_found_cycle_in_complete_cyclic_graph()
        {
            var graph = GetCompleteCyclicGraph();
            var graphWalker = new GraphWalker(graph);
            var exception = Assert.Catch<StagesCycleException>(() =>
            {
                var sortedNodes = graphWalker.GetSortedGraph().ToArray();
            });
        }

        [Test]
        public void Should_throw_exception_if_found_cycle_in_cycle_graph()
        {
            var graph = GetCycleGraph();
            var graphWalker = new GraphWalker(graph);
            var exception = Assert.Catch<StagesCycleException>(() =>
            {
                var sortedNodes = graphWalker.GetSortedGraph().ToArray();
            });
        }

        [Test]
        public void Should_sort_star_graph()
        {
            var graph = GetStarGraph();
            var graphWalker = new GraphWalker(graph);
            var sortedNodes = graphWalker.GetSortedGraph().ToArray();

            sortedNodes.Length.Should().Be(graph.Count);
            sortedNodes.Should().BeEquivalentTo(graph.Select(it => it.Key));
            sortedNodes.Last().Should().Be(A);
        }

        [Test]
        public void Should_sort_path_graph()
        {
            var graph = GetPathGraph();
            var graphWalker = new GraphWalker(graph);
            var sortedNodes = graphWalker.GetSortedGraph().ToArray();
            AssertGraphNodes(new[] {A, B, C, D, E}, sortedNodes);
        }

        private Dictionary<string, ISet<string>> GetDirectedAcyclicGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                {A, new HashSet<string> {C, D, E, B}},
                {B, new HashSet<string> {D, E}},
                {C, new HashSet<string> {D, E}},
                {D, new HashSet<string> {E}},
                {E, new HashSet<string>()}
            };
            return graph;
        }

        private Dictionary<string, ISet<string>> GetCompleteAcyclicGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                {A, new HashSet<string> {B, C, D}},
                {B, new HashSet<string> {C, D}},
                {C, new HashSet<string>()},
                {D, new HashSet<string> {C}}
            };
            return graph;
        }

        private Dictionary<string, ISet<string>> GetCompleteCyclicGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                {A, new HashSet<string> {B, C}},
                {B, new HashSet<string> {C}},
                {C, new HashSet<string> {D}},
                {D, new HashSet<string> {A, B}}
            };
            return graph;
        }

        private Dictionary<string, ISet<string>> GetCycleGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                {A, new HashSet<string> {B}},
                {B, new HashSet<string> {C}},
                {C, new HashSet<string> {D}},
                {D, new HashSet<string> {A}}
            };
            return graph;
        }

        private Dictionary<string, ISet<string>> GetStarGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                {A, new HashSet<string>()},
                {B, new HashSet<string> {A}},
                {C, new HashSet<string> {A}},
                {D, new HashSet<string> {A}},
                {E, new HashSet<string> {A}}
            };
            return graph;
        }

        private Dictionary<string, ISet<string>> GetPathGraph()
        {
            var graph = new Dictionary<string, ISet<string>>
            {
                { A, new HashSet<string> { B } },
                { B, new HashSet<string> { C } },
                { C, new HashSet<string> { D } },
                { D, new HashSet<string> {E} },
                { E, new HashSet<string>() }
            };
            return graph;
        }

        private void AssertGraphNodes(string[] expected, string[] received)
        {
            received.Should().Equal(expected);
            received.Length.Should().Be(expected.Length);
        }
    }
}