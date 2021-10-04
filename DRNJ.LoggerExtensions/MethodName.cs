using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace DRNJ.LoggerExtensions
{
    public class MethodName
    {
        /// <summary>
        /// The calling method frame.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static string CallingMethodFrame(int level)
        {
            StringBuilder signature = new StringBuilder(); ;
            try
            {
                int frame = level + 1;
                StackFrame stackFrame = new StackFrame(frame);

                MethodBase method = stackFrame.GetMethod();

                signature.Append(string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                    (method.DeclaringType != null) ? method.DeclaringType.FullName : "Unknown",
                    method.Name));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                return "Unknown";
            }
            return signature.ToString();
        }
    }
}
