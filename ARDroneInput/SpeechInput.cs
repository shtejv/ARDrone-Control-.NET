using System;
using System.Collections.Generic;
using System.Text;
using ARDrone.Input.Utility;

namespace ARDrone.Input
{
    class SpeechInput : GenericInput
    {
        public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();

            if (!CheckIfDeviceExists(currentDevices))
            {
                newDevices.Add(new SpeechInput());
            }

            return newDevices;
        }

        private static bool CheckIfDeviceExists(List<GenericInput> currentDevices)
        {
            for (int i = 0; i < currentDevices.Count; i++)
            {
                if (currentDevices[i].DeviceInstanceId == "SP")
                    return true;
            }

            return false;
        }

        public SpeechInput()
            : base()
        { }

        public override void Dispose()
        {

        }

        public override void Init()
        {

        }

        public override void StartRawInput()
        {

        }

        public override String GetCurrentRawInput(out bool isAxis)
        {
            isAxis = false;
            return "";
        }

        public override void EndRawInput()
        {

        }

        public override void StartControlInput()
        {
            
        }

        public override InputState GetCurrentControlInput()
        {
            return null;
        }

        public override void EndControlInput()
        {

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
