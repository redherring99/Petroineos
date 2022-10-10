using DRNJ.Petro.Components.IO;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.Aggregator
{
    public interface IAggregator
    {
        Task Start(DateTime startTime);
    }


    /// <summary>
    /// Aggregator class
    /// 
    /// Get data from external DLL for passed in DateTime startTime
    /// Aggregate
    /// Write to File with appropriate name
    /// 
    /// </summary>
    public class Aggregator  : IAggregator
    {
        #region Properties/Members
        private readonly IPowerService thePowerService;
        private readonly IFileHandler fileWriter;
        private readonly ILogger Logger;
        private readonly string FilePath;

        #endregion

        #region Constructor

        public Aggregator(ILogger<Aggregator> logger, 
                          IPowerService powerService,
                          IFileHandler fileHandler,
                          string filePath
            )
        {
            this.Logger = logger;

            //----------------------------
            // Store our config          |
            //----------------------------

            this.thePowerService = powerService;
            this.fileWriter = fileHandler;
            this.FilePath = filePath;

        }
        #endregion

        #region Public Methods

        public async Task Start(DateTime startTime)
        {
            this.Logger.LogWarning("Starting Aggregator for Time: {0}", startTime);

            //-------------------------------------
            // Get Data                           |
            //-------------------------------------

            var trades = await this.GetTradesAsync(startTime);

            //-------------------------------------
            // Aggregate the results              |
            //-------------------------------------

            var aggre = this.AggregateResults(trades);

            //-------------------------------------
            // Write results to a file            |
            //-------------------------------------

             this.WriteDataToFile(startTime, aggre);
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Facade to external DLL
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime time)
        {
            this.Logger.LogWarning("Getting Trades for Time: {0}", time);
            return await this.thePowerService.GetTradesAsync(time);
        }

        /// <summary>
        /// Sum up for each period
        /// 
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        protected IList<PowerPeriod> AggregateResults(IEnumerable<PowerTrade> trades)
        {
            this.Logger.LogWarning("Calculating Sums");

            //-----------------------------------------------------------------------
            // Assumes we have 24 periods in each trade. We could do outer join to  |
            // int 1..24 and then sum if this were the case                         |
            //-----------------------------------------------------------------------
            var res = (from g in trades.SelectMany(a => a.Periods).GroupBy(b => b.Period)
                       select new PowerPeriod { Period = g.Key, Volume = g.Sum(a => a.Volume) }).ToList();

            return res;

        }

        protected void WriteDataToFile(DateTime startTime, IList<PowerPeriod> data)
        {
            //------------------------------------------------------
            // Calculate file name                                 |
            // Using string.Format instead of string interpolation |
            //------------------------------------------------------

            string fileName = String.Format("{0}\\PowerPosition_{1:yyyyMMdd_HHmm}.csv", this.FilePath, startTime);

            //------------------------------------------------------
            // Write the data                                      |
            //------------------------------------------------------

            this.Logger.LogWarning("Writing data to file: {n}",fileName); 

            this.fileWriter.WriteCsv(fileName, data);

        }

        protected async Task WriteDataToFileAsync(DateTime startTime, IList<PowerPeriod> data)
        {
            //------------------------------------------------------
            // Calculate file name                                 |
            // Using string.Format instead of string interpolation |
            //------------------------------------------------------

            string fileName = String.Format("{0}\\PowerPosition_{1:yyyyMMdd_HHmm}.csv", this.FilePath, startTime);

            //------------------------------------------------------
            // Write the data                                      |
            //------------------------------------------------------

            this.Logger.LogWarning("Writing data to file: {n}", fileName);

            await this.fileWriter.WriteCsvAsync(fileName, data);

        }

        #endregion

    }
}
