using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANPR
{
    class Controller : Module
    {
        private CarDetector carDetector;
        private PlateRecognizer plateRecognizer;
        private ReportManager reportManager;
        private Analyzer analyzer;

        Controller() {
            carDetector = new CarDetector();
            plateRecognizer = new PlateRecognizer();
            reportManager = new ReportManager();
            analyzer = new Analyzer();
        }

        private bool changeDronePosition();
        
        public bool setModuleParameters (string module, List<Tuple<String,Object>> parameters);
    }
}
