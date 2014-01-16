// --------------------------------------------------------------------------
// <copyright file="ProjectInstaller.Designer.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.17">Created</change>
// </history>

namespace AutoJobs
{
    /// <summary>
    /// Project Installer.
    /// </summary>
    public partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;
        private System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller _serviceInstaller1;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this._serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this._serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this._serviceProcessInstaller1.Password = null;
            this._serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            // 
            this._serviceInstaller1.ServiceName = "AutoJob";
            this._serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this._serviceProcessInstaller1,
            this._serviceInstaller1});

        }

        #endregion
    }
}