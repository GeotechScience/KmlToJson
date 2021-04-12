using KmlToJson.Views;
using Prism.Ioc;
using System.Windows;

namespace KmlToJson
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //SharpKml.Engine.KmlFile.;

            //containerRegistry.RegisterInstance();
        }
    }
}
