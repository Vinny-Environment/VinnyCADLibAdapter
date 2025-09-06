using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinnyLibConverterCommon.VinnyLibDataStructure;
using VinnyLibConverterCommon;

namespace VinnyCADLibAdapter
{
    internal class VinnyCaDLibImporter : VinnyLibConverterCommon.Interfaces.ICadImportProcessing
    {
        public static VinnyCaDLibImporter CreateInstance()
        {
            if (mInstance == null) mInstance = new VinnyCaDLibImporter();
            return mInstance;
        }
        public void ImportFrom(ImportExportParameters openParameters)
        {

        }

        private static VinnyCaDLibImporter mInstance;
    }
}
