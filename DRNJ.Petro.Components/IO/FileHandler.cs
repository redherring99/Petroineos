using DRNJ.LoggerExtensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DRNJ.Petro.Components.IO
{

    public interface IFileHandler
    {
        void WriteCsv(string filename, List<double> tradeInfo);

    }

    /// <summary>
    /// Wrapper for Static System.IO stuff so we can test
    /// 
    /// Given more time I would inject System.IO wrapped in a facade and an Interface 
    /// so that I could test the inneer logic of this class but time does not permit
    /// - I have a pending hospital operation !!!!
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

        public void WriteCsv(string fileName, List<double> tradeInfo)
        {
            //---------------------------------
            // Open File                      |
            //---------------------------------

            StreamWrapper.OpenFile(fileName);

            //---------------------------------
            // Write header                   |      
            //---------------------------------

            StreamWrapper.WriteLine("Local Time, Volume");
            Logger.LogDebugMsg("Local Time, Volume");

            //----------------------------------
            // Write Data - assumption is that |
            // list is in "hour" order         |
            //----------------------------------

            for (int period = 0; period < 24; period++)
            {
                var data = string.Format("{0},{1}", this.CreateLocalTimeString(period), tradeInfo[period]);
                Logger.LogDebugMsg(data);
                StreamWrapper.WriteLine(data);
            }

            //---------------------------------
            // Close + Dispose of stream      |
            //---------------------------------
            StreamWrapper.CloseFile();

        }


        public void WriteCsvOrig(string fileName, List<double> tradeInfo)
        {
            using (var Writer = new StreamWriter(fileName))
            {
                //--------------------
                // Write header      |      
                //--------------------

                Writer.WriteLine("Local Time, Volume");
                Logger.LogDebugMsg("Local Time, Volume");
                //----------------------------------
                // Write Data - assumption is that |
                // list is in "hour" order         |
                //----------------------------------

                for (int period = 0; period < 24; period++)
                {
                    var data = string.Format("{0}, {1}", this.CreateLocalTimeString(period), tradeInfo[period]);
                    Logger.LogDebugMsg(data);
                    Writer.WriteLine(data);
                }
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Create string of time from period
        /// Not particularly elegant
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        protected string CreateLocalTimeString(int period)
        {
            if (period == 0)
            {
                return "23:00";
            }

            return string.Format("{0:00}:00", period - 1);

        }
        #endregion
    }
}

