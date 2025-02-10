using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.IO
{
    public interface IFileHandler
    {
        void WriteCsv(string filename, IList<PowerPeriod> tradeInfo);

        Task WriteCsvAsync(string fileName, IList<PowerPeriod> tradeInfo);

    }
}

