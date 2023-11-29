using DICOM_module.View;
using DICOM_module.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DICOM_module
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DicomV dicomv = new DicomV();
            DicomVM dicomVM = new DicomVM();
            dicomv.DataContext = dicomVM;
            dicomv.ShowDialog();
        }
    }

}
