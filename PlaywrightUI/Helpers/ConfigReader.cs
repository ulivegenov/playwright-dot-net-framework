using System.Net;

namespace PlaywrightUI.Helpers
{
    internal class ConfigReader
    {
        public static string GetCurrentEnv()
        {
            return Environment.GetEnvironmentVariable("CURRENT_ENV");
        }


        public static string GetBuildNumber()
        {
            if (Environment.GetEnvironmentVariable("BUILD_NUMBER") is null)
            {
                return Dns.GetHostName();
            }
            else
            {
                return Environment.GetEnvironmentVariable("BUILDBRANCH") + "_" + Environment.GetEnvironmentVariable("BUILD_NUMBER");
            }
        }

        public static bool GetExtentReportMode()
        {
            return Environment.GetEnvironmentVariable("EXTENT_REPORT_MODE") is null || Convert.ToBoolean(Environment.GetEnvironmentVariable("EXTENT_REPORT_MODE"));
        }

        /// <summary>
        /// Set enviroment variable 'EXTENT_REPORT_MODE' to true or false.
        /// </summary>
        /// <param name="value">true or false</param>
        public static void SetExtentReportMode(bool value)
        {
            Environment.SetEnvironmentVariable("EXTENT_REPORT_MODE", value.ToString());
        }

        /// <summary>
        /// Get Screenshot On Each Step status (true/false).
        /// </summary>
        /// <returns></returns>
        public static bool GetScreenshotOnEachStep()
        {
            return Convert.ToBoolean(Environment.GetEnvironmentVariable("SCREENSHOT_ON_EACHSTEP"));
        }

        /// <summary>
        /// Sets SCREENSHOT_ON_EACHSTEP (if we want to take screenshots on each UI step).
        /// </summary>
        /// <param name="value">True/False</param>
        public static void SetScreenshotOnEachStep(bool value)
        {
            Environment.SetEnvironmentVariable("SCREENSHOT_ON_EACHSTEP", value.ToString());
        }

        /// <summary>
        /// Check if BrowserLogs need to be captured after test failure.
        /// </summary>
        /// <returns>True if 'CAPTURE_BROWSER_LOGS' environment variable is set to true, false otherwise.</returns>
        public static bool GetCaptureBrowserLogs()
        {
            return Convert.ToBoolean(Environment.GetEnvironmentVariable("CAPTURE_BROWSER_LOGS"));
        }

        /// <summary>
        /// Save browser logs after each scenario failure and add it to the HTML report.
        /// !!! It will create a lot of text files, do not use it unless it is needed for debugging.
        /// </summary>
        /// <param name="value">True/False</param>
        public static void SetCaptureBrowserLogs(bool value)
        {
            Environment.SetEnvironmentVariable("CAPTURE_BROWSER_LOGS", value.ToString());
        }
    }
}
