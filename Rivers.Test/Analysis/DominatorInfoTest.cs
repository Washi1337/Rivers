using System.Collections.Generic;
using Rivers.Analysis;
using Xunit;

namespace Rivers.Test.Analysis
{
    public class DominatorInfoTest
    {
        private static readonly Graph IfStatementGraph;
        private static readonly Graph LoopGraph;

        static DominatorInfoTest()
        {
            IfStatementGraph = new Graph();
            
            IfStatementGraph.Nodes.Add("1");
            IfStatementGraph.Nodes.Add("2");
            IfStatementGraph.Nodes.Add("3");
            IfStatementGraph.Nodes.Add("4");

            IfStatementGraph.Edges.Add("1", "2");
            IfStatementGraph.Edges.Add("1", "3");
            IfStatementGraph.Edges.Add("2", "4");
            IfStatementGraph.Edges.Add("3", "4");
            
            LoopGraph = new Graph();
            
            LoopGraph.Nodes.Add("1");
            LoopGraph.Nodes.Add("2");
            LoopGraph.Nodes.Add("3");
            LoopGraph.Nodes.Add("4");

            LoopGraph.Edges.Add("1", "2");
            LoopGraph.Edges.Add("2", "3");
            LoopGraph.Edges.Add("3", "2");
            LoopGraph.Edges.Add("2", "4");
        }

        [Fact]
        public void IfStatementDominators()
        {
            var cfg = IfStatementGraph;
            
            var info = new DominatorInfo(cfg.Nodes["1"]);
            Assert.Equal(cfg.Nodes["1"], info.GetImmediateDominator(cfg.Nodes["1"]));
            Assert.Equal(cfg.Nodes["1"], info.GetImmediateDominator(cfg.Nodes["2"]));
            Assert.Equal(cfg.Nodes["1"], info.GetImmediateDominator(cfg.Nodes["3"]));
            Assert.Equal(cfg.Nodes["1"], info.GetImmediateDominator(cfg.Nodes["4"]));
        }

        [Fact]
        public void IfStatementFrontiers()
        {
            var cfg = IfStatementGraph;
            var info = new DominatorInfo(IfStatementGraph.Nodes["1"]);
            var frontier = new HashSet<Node>(new[] {IfStatementGraph.Nodes["4"]});
            Assert.Equal(frontier, info.GetDominanceFrontier(IfStatementGraph.Nodes["3"]));
            Assert.Equal(frontier, info.GetDominanceFrontier(IfStatementGraph.Nodes["3"]));
        }

        [Fact]
        public void IfStatementDominated()
        {
            var cfg = IfStatementGraph;
            var info = new DominatorInfo(IfStatementGraph.Nodes["1"]);
            Assert.True(info.GetDominatedNodes(cfg.Nodes["1"]).SetEquals(cfg.Nodes));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["2"]).SetEquals(new[] {cfg.Nodes["2"]}));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["3"]).SetEquals(new[] {cfg.Nodes["3"]}));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["4"]).SetEquals(new[] {cfg.Nodes["4"]}));
        }
        
        [Fact]
        public void LoopDominators()
        {
            var info = new DominatorInfo(LoopGraph.Nodes["1"]);
            Assert.Equal(LoopGraph.Nodes["1"], info.GetImmediateDominator(LoopGraph.Nodes["1"]));
            Assert.Equal(LoopGraph.Nodes["1"], info.GetImmediateDominator(LoopGraph.Nodes["2"]));
            Assert.Equal(LoopGraph.Nodes["2"], info.GetImmediateDominator(LoopGraph.Nodes["3"]));
            Assert.Equal(LoopGraph.Nodes["2"], info.GetImmediateDominator(LoopGraph.Nodes["4"]));
        }

        [Fact]
        public void LoopFrontiers()
        {
            var info = new DominatorInfo(LoopGraph.Nodes["1"]);
            var frontier = new HashSet<Node>(new[] {LoopGraph.Nodes["2"]});
            Assert.Equal(frontier, info.GetDominanceFrontier(LoopGraph.Nodes["2"]));
            Assert.Equal(frontier, info.GetDominanceFrontier(LoopGraph.Nodes["3"]));
        }

        [Fact]
        public void LoopDominated()
        {
            var cfg = LoopGraph;
            var info = new DominatorInfo(LoopGraph.Nodes["1"]);
            Assert.True(info.GetDominatedNodes(cfg.Nodes["1"]).SetEquals(cfg.Nodes));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["2"]).SetEquals(new[] {cfg.Nodes["2"], cfg.Nodes["3"], cfg.Nodes["4"]}));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["3"]).SetEquals(new[] {cfg.Nodes["3"]}));
            Assert.True(info.GetDominatedNodes(cfg.Nodes["4"]).SetEquals(new[] {cfg.Nodes["4"]}));
        }
    }
}