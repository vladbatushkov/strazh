using System;

namespace Fakelib
{
    public interface IDataProvider
    {
        int GetOne();

        int GetTwo();
    }

    public class DataProvider : IDataProvider
    {
        public int GetOne()
            => 1;

        public int GetTwo()
            => 2;
    }

    public class DataConsumer
    {
        private readonly IDataProvider _dataProvider;

        public DataConsumer(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public int Sum()
        {
            var one = _dataProvider.GetOne();
            var two = _dataProvider.GetTwo();
            return one + two;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var dc = new DataConsumer(new DataProvider());
            var result = dc.Sum();
            Console.WriteLine($"Result is {result}");
        }
    }
}
