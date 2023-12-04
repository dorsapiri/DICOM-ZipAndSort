using DICOM_module.Model;
using DICOM_module.ViewModel.Helper;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;
using EvilDICOM.Core.Helpers;
using EvilDICOM.Core.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;




namespace DICOM_module.ViewModel
{
    public class DicomVM : ViewModelBase
    {
        
        private ObservableCollection<DicomM> dicomFiles { get; set; }
        ObservableCollection<DicomM> DICOMsortedByPatientID {  get; set; }
        private ObservableCollection<ZipFolderVM> _zipFolders;

        public ObservableCollection<ZipFolderVM> zipFolders
        {
            get { return _zipFolders; }
            set 
            { 
                _zipFolders = value;
                OnPropertyChanged(nameof(zipFolders));
            }
        }

        private ICommand _openDicomFolder;
        public ICommand OpenDicomFolder
        {
            get
            {
                return _openDicomFolder ?? (_openDicomFolder = new RelayCommand(OpenFolder));
            }
        }
        public DicomVM()
        {
            dicomFiles = new ObservableCollection<DicomM>();
            DICOMsortedByPatientID = new ObservableCollection<DicomM>();
            LoadZipFolders();
        }

        private void OpenFolder()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select file",
                Filter = "DICOM Files|*.dcm|All Files|*.*",
                Multiselect= true,
                CheckFileExists=true,
                CheckPathExists=true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                LoadFiles(openFileDialog.FileNames);
                
            }
        }

        private void LoadFiles(string[] fileNames)
        {
            try
            {
                foreach (string filename in fileNames)
                {
                    DICOMObject Dicomobj = DICOMObject.Read(filename);
                    DicomM dicomM = new DicomM {
                        DICOMObject = Dicomobj,
                        FilePath = filename,
                        FileName = Path.GetFileName(filename)
                    };
                    dicomFiles.Add(dicomM);
                }
            }
            catch
            {

            }
            Sort(dicomFiles);
            CategoriesAndZipp();
            LoadZipFolders();
        }

        private void LoadZipFolders()
        {
            string pathzipFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) , "DICOM_Zip_Files");
            string[] zipfiles = Directory.GetFiles(pathzipFolder, "*.zip");
            zipFolders = new ObservableCollection<ZipFolderVM>(zipfiles.Select(
                file => new ZipFolderVM { filename = Path.GetFileNameWithoutExtension(file), filePath = file }
                ));
        }

        private void Sort(ObservableCollection<DicomM> dicomsList) 
        {
            SortByPatientID();
            SortByStudyID();
            SortBySerriesInstanceId();

        }

        private void SortByPatientID()
        {
            var DICOMsortedByPatientID = new ObservableCollection<DicomM>(
            dicomFiles.OrderBy((DicomM dicomObject) =>
                {
                    var patientIdElement = dicomObject.DICOMObject.FindFirst(TagHelper.PatientID);
                    return patientIdElement?.DData.ToString() ?? string.Empty;
                }).ToList());
            dicomFiles.Clear();
            dicomFiles = DICOMsortedByPatientID;
        }
        private void SortByStudyID()
        {
            var DICOMsortedByPatientID = new ObservableCollection<DicomM>(
            dicomFiles.OrderBy((DicomM dicomObject) =>
            {
                var patientIdElement = dicomObject.DICOMObject.FindFirst(TagHelper.StudyID);
                return patientIdElement?.DData.ToString() ?? string.Empty;
            }).ToList());
            dicomFiles.Clear();
            dicomFiles = DICOMsortedByPatientID;
        }
        private void SortBySerriesInstanceId()
        {
            var DICOMsortedByPatientID = new ObservableCollection<DicomM>(
            dicomFiles.OrderBy((DicomM dicomObject) =>
            {
                var patientIdElement = dicomObject.DICOMObject.FindFirst(TagHelper.SeriesNumber);
                return patientIdElement?.DData.ToString() ?? string.Empty;
            }).ToList());
            dicomFiles.Clear();
            dicomFiles = DICOMsortedByPatientID;
        }

        private void CategoriesAndZipp()
        {
            var categoriesWithPatientId = dicomFiles.GroupBy((DicomM dicomObject) => 
            {
                var patientIdElement = dicomObject.DICOMObject.FindFirst(TagHelper.PatientID);
                return patientIdElement?.DData.ToString() ?? string.Empty;
            }).ToList();
            
            string zipFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DICOM_Zip_Files");
            if (!Directory.Exists(zipFolderPath))
                Directory.CreateDirectory(zipFolderPath);

            foreach (var patientGroup in categoriesWithPatientId) 
            {
                string patientId = patientGroup.Key;
                List<DicomM> list = new List<DicomM>();
                list = patientGroup.ToList(); 
                var firstDicom = list.Find(dcm => dcm.DICOMObject.FindFirst(TagHelper.PatientID).DData == patientId).DICOMObject ;
                var strongName = firstDicom.FindFirst(TagHelper.PatientName) as PersonName; 
                var firstName = strongName.FirstName;
                var lastName = strongName.LastName;
                string zipFileName = $"{patientId}_{firstName}_{lastName}.zip";
                string zipFilePath = Path.Combine(zipFolderPath, zipFileName);

                if (!File.Exists(zipFilePath))
                {
                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var dicom in patientGroup)
                        {
                            byte[] dicomData = dicom.DICOMObject.GetBytes();
                            var entry = zipArchive.CreateEntryFromFile(dicom.FilePath, $"{dicom.FileName}.dcm");
                        }
                    }
                }
                else 
                {
                    using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update))
                    {
                        foreach (var dicom in patientGroup)
                        {
                            byte[] dicomData = dicom.DICOMObject.GetBytes();
                            var entry = zipArchive.CreateEntryFromFile(dicom.FilePath, $"{dicom.FileName}.dcm");
                        }
                    }
                }
                
            }
        }


    }

    public class ZipFolderVM : ViewModelBase
    {
        public string filename { get; set; }
        public string filePath { get; set; }
        private string _txtEmailAddress;

        public string txtEmailAddress
        {
            get { return _txtEmailAddress; }
            set 
            { 
                _txtEmailAddress = value;
                OnPropertyChanged(nameof(txtEmailAddress));
            }
        }
        private ICommand _sendEmail;

        public ICommand sendEmail
        {
            get { return _sendEmail ?? (_sendEmail = new RelayCommand(SendEmail)); }
            
        }

       

        private void SendEmail()
        {
            string attachmentPath = filePath;
            string recipient = txtEmailAddress;
            string subject = "DICOM Files";
            string body = "Your Files attached in the email.";

            EmailSender emailsender = new EmailSender();
            emailsender.senEmailWithAttachment(recipient, subject, body, attachmentPath);
        }
    }
}
