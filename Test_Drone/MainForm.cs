using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARDrone.Control;
using System.Speech.Recognition;

namespace Test_Drone
{
    public partial class MainForm : Form
    {
        SpeechRecognitionEngine speechRecognizer;
        ARDroneControl control;

        public MainForm()
        {
            InitializeComponent();
        }

        private void timerDrone_Tick(object sender, EventArgs e)
        {
            ARDroneControl.DroneData data = control.GetCurrentDroneData();
            textBoxDrone.Text = data.Altitude.ToString();
        }

        private void buttonSpeech_Click(object sender, EventArgs e)
        {
            speechRecognizer = new SpeechRecognitionEngine();
            speechRecognizer.SetInputToDefaultAudioDevice();

            speechRecognizer.LoadGrammar(new DictationGrammar());

            speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecognizer_SpeechRecognized);
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void buttonDrone_Click(object sender, EventArgs e)
        {
            control = new ARDroneControl();
            control.Connect();

            timerDrone.Enabled = true;
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            textBoxSpeech.Text = e.Result.Text;
        }
    }
}
