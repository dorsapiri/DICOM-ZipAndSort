using EvilDICOM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_module.Model
{
    public class DicomM
    {
        public DICOMObject DICOMObject { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

    }
}
