using DRNJ.Petro.Components.IO;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRNJ.LoggerExtensions;

namespace DRNJ.Petro.Components.Aggregator
{
    public interface IAggregator
    {
        Task Start(DateTime startTime);
    }


    /// <summary>
    /// Aggregator class
    /// 
    /// Basically and "outer loop" which fires off tasks to run at specified intervals
    /// which gets the data and adds to List of PowerTrades (with crude/simple lock/unclock
    /// to control concurrency)
    /// 
    /// Awaits all Tasks to have run
    /// Aggregates and writes file
    /// 
    /// Reschedules itself
    /// 
    /// I could aggregate as I go along - but then I'd have to be careful of calculation times and the
    /// next X min Get arriving before the calc had completed
    /// 
    /// As I said in the readme I am no overly happy with the messy way the tasks are fired off
    /// But I have to be pragmatic in the time available
    /// </summary>
    public class Aggregator  : IAggregator
    {
        #region Properties/Members
        private readonly IPowerService thePowerService;
        private readonly IFileHandler fileWriter;
        private readonly ILogger Logger;
        private readonly string FilePath;
        /// <summary>
        /// Stores tasks that have been fired off
        /// </summary>
        public List<Task> powerTasks { get; set; }

        /// <summary>
        /// Results of Gets
        /// </summary>
        public List<PowerTrade> powerTrades { get; set; }

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
            //----------------------------
            // Initialise everything     |
            //----------------------------

            this.powerTrades = new List<PowerTrade>();
            this.powerTasks = new List<Task>();
        }
        #endregion

        #region Public Methods

        public async Task Start(DateTime startTime)
        {
            this.Logger.LogDebugMsg("Starting Aggregator for Time: {0}", startTime);

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

        protected async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime time)
        {
            this.Logger.LogDebugMsg("Getting Trades for Time: {0}", time);
            return await this.thePowerService.GetTradesAsync(time);
        }

        /// <summary>
        /// Sum up for each period
        /// 
        /// TODO: Can probably do something simpler with .GroupBy or .Aggregate
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        protected List<double> AggregateResults(IEnumerable<PowerTrade> trades)
        {
            this.Logger.LogDebugMsg("Calculating Sums ");

            List<double> volume = new List<double>();

            for (int period = 1; period <= 24; period++)
            {
                var totalVol = trades.Sum(a => a.Periods.Where(c => c.Period == period).First().Volume);
                volume.Add(totalVol);
            }

            return volume;
        }

        protected void WriteDataToFile(DateTime startTime, List<double> data)
        {
            //------------------------------------------------------
            // Calculate file name                                 |
            // Using string.Format instead of string interpolation |
            //------------------------------------------------------

            string fileName = String.Format("{0:yyyyMMdd_HHm}.csv", startTime);
            string fullFileName = String.Format("{0}\\{1}", this.FilePath, fileName);

            //------------------------------------------------------
            // Write the data                                      |
            //------------------------------------------------------

            this.Logger.LogDebugMsg("Writing data to file: {n}",fullFileName); 

            this.fileWriter.WriteCsv(fullFileName, data);

        }

        #endregion

    }
}
