[assembly: WebActivator.PreApplicationStartMethod(typeof(Fanex.NexHR.Site.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Fanex.NexHR.Site.App_Start.NinjectWebCommon), "Stop")]

namespace Fanex.NexHR.Site.App_Start
{
    using System;
    using System.Web;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Fanex.NexHR.Repositories.Interfaces;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper _bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            _bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            _bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {            
            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();
            kernel.Bind<IHolidayRepository>().To<HolidayRepository>();
            kernel.Bind<IMemoRepository>().To<MemoRepository>();
            kernel.Bind<ITeamRepository>().To<TeamRepository>();
            kernel.Bind<ILookupDataRepository>().To<LookupDataRepository>();
            kernel.Bind<IShiftRepository>().To<ShiftRepository>();
            kernel.Bind<IReportRepository>().To<ReportRepository>();
        }
    }
}
