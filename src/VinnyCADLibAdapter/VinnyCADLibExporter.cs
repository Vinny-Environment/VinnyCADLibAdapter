using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyLibConverterCommon;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterKernel;

namespace VinnyCADLibAdapter
{
    internal class VinnyCADLibExporter : VinnyLibConverterCommon.Interfaces.ICadExportProcessing
    {
        public static VinnyCADLibExporter CreateInstance()
        {
            if (mInstance == null) mInstance = new VinnyCADLibExporter();
            if (mConverter == null) mConverter = VinnyLibConverter.CreateInstance2();
            return mInstance;
        }
        public VinnyLibDataStructureModel CreateData()
        {
            VinnyLibDataStructureModel vinnyModel = new VinnyLibDataStructureModel();

            return vinnyModel;
        }
        public void ExportTo(VinnyLibDataStructureModel vinnyData, ImportExportParameters outputParameters)
        {
            mConverter.ExportModel(vinnyData, outputParameters);
        }

        private static VinnyCADLibExporter mInstance;
        private static VinnyLibConverterKernel.VinnyLibConverter mConverter;
    }
}
