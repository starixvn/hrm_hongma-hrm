// --------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.17">Created</change>
// </history>

namespace AutoJobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Program Class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /*
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        */
        public static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new JobService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
