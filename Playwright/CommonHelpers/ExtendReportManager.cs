using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using BoDi;
using Microsoft.Playwright;
using System.Diagnostics;
using TechTalk.SpecFlow;

namespace PlaywrightUtils.CommonHelpers
{
    /// <summary>
    /// This class provides Extent Report capabilities
    /// </summary>
    [Binding]
    public class ExtendReportManager
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private static AventStack.ExtentReports.ExtentReports _extentReport;

        private static readonly string ReportPath = Directory.GetParent(Environment.CurrentDirectory).Parent?.FullName
            + Path.DirectorySeparatorChar + "Report" + Path.DirectorySeparatorChar;

        [ThreadStatic]
        private static ExtentTest _feature;

        private ExtentTest _scenario;

        private string _prevStep;

        /// <summary>
        /// Init Extent Report
        /// </summary>
        /// <param name="objectContainer"></param>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        public ExtendReportManager(IObjectContainer objectContainer, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        /// <summary>
        /// Enable Extent Report before test run if EXTENT_REPORT_MODE = true
        /// </summary>
        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRun()
        {
            if (ConfigReader.GetExtentReportMode())
            {
                var path = ReportPath + "index.html";
                Log.Info("Extent report ENABLED. Report path: " + path);

                var htmlReporter = new ExtentV3HtmlReporter(path);
                htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;
                htmlReporter.Config.DocumentTitle = "IRIS Elements TA Report";
                htmlReporter.Config.ReportName = ConfigReader.GetBuildNumber();
                Log.Info("Build Number: " + ConfigReader.GetBuildNumber());

                _extentReport = new AventStack.ExtentReports.ExtentReports();
                _extentReport.AttachReporter(htmlReporter);
            }
        }

        /// <summary>
        /// Create BDD Features in the report.
        /// </summary>
        /// <param name="featureContext"></param>
        [BeforeFeature(Order = 0)]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (ConfigReader.GetExtentReportMode())
            {
                //Create dynamic feature name
                _feature = _extentReport.CreateTest<Feature>(featureContext.FeatureInfo.Title, featureContext.FeatureInfo.Description);
            }
        }

        /// <summary>
        /// Create BDD Test in the report.
        /// </summary>
        [BeforeScenario(Order = 1)]
        public void BeforeScenario()
        {
            string title = _scenarioContext.ScenarioInfo.Title;
            foreach (var exampleKey in _scenarioContext.ScenarioInfo.Arguments.Keys)
            {
                title = title.Replace($"<{exampleKey}>", $"<em>{_scenarioContext.ScenarioInfo.Arguments[exampleKey]}</em>");
            }

            Log.Info($"{Environment.NewLine}----- Scenario '{title}'  with arguments has started -----");

            Stopwatch stopWatch = new Stopwatch();
            _scenarioContext.Add("StepStopWatch", stopWatch);
            stopWatch.Start();

            if (ConfigReader.GetExtentReportMode())
            {
                //Create dynamic test name
                _scenario = _feature.CreateNode<Scenario>(title);
                List<string> allTags = new List<string>();
                allTags.AddRange(_featureContext.FeatureInfo.Tags);
                allTags.AddRange(_scenarioContext.ScenarioInfo.Tags);
                _scenario.AssignCategory(allTags.ToArray());
            }
        }

        /// <summary>
        /// Init test step duration timer.
        /// </summary>
        [BeforeStep]
        public void BeforeStep()
        {
            Stopwatch stopWatch = _scenarioContext.Get<Stopwatch>("StepStopWatch");
            stopWatch.Reset();
            stopWatch.Start();
        }

        /// <summary>
        /// Record executed step in the report. Add screenshots, failure message, step duration and etc.
        /// </summary>
        [AfterStep]
        public async Task AfterStep()
        {
            string stepTitle = _scenarioContext.StepContext.StepInfo.Text;
            Log.Info($"{Environment.NewLine}----- Scenario '{_scenarioContext.ScenarioInfo.Title}' step '{stepTitle}' ended -----");
            Stopwatch stopWatch = _scenarioContext.Get<Stopwatch>("StepStopWatch");
            stopWatch.Stop();

            var tags = _featureContext.FeatureInfo.Tags;
            var uiTag = Array.Find(tags, s => s.Equals("UI"));

            if (ConfigReader.GetExtentReportMode())
            {
                var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();

                long elapsedTime = stopWatch.ElapsedMilliseconds;

                string stepStart = (!string.IsNullOrEmpty(stepType) && stepType.Equals(_prevStep)) ? "&nbsp;&nbsp;&nbsp;&nbsp; <b>And</b> " : $"<b>{stepType}</b> ";

                var stepToReport = _scenario.CreateNode(new GherkinKeyword(stepType),
                                                      $"{stepStart} {stepTitle} <span class='duration right label'> {elapsedTime} ms. </span>");

                if (_scenarioContext.StepContext.StepInfo.Table != null)
                {
                    var tableData = PrepareTableDataForReport(_scenarioContext.StepContext.StepInfo.Table);
                    try
                    {
                        stepToReport.Info(tableData);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Exception occured when step definition data table was logged into the report!");
                    }
                }

                if (ShouldAddScreenShot(uiTag))
                {
                    string filename = await TakeScreenshot();

                    stepToReport.AddScreenCaptureFromPath(filename);
                }

                if (_scenarioContext.TestError != null)
                {
                    if (ConfigReader.GetCaptureBrowserLogs()) stepToReport.Log(Status.Info, $"Captured browser logs : <a href='{CaptureBrowserLogs()}'>LINK</a>");

                    stepToReport.Fail("<br />" + _scenarioContext.TestError.Message.Replace("\r\n", "<br />"));
                }

                if (!string.IsNullOrEmpty(stepType)) _prevStep = stepType;
            }
        }

        private static IMarkup PrepareTableDataForReport(Table table)
        {
            int rowsCount = table.Rows.Count + 1;
            string[][] data = new string[rowsCount][];
            data[0] = table.Header.ToArray();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                data[i + 1] = table.Rows[i].Values.ToArray();
            }

            return MarkupHelper.CreateTable(data);
        }

        private bool ShouldAddScreenShot(string screenShotTags)
        {
            return (ConfigReader.GetScreenshotOnEachStep() || _scenarioContext.TestError != null)
                    && !string.IsNullOrEmpty(screenShotTags);
        }

        /// <summary>
        /// Log scenario completion in the logs.
        /// </summary>
        [AfterScenario(Order = 1)]
        public void AfterScenario()
        {
            Log.Info($"{Environment.NewLine}----- Scenario '{_scenarioContext.ScenarioInfo.Title}' has finished -----");
            _scenarioContext.Remove("StepStopWatch");
        }

        /// <summary>
        /// After test run create the generated HTML report.
        /// </summary>
        [AfterTestRun(Order = 0)]
        public static void AfterTestRun()
        {
            if (ConfigReader.GetExtentReportMode())
            {
                //Flush report once whole test completes
                _extentReport.Flush();
            }
        }

        /// <summary>
        /// Takes web browser screenshot.
        /// </summary>
        /// <returns>Filename of the screenshot</returns>
        private async Task<string> TakeScreenshot()
        {
            var scenarioTitle = string.Concat(_scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{DateTime.Now.ToFileTime()}_{scenarioTitle}.png";
            try
            {
                IBrowser browser = _objectContainer.Resolve<IBrowser>();
                if (browser != null)
                {
                    var path = ReportPath + fileName;
                    var page = browser.Contexts.First().Pages.LastOrDefault();

                    if (page != null)
                    {
                        await page.ScreenshotAsync(new()
                        {
                            Path = path,
                            FullPage = false,
                        });
                    }
                }
            }
            catch (ObjectContainerException e)
            {
                Log.Warn("Empty screenshot will be attached to the report as cannot resolve the WebDriver from context.");
                Log.Warn(e.Message);
            }
            catch (Exception e)
            {
                Log.Error("Error occurred during Screenshot capture!");
                Log.Error(e.ToString());
            }

            return fileName;
        }

        private async Task<string> CaptureBrowserLogs()
        {
            var scenarioTitle = string.Concat(_scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{DateTime.Now.ToFileTime()}_{scenarioTitle}.txt";
            var path = ReportPath + fileName;

            try
            {
                IBrowser browser = _objectContainer.Resolve<IBrowser>();
                if (browser != null)
                {
                    var logs = await browser.Contexts.First().WaitForConsoleMessageAsync();
                    var logEntries = new List<string>()
                    {
                        logs.Location,
                        logs.Text
                    };
                    IList<string> strings = new List<string>();
                    foreach (var logEntry in logEntries)
                    {
                        strings.Add(logEntry.ToString());
                    }

                    File.WriteAllLines(path, strings);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error occurred during Browser log capture!");
                Log.Error(e.ToString());
            }

            return fileName;
        }
    }
}
