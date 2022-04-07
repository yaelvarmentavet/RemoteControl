using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using RemoteControl.Models;

namespace RemoteControl.Droid
{
    public sealed class Bootstrap
    {
        public static void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<DataModel>().AsSelf();
            builder.RegisterType<UsbSerial>().As<IUsbSerial>();
            builder.RegisterType<UsbCamera>().As<IUsbCamera>();

            IContainer container = builder.Build();

            AutofacServiceLocator asl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => asl);
        }
    }
}