using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using RemoteControl.Models;

namespace RemoteControl.UWP
{
    public sealed class Bootstrap
    {
        public static void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<DataModel>().AsSelf();
            builder.RegisterType<UsbDevice>().As<IUsbDevice>();

            IContainer container = builder.Build();

            AutofacServiceLocator asl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => asl);
        }
    }
}
