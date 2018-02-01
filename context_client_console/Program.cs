using System;
using context_client;
using context_client.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace context_client_console
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new ImportTestClass();

            Console.WriteLine("TestClass.Add(a, b) = {0}", test.Add(10, 20));
            
            Console.WriteLine("Hello World!");

            ContextSimpleTest();
            ContextSimpleTestAsync().GetAwaiter().GetResult();

			ContextMultipleResultSets();

        }

        static void ContextSimpleTest()
        {
            var context = new ServiceContentsContext();
            foreach (var elem in context.GetAll())
            {
                Console.WriteLine("sync: {0}", elem.ToString());
            }
		}

        static async Task ContextSimpleTestAsync()
        {
            var context = new ServiceContentsContext();
            foreach (var elem in await context.GetAllAsync())
            {
                Console.WriteLine("async: {0}", elem.ToString());
            }
        }

		static void ContextMultipleResultSets()
		{
			var context = new ServiceContentsContext();
			context.GetMultipleResultSets();
		}

        static async Task<int> AsyncFunction()
        {
            // Task.Delay(5);
            await Task.Delay(5000);
            return 10;
            /*
            return new Task<int>(() => {
                Task.Delay(5);
                return 10;
            });
            */
        }

    }
}
