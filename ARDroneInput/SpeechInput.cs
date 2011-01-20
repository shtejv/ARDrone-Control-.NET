using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Input
{
    class SpeechInput : GenericInput
    {
        public SpeechInput()
            : base()
        { }

        public override void Dispose()
        {

        }

        public override void InitCurrentlyInvokedInput()
        {

        }

        public override String GetCurrentlyInvokedInput(out bool isAxis)
        {
            isAxis = false;
            return "";
        }

        public override InputState GetCurrentState()
        {
            return null;
        }

        public override void CancelEvents()
        {
            
        }

        public override bool Cancellable
        {
            get { return true; }
        }

        public override bool IsDevicePresent
        {
            get
            {
                return true;
            }
        }

        public override String DeviceName
        {
            get { return "Speech"; }
        }

        public override String DeviceInstanceId
        {
            get { return "SP"; }
        }


    }
}
