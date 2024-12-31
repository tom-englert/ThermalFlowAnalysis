using System.Windows;
using TomsToolbox.Wpf.Styles;

namespace ThermalFlowAnalysis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Resources.RegisterDefaultStyles();
        }
    }
}
