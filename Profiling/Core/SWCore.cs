using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SolidWorks.Interop.swcommands;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Profiling.Core
{
    class SWCore
    {

        EquationMgr swMgr;
        ModelDoc2 swModel;
        SldWorks swApp;

        public void Open3DModel(string path)
        {
           //убиваем процессы SW, если запущены
            Process[] processes = Process.GetProcessesByName("SLDWORKS");  
            foreach (Process process in processes)  
            {  
                process.CloseMainWindow();  
                process.Kill();  
            }

            //создаём инстанс
            object processSW = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("SldWorks.Application"));
            swApp = (SldWorks)processSW;
            swApp.Visible = true;

            //открываем файл
            int fileError=0,fileWarning=0;
            string pathToFile = path;
            swModel = swApp.OpenDoc6(pathToFile, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "1",ref fileError,ref fileWarning);
        
        }
      
    }
}
