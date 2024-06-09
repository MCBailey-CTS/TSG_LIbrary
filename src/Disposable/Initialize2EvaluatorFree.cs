using System;
using NXOpen;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Disposable
{
    public class Initialize2EvaluatorFree : IDisposable
    {
        private readonly IntPtr __evaluator;

        public Initialize2EvaluatorFree(Tag tag)
        {
            ufsession_.Eval.Initialize2(tag, out __evaluator);
        }

        public void Dispose()
        {
            ufsession_.Eval.Free(__evaluator);
        }
    }
}