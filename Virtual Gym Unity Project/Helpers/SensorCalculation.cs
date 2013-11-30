using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Template.Helpers
{
    class SensorCalculation
    {     
        
        public static int pitchDirection;//pitchDirection = 1 -> "FORWARD" || 2 -> "BACKWARD"
        public static bool doBeep = false;
        public static bool learningMode = false;
        public static float previousPitchDegrees = -999;
        public static float firstPitchDegree;
        private static int accuracyForward; // measurement count
        private static int accuracyBackward;// measurement count
        public static float maxPitchValue;
        
        public static void checkCurrentPitchValue(float pitchDegrees)
        {
            if (pitchDegrees >= maxPitchValue)
            {
                doBeep = true;
            }
        }
        public static void DefineMaxPitchValue(float pitchDegrees)
        {
            maxPitchValue = pitchDegrees - 2;
            learningMode = false;  
        }

        public static String GetPitchDirection(float pitchDegrees)
        {
            // rounding to 0 digits after comma
            float currentPitchDegrees = (float) Math.Round(pitchDegrees, 0);
            // case when method is called for the first time
            if (previousPitchDegrees == -999)
            {
                previousPitchDegrees = currentPitchDegrees;
            }
            // if previous pitchDirection is FORWARD and accuracy more than 1, then we done
            if (pitchDirection == 1 && accuracyForward > 1)
            {
                accuracyForward = 0;
                return "FORWARD";
            }
            // if previous pitchDirection is BACKWARD and accuracy more than 1, then we done
            if (pitchDirection == 2 && accuracyBackward > 1)
            {
                DefineMaxPitchValue(firstPitchDegree);
                
                accuracyBackward = 0;
                return "BACKWARD";
            }
            // adding 1 to accuracy only if previous pitchDirection is the same as current
            if (previousPitchDegrees < currentPitchDegrees)
            {
                if (pitchDirection == 1)
                {
                    accuracyForward++;
                }
                pitchDirection = 1;
                
            }
            else if (previousPitchDegrees > currentPitchDegrees)
            {
                if (pitchDirection == 2)
                {
                    accuracyBackward++;
                }
                pitchDirection = 2;
                
            }
            firstPitchDegree = previousPitchDegrees;
            previousPitchDegrees = currentPitchDegrees;
            return pitchDirection == 0 ? null : GetPitchDirectionName(pitchDirection);// returning null always except when calculations is done once
        }

        private static String GetPitchDirectionName(int code)
        {
            return pitchDirection == 1 ? "FORWARD" : "BACKWARD";
        }
    }
}
