using DRNJ.Petro.Components.Aggregator;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components
{
    public interface IExtractLauncher
    {
        Task Start(int pollInterval);
    }


    /// <summary>
    /// "Manager" for launching extract/aggregator
    /// </summary>
    public class ExtractLauncher : IExtractLauncher
    {
        #region Members

        private readonly ILogger Logger;

        private readonly IAggregator tradeAggregator;

        /// <summary>
        /// Allow external tasks to stop loop running - not sure how useful this is for the moment
        /// </summary>
        public bool Running { get; set; }

        #endregion

        #region Constructor

        public ExtractLauncher(ILogger<ExtractLauncher> logger, IAggregator aggregator, int pollInterval)
        {
            this.Logger = logger;
            this.tradeAggregator = aggregator;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// ------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------
        /// Fire off the extract
        /// 
        /// Here I use a simple loop where I run the task to do the extract then 
        /// Wait for poll interval and then repeat
        /// 
        /// I'm sure that there are far more elegant ways of doing this with Timers,
        /// multi threadingTPL etc.
        /// 
        /// -------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------
        /// </summary>
        public async Task Start(int pollInterval)
        {
            this.Running = true;


            while (this.Running)
            {
                //--------------------------------------
                // Fire task to get and aggregate data |
                //--------------------------------------
                await this.tradeAggregator.Start(DateTime.Now);

                //-------------------------------------
                // Poll Interval is in minutes        |
                //-------------------------------------
                await Task.Delay(pollInterval * 60 * 1000);
            }
        }

        #endregion


    }
}
