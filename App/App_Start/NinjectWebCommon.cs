using App;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace App
{
    using System;
    using System.Web;

    using Domain.Dal.Repositories.Abstract;
    using Domain.Dal.Repositories.Concrete;
    using Domain.Models;
    using Domain.Services.Abstract;
    using Domain.Services.Concrete;
    using Domain.Validations.Abstract;
    using Domain.Validations.Concrete;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            NinjectWebCommon.bootstrapper.Initialize(NinjectWebCommon.CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            NinjectWebCommon.bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            StandardKernel kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>()
                    .ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>()
                    .To<HttpApplicationInitializationHttpModule>();

                NinjectWebCommon.RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // TODO: Load settings from config file
            DatabaseValidationSettings databaseValidationSettings = new DatabaseValidationSettings();
            DbRepositorySettings dbRepositorySettings = new DbRepositorySettings();

            kernel.Bind<IDbRepository>()
                .ToMethod(context => new DbRepository(dbRepositorySettings));

            kernel.Bind<IDatabaseValidation>()
                .ToMethod(context => new DatabaseValidation(databaseValidationSettings));

            kernel.Bind<IDatabaseService>()
                .ToMethod(context => new DatabaseService(context.Kernel.Get<IDbRepository>(), context.Kernel.Get<IDatabaseValidation>()));
        }
    }
}