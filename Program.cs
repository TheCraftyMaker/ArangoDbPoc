using System;
using System.Threading.Tasks;

namespace ArangoDbPoc
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await ChooseFlow().StartFlow();

            Console.ReadLine();
        }

        private static Flow ChooseFlow()
        {
            Console.WriteLine("Choose Arango DB POC Flow:");
            Console.WriteLine("1: Simple Document Flow");
            Console.WriteLine("2: Simple Graph Traversal Flow");
            Console.WriteLine("3: Complex Graph Traversal Flow");

            var value = Console.ReadLine();

            if (int.TryParse(value, out var flowId))
            {
                switch (flowId)
                {
                    case 1:
                        return new SimpleDocument();
                    case 2:
                        return new SimpleGraphTraversal();
                    case 3:
                        return new ComplexGraphTraversal();
                }
            }

            Console.WriteLine("Invalid value!");
            Console.WriteLine();

            return ChooseFlow();
        }
    }
}
