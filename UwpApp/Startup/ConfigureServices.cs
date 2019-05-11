using Autofac;
using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpApp.ViewModel;

namespace UwpApp.Startup
{
    public static class ConfigureServices
    {
        private static readonly IDataProvider dataProvider = new MbaApiClient(new Uri("http://localhost:58665/"));
        public static IContainer Container()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LoginViewModel>().As<ILoginViewModel>();
            builder.RegisterType<MainViewModel>().As<IMainViewModel>();
            builder.RegisterInstance(dataProvider).As<IDataProvider>();
            return builder.Build();
        }
    }
}
