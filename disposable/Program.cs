using System;

namespace disposable
{
    public class SampleDisposable : IDisposable
    {
        public SampleDisposable()
        {
            Console.WriteLine("Constructor is called.");
        }

        public void Throw()
        {
			throw new Exception();
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose is called.");
        }

    }

    public class Testcase
    {
        public Testcase()
        {
        }

        public void Test_Using()
        {
			using (var a = new SampleDisposable())
			{
                a.Throw();
			}
        }

        public void Test_UsingTryCatch()
        {
            using (var a = new SampleDisposable())
            {
                try
                {
                    a.Throw();
                }
                catch
                {
                }
            }
        }

        public void Test_TryCatch()
        {
            try
            {
                var a = new SampleDisposable();
                a.Throw();
            }
            catch
            {
            }   
        }

        public void Test_TryCatchUsing()
        {
            try
            {
                using(var a = new SampleDisposable())
                {
                    a.Throw();
                }
            }
            catch
            {
            }
        }

        public void Test_EqualNull()
        {
            var a = new SampleDisposable();
            a = null;
        }

        public void Test_Equal()
        {
            var a = new SampleDisposable();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var test = new Testcase();
            test.Test_Equal();
            test.Test_EqualNull(); 
            test.Test_Using();
            test.Test_UsingTryCatch();
            test.Test_TryCatch();
            test.Test_TryCatchUsing();
        }
    }
}
