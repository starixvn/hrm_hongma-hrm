// --------------------------------------------------------------------------
// <copyright file="JobService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.8">Created</change>
// </history>

namespace AutoJobs
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Timers;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using log4net;

    /// <summary>
    /// Job Service.
    /// </summary>
    public partial class JobService : ServiceBase
    {
        /// <summary>
        /// Default interval for Ticker is 300 seconds.
        /// </summary>
        private const int _DEFAULT_TIMER_INTERVAL = 300;

        private static readonly ILog _LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly EmailService _EmailService = new EmailService();
        private static readonly EmployeeRepository _EmployeeRepository = new EmployeeRepository();
        private static readonly TimeRepository _Respository = new TimeRepository();
        
        private static readonly string _SenderEmail = ConfigurationManager.AppSettings["Email.Sender"].ToString();
        private static readonly string _SenderName = ConfigurationManager.AppSettings["Email.SenderName"].ToString();
        private static readonly string _TemplateEmail = ConfigurationManager.AppSettings["EmailTemplate"].ToString();
        //// Do not send email forget check in out for these employees
        private static readonly List<string> _IgnoreList = new List<string> { "sean", "joe" };
        private Timer _updateCounterTimer = null;
        private bool _isBusy = false;
        private bool _checkSendMailCheckInOut = false;

        public JobService()
        {
            InitializeComponent();
            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();
            int timerInterval = _DEFAULT_TIMER_INTERVAL;
            int.TryParse(ConfigurationManager.AppSettings["TimerInterval"], out timerInterval);
            TimerInterval = timerInterval;
            _updateCounterTimer = new Timer(TimerInterval * 1000);
            _updateCounterTimer.Elapsed += new ElapsedEventHandler(ExecuteJob);
            _updateCounterTimer.Enabled = false;
        }

        public int TimerInterval { get; private set; }

        /// <summary>
        /// Function to Execute Job.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Elapsed EventArgs.</param>
        public void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_isBusy)
                {
                    _isBusy = true;

                    _LOG.Info("Import Time Record started at " + DateTime.Now.ToString());
                    _Respository.ImportTimeRecord();
                    _LOG.Info("Import Time Record stopped at " + DateTime.Now.ToString());

                    var currentDateTime = DateTime.Now;

                    if (currentDateTime.Hour > 9)
                    {
                        if (!_checkSendMailCheckInOut)
                        {
                            _LOG.Info("CheckInOut Reminder started at " + DateTime.Now.ToString());
                            CheckInOutReminder();
                            _LOG.Info("CheckInOut Reminder stopped at " + DateTime.Now.ToString());
                            _checkSendMailCheckInOut = true;
                        }
                    }
                    else
                    {
                        _checkSendMailCheckInOut = false;
                    }
                    
                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                _LOG.Error("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Function to send mail to reminder missing check in out.
        /// </summary>
        public void CheckInOutReminder()
        {
            try
            {
                // Get employee who forgot check in or check out
                var employeeList = _EmployeeRepository.GetMissingCheckInOutEmployees();
                employeeList = employeeList.Where(s => !_IgnoreList.Contains(s.NickName.ToLower())).ToList();

                if (employeeList.Count > 0)
                {
                    _LOG.Info("There is/are " + employeeList.Count + " employee(s) forgot check in/out");

                    var bodyTemplate = ReadTextFile("emails/forgetcheckinginout.cshtml");

                    if (!string.IsNullOrWhiteSpace(bodyTemplate))
                    {
                        foreach (var item in employeeList)
                        {
                            CheckInOutInfo data;
                            string emailContent;
                            CreateEmailBody(bodyTemplate, item, out data, out emailContent);

                            string emailSubject = string.Join(null, _SenderName, " - Forgot ", data.Type);

                            _EmailService.SendEmail(
                                _SenderEmail,
                                _SenderName,
                                receiverEmail: item.Email,
                                mailCc: null,
                                subject: emailSubject,
                                body: emailContent);
                        }
                    }
                    else
                    {
                        _LOG.Error("Cannot load email template service emails/forgetcheckinginout.cshtml or it's empty");
                    }
                }
                else
                {
                    _LOG.Info("No one missed check in/out");
                }
            }
            catch (Exception ex)
            {
                _LOG.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Function to read file text.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>Content of file.</returns>
        public string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new System.ArgumentNullException("filePath");
            }

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            _LOG.Info("Email teamplate path: " + filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _LOG.Error(ex.Message, ex);
                return string.Empty;
            }
        }

        protected override void OnStart(string[] args)
        {
            _LOG.Info("Service started at " + DateTime.Now.ToString());

            _updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            _updateCounterTimer.Enabled = false;

            _LOG.Info("Service stopped at " + DateTime.Now.ToString());
        }

        /// <summary>
        /// Create email body.
        /// </summary>
        /// <param name="bodyTemplate">Body template.</param>
        /// <param name="item">MissingCheckInOut Model.</param>
        /// <param name="data">CheckInOut Info.</param>
        /// <param name="emailContent">Email Content.</param>
        private static void CreateEmailBody(string bodyTemplate, MissingCheckInOutModel item, out CheckInOutInfo data, out string emailContent)
        {
            data = new CheckInOutInfo
            {
                Employee = item.NickName,
                Date = item.AttendanceDate.ToShortDateString(),
                Type = item.Type == 1 ? "Check In" : "Check Out",
                Shift = item.ShiftCode,
                Remark = item.Type == 1 ? "Forgot Check In" : "Forgot Check Out"
            };

            emailContent = RazorEngine.Razor.Parse(bodyTemplate, data);
        }
    }
}
