using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Components.IO
{

    public interface IFileHandler
    {
        void WriteCsv(string filename, IList<PowerPeriod> tradeInfo);

        Task WriteCsvAsync(string fileName, IList<PowerPeriod> tradeInfo);

    }

    /// <summary>
    /// Wrapper for Static System.IO stuff so we can test
    /// 
    /// 
    /// TODO - Inject wrapped System.IO streamwriter + Unit Test what's sent to the file
    /// </summary>
    public class FileHandler : IFileHandler
    {
        #region Members

        private readonly ILogger Logger;
        private readonly IStreamWrapper StreamWrapper;

        #endregion

        #region Constructor
        public FileHandler(ILogger<FileHandler> logger, IStreamWrapper streamWrapper)
        {
            this.Logger = logger;
            this.StreamWrapper = streamWrapper;
        }
        #endregion

        #region Public Methods

        public void WriteCsv(string fileName, IList<PowerPeriod> tradeInfo)
        {
            //---------------------------------
            // Order the data                 |
            //---------------------------------

            var orderedData = tradeInfo.OrderBy(a => a.Period);
            //---------------------------------
            // Open File                      |
            //---------------------------------

            StreamWrapper.OpenFile(fileName);

            //---------------------------------
            // Write header                   |      
            //---------------------------------

            StreamWrapper.WriteLine("Local Time, Volume");
            this.Logger.LogWarning("Local Time, Volume");

            //----------------------------------
            // Write Data - assumption is that |
            // list is in "hour" order         |
            //----------------------------------

            foreach (var item in orderedData)
            {
                var data = string.Format("{0},{1}", this.CreateLocalTimeString(item.Period), item.Volume);
                this.Logger.LogWarning(data);
                StreamWrapper.WriteLine(data);
            }

            //---------------------------------
            // Close + Dispose of stream      |
            //---------------------------------
            StreamWrapper.CloseFile();

        }


        public async Task WriteCsvAsync(string fileName, IList<PowerPeriod> tradeInfo)
        {
            //---------------------------------
            // Order the data                 |
            //---------------------------------

            var orderedData = tradeInfo.OrderBy(a => a.Period);
            //---------------------------------
            // Open File                      |
            //---------------------------------

            StreamWrapper.OpenFile(fileName);

            //---------------------------------
            // Write header                   |      
            //---------------------------------

            await StreamWrapper.WriteLineAsync("Local Time, Volume");
            this.Logger.LogWarning("Local Time, Volume");

            //----------------------------------
            // Write Data - assumption is that |
            // list is in "hour" order         |
            //----------------------------------

            foreach (var item in orderedData)
            {
                var data = string.Format("{0},{1}", this.CreateLocalTimeString(item.Period), item.Volume);
                this.Logger.LogWarning(data);
                await StreamWrapper.WriteLineAsync(data);
            }

            //---------------------------------
            // Close + Dispose of stream      |
            //---------------------------------
            StreamWrapper.CloseFile();

        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Create string of time from period
        /// Done as separate method so can modify if need to change time format in the future
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        protected string CreateLocalTimeString(int period)
        {
            return period == 1 ? "23:00" : string.Format("{0:00}:00", period - 2);
        }
        #endregion
    }
}

