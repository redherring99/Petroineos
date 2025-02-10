using System;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.Aggregate
{
    public interface IAggregator
    {
        Task Start(DateTime startTime);
    }
}