using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANPR
{
    enum CarDetectorRetStatus {
        //Здесь дальше разные статусы

    };

    enum PlateRecognizerRetStatus {
        //Здесь дальше разные статусы
    };


    class Analyzer
    {
        private CarDetector carDetector;
        private PlateRecognizer plateRecognizer;

        public Analyzer()
        {
            this.carDetector = null;
            this.plateRecognizer = null;
        }

        public Analyzer(CarDetector carDet, PlateRecognizer plateRec)
        {
            this.carDetector = carDet;
            this.plateRecognizer = plateRec;
        }

        //public CarDetectorRetStatus checkDetectorData();

        //public PlateRecognizerRetStatus checkRecognizerDate();
    }
}
