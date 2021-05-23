using Strazh.Tests.ProjectB.Math;
using System;

namespace Strazh.Tests.ProjectA
{
    public class MathConsumer
    {
        public readonly IMathService _mathService;

        public MathConsumer(IMathService mathService)
        {
            _mathService = mathService;
        }

        public int SumOfSum(int a, int b, int c)
        {
            var sumAb = _mathService.Sum(a, b);
            var sumBc = _mathService.Sum(b, c);
            var sumAc = _mathService.Sum(a, c);
            return _mathService.Sum(sumAb, sumBc, sumAc);
        }

        public int SumOfStrings(string str1, string str2)
        {
            return _mathService.Sum(_mathService.Sum(0, str1), str2);
        }
    }
}
