namespace Strazh.Tests.ProjectB.Math
{
    public class MathService : IMathService
    {
        public int Sum(int a, string b)
        {
            return Sum(a, int.TryParse(b, out var x) ? x : 0);
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }

        public int Sum(int a, int b, int c)
        {
            return Sum(Sum(a, b), c);
        }
    }
}
