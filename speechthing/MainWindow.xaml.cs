using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Navigation;

namespace speechthing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SpeechSynthesizer spe = new SpeechSynthesizer();

        static List<string> list = new List<string>();

        static char Mode = '0';

        System.Speech.AudioFormat.SpeechAudioFormatInfo format;

        public MainWindow()
        {
            InitializeComponent();
            loadSettings();
            spe.StateChanged += Spe_StateChanged;
            spe.Volume = 100;
            name.MaxLength = 128;
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                time.Text = DateTime.Now.ToString("F");
            },
            Dispatcher);
        }

        private void Spe_StateChanged(object sender, StateChangedEventArgs e)
        {
            string aux = "";
            switch (Mode)
            {
                case '0':
                    aux = "SSML Disabled";
                    break;
                case '1':
                    aux = "SSML Enabled";
                    break;
            }
            warning.Text = spe.State.ToString() + " – " + aux;
        }

        public int loadSettings()
        {
            systemVoice.Text = $"Voice Name: {spe.Voice.Description}\nVoice Culture: {spe.Voice.Culture}\nVoice ID: {spe.Voice.Id}";
            return 1;
        }

        public void rateChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newRate = Convert.ToInt32(speechRate.Value);
            rateText.Text = newRate.ToString();
            speechRate.Value = newRate;
            spe.Rate = newRate;
        }

        public void speak(object sender, RoutedEventArgs e)
        {
            string pra = text.Text;
            switch (Mode)
            {
                case '0':
                    spe.SpeakAsync(pra);
                    break;
                case '1':
                    spe.SpeakSsmlAsync(pra);
                    break;
                default:
                    spe.SpeakAsync(pra);
                    break;
            }
            speech.IsEnabled = false;
            cancel.IsEnabled = true;
            pauseB.IsEnabled = true;
            unpauseB.IsEnabled = false;
            spe.SpeakCompleted += Spe_SpeakCompleted;
            progress.Value = volumeSpeak.Value;
            Console.WriteLine("Speaking");
        }

        private void Spe_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            speech.IsEnabled = true;
            cancel.IsEnabled = false;
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = false;
            progress.Value = 0;
            Console.WriteLine("Speak Completed");
        }

        private void helpGet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Speechy\nBuild 0.2.8 (12/26/2022)\n \n\nCreated by pythonomial\nContact me on Discord!\nazure#9308\n\nChangelog (version 1.3)\nAdded ability to stop recording\nFixed readability issues for devices\nChanged TextBox font to Consolata for easier reading\nChanged TextBox to allow wordwrapping and newlines\nAdded SSML support\nAdded the ability to specify the export directory while recording\nChanged layout to be more simple\nBug fixes\nAdded the ability to change from Male to Female voice", "Speechy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cancelSpeech(object sender, RoutedEventArgs e)
        {
            spe.SpeakAsyncCancelAll();
            cancel.IsEnabled = false;
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = false;
            speech.IsEnabled = true;
            spe.Resume();
            progress.Value = 0;
            Console.WriteLine("Cancel");
        }

        private void pause(object sender, RoutedEventArgs e)
        {
            spe.Pause();
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = true;
            Console.WriteLine("Pause");
        }

        private void unpause(object sender, RoutedEventArgs e)
        {
            spe.Resume();
            pauseB.IsEnabled = true;
            unpauseB.IsEnabled = false;
            Console.WriteLine("Unpause");
        }

        private void changevoice(object sender, RoutedEventArgs e)
        {
            switch (Voicechanger.Content)
            {
                case "Male":
                    spe.SelectVoiceByHints(VoiceGender.Female);
                    Voicechanger.Content = "Female";
                    break;
                case "Female":
                    spe.SelectVoiceByHints(VoiceGender.Male);
                    Voicechanger.Content = "Male";
                    break;
            }
        }

        private void volumeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newVolume = Convert.ToInt32(volumeSpeak.Value);
            volumeText.Text = newVolume.ToString();
            volumeSpeak.Value = newVolume;
            spe.Volume = newVolume;
        }

        private void saveFileA(object sender, RoutedEventArgs e)
        {
            savefilegrid.Visibility = Visibility.Visible;
            spe.SpeakAsyncCancelAll();
            cancel.IsEnabled = false;
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = false;
            speech.IsEnabled = true;
            spe.Resume();
            progress.Value = 0;
            Console.WriteLine("Cancel");
        }

        private void saveFileB(object sender, RoutedEventArgs e)
        {
            savefilegrid.Visibility = Visibility.Hidden;
            var confirm = MessageBox.Show("Are you sure you want to save this file? Speechy may overwrite any file in the same directory as speechy.exe is in with the same name. Data loss may occur!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (confirm)
            {
                case MessageBoxResult.Yes:
                    var quality = MessageBox.Show("Save as high quality? (48.0 kHz 16-bit stereo) (Low quality is 44.1 kHz 8-bit mono)", "Quality", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (quality)
                    {
                        case MessageBoxResult.Yes:
                            format = new System.Speech.AudioFormat.SpeechAudioFormatInfo(480000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Stereo);
                            break;
                        case MessageBoxResult.No:
                            format = new System.Speech.AudioFormat.SpeechAudioFormatInfo(441000, System.Speech.AudioFormat.AudioBitsPerSample.Eight, System.Speech.AudioFormat.AudioChannel.Mono);
                            break;
                    }
                    if (name.Text != "")
                    {
                        try
                        {
                            spe.SetOutputToWaveFile($@"{name.Text}", format);
                        }
                        catch
                        {
                            MessageBox.Show("There was an error saving the file.", "Error saving file", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    savefilegrid.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        private void exitSaveFile(object sender, RoutedEventArgs e)
        {
            savefilegrid.Visibility = Visibility.Hidden;
        }

        private void changeMode(object sender, RoutedEventArgs e)
        {
            string aux = "";
            switch (Mode)
            {
                case '0':
                    Mode = '1';
                    mode.Content = "SSML (Advanced)";
                    aux = "SSML Enabled";
                    break;
                case '1':
                    Mode = '0';
                    mode.Content = "Non-SSML (Basic)";
                    aux = "SSML Disabled";
                    break;
            }
            warning.Text = spe.State.ToString() + " – " + aux;
        }

        private void DefaultDevice(object sender, RoutedEventArgs e)
        {
            spe.SetOutputToDefaultAudioDevice();
        }

        private void text_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void linkA(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }

}