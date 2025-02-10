using DRNJ.Petro.Components.Error;
using DRNJ.Petro.Components.IO;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.Aggregate
{
    /// <summary>
    /// Aggregator class
    /// 
    /// Get data from external DLL for passed in DateTime startTime
    /// Aggregate
    /// Write to File with appropriate name
    /// 
    /// Throttles Logging in case of repeated errors
    /// 
    /// TODO - Implement event throttling for all logging to aboid
    /// infinite look error message storm
    /// 
    /// </summary>
    public class Aggregator : IAggregator
    {
        #region Properties/Members
        private readonly IPowerService thePowerService;
        private readonly IFileHandler fileWriter;
        private readonly ILogger Logger;
        private readonly string FilePath;

        protected ISubject<ErrorMessage> ErrorMessageThrottleQueue { get; set; }


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

        #region Event Log Throttling

        //***************************************************
        // Add to config file to make configurable 
        //***************************************************
        public int EventWindow = 60;
        public int EventLimit = 10;

        /// <summary>
        /// The configure event throttling.
        /// </summary>
        protected void ConfigureEventThrottling()
        {
            //-------------------------------------
            // Set up RX Error Message Throttling |
            //-------------------------------------

            this.ErrorMessageThrottleQueue = new Subject<ErrorMessage>();

            //----------------------------------------------------------------------------------------------------------------
            // Get first event immediately then wait and ignore subsequent for time span                                     |
            // https://stackoverflow.com/questions/7999503/rx-how-can-i-respond-immediately-and-throttle-subsequent-requests |
            //----------------------------------------------------------------------------------------------------------------
            this.ErrorMessageThrottleQueue.Window(() => { return Observable.Interval(TimeSpan.FromSeconds(this.EventWindow)); })
                .SelectMany(x => x.Take(this.EventLimit))
                .Subscribe(e => this.DispatchError(e));
        }


        /// <summary>
        /// The send error.
        /// Pump message into Rx event-throttlign queue
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void SendError(string message, Exception ex)
        {
            var errorMessage = new ErrorMessage();
            errorMessage.Message = message;
            errorMessage.Exception = ex;

            this.ErrorMessageThrottleQueue.OnNext(errorMessage);
        }


        /// <summary>
        /// Actually Send error to log
        /// </summary>
        /// <param name="error"></param>
        private void DispatchError(ErrorMessage error)
        {
            this.Logger.LogError(error.Exception, error.Message);
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Aggregator main method
        /// Gets data, processes it
        /// Reruns on exception as requirement is time period cannot be missed
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public async Task Start(DateTime startTime)
        {
            bool running = true;
            this.ConfigureEventThrottling();
            //-----------------------
            // Try/Catch and replay |
            //-----------------------
            while (running)
            {
                try
                {
                    await this.ProcessData(startTime);
                    running = false; // Done
                }
                catch (Exception ex)
                {
                    //-------------------------------------
                    // Log Error - but don't create storm |
                    //-------------------------------------
                    this.SendError( ex.Message,ex);
                }
            }
        }


        /// <summary>
        /// Process the data
        ///     Get Data
        ///     Aggregate
        ///     Write to File
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public async Task ProcessData(DateTime startTime)
        {
            this.Logger.LogInformation("Starting Extract for Time: {0}", startTime);

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

            await this.WriteDataToFileAsync(startTime, aggre);

            this.Logger.LogInformation("Extract for Time: {0} Complete", startTime);
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
            this.Logger.LogInformation("Getting Trades for Time: {0}", time);
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

        /// <summary>
        /// Write data to CSV File
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="data"></param>
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

            this.Logger.LogWarning("Writing data to file: {n}", fileName);

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
