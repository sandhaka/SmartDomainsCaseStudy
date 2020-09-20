using System.Collections.Generic;
using Ai.Infrastructure.Search.Graph;
using TransportFleet.UseCase.AiModels;

namespace TransportFleet.UseCase.DemoData
{
    public class NorthItalyMap : UndirectedGraph<StringAim>
    {
        // North Italy example map
        private static readonly List<GraphNode<StringAim>> GraphData =
            new List<GraphNode<StringAim>>
            {
                new GraphNode<StringAim>("MILANO", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("MONZA", 26),
                        new GraphNode<StringAim>("TREVIGLIO", 64),
                        new GraphNode<StringAim>("CREMA", 46),
                        new GraphNode<StringAim>("PIACENZA", 67),
                        new GraphNode<StringAim>("ROZZANO", 13)
                    }
                },
                new GraphNode<StringAim>("MONZA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("BERGAMO", 45)
                    }
                },
                new GraphNode<StringAim>("ROZZANO", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("PAVIA", 33)
                    }
                },
                new GraphNode<StringAim>("TREVIGLIO", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("BRESCIA", 57)
                    }
                },
                new GraphNode<StringAim>("CREMA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("CREMONA", 42)
                    }
                },
                new GraphNode<StringAim>("PIACENZA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("CREMONA", 43)
                    }
                },
                new GraphNode<StringAim>("BRESCIA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("VERONA", 74),
                        new GraphNode<StringAim>("MANTUA", 101)
                    }
                },
                new GraphNode<StringAim>("CREMONA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("MANTUA", 72)
                    }
                },
                new GraphNode<StringAim>("VERONA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("VICENZA", 58),
                        new GraphNode<StringAim>("PADOVA", 88),
                        new GraphNode<StringAim>("MANTUA", 48)
                    }
                },
                new GraphNode<StringAim>("VICENZA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("TREVISO", 58),
                        new GraphNode<StringAim>("VENEZIA", 70)
                    }
                },
                new GraphNode<StringAim>("PADOVA", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("VENEZIA", 39)
                    }
                },
                new GraphNode<StringAim>("TREVISO", 0)
                {
                    Neighbors =
                    {
                        new GraphNode<StringAim>("VENEZIA", 41)
                    }
                }
            };

        public NorthItalyMap() : base(GraphData)
        {

        }
    }
}