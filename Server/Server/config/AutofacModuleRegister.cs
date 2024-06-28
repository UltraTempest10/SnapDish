using Autofac;
using System.Reflection;

namespace Server.config
{
    public class AutofacModuleRegister : Autofac.Module
    {
        //绑定service
        protected override void Load(ContainerBuilder builder)
        {
            Assembly serviceAssembly = Assembly.Load("Service");
            builder.RegisterAssemblyTypes(serviceAssembly).AsImplementedInterfaces();
        }
    }
}
