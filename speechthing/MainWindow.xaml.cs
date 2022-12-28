using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace speechthing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SpeechSynthesizer spe = new SpeechSynthesizer();

        static List<string> list = new List<string>(); // Of use in the future

        System.Speech.AudioFormat.SpeechAudioFormatInfo format;

        private void LoadKeyboardShortcuts()
        {
            RoutedCommand Open = new RoutedCommand(); Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(Open, OpenFile_Click));
            RoutedCommand Save = new RoutedCommand(); Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(Save, SaveFile_Click));
            RoutedCommand Gender = new RoutedCommand(); Gender.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(Gender, changevoice));
            RoutedCommand Mode = new RoutedCommand(); Mode.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(Mode, SSMLEnabled));
        }
        public MainWindow()
        {
            InitializeComponent();
            loadSettings();
            spe.StateChanged += Spe_StateChanged;
            spe.Volume = 100;
            name.MaxLength = 128;
            LoadKeyboardShortcuts(); // For stuff //
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                time.Text = DateTime.Now.ToString("F");
            },
            Dispatcher);
        }

        private void Spe_StateChanged(object sender, StateChangedEventArgs e)
        {
            string aux = "";
            switch (SSMLEnable.IsChecked)
            {
                case true:
                    aux = "SSML Disabled";
                    break;
                case false:
                    aux = "SSML Enabled";
                    break;
            }
            warning.Text = spe.State.ToString() + " – " + aux;
        }

        public int loadSettings()
        {
            systemVoice.Text = $"Voice Name: {spe.Voice.Description}\nVoice Culture: {spe.Voice.Culture}\nVoice ID: {spe.Voice.Id}\nVoice Gender: {spe.Voice.Gender}\nMode: {SSMLEnable.Header.ToString().Substring(1)}";
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
            switch (SSMLEnable.Header)
            {
                case "_SSML":
                    spe.SpeakSsmlAsync(pra);
                    break;
                case "_Raw Text":
                    spe.SpeakAsync(pra);
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
            Console.WriteLine("Speaking");
        }

        private void Spe_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            speech.IsEnabled = true;
            cancel.IsEnabled = false;
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = false;
            Console.WriteLine("Speak Completed");
        }

        private void helpGet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Speechy\nBuild 0.2.8 (12/28/2022)\n \n\nCreated by pythonomial\nContact me on Discord!\nazure#9308\n\nThis was a quick update that changed the layout a bit.", "Speechy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void cancelSpeech(object sender, RoutedEventArgs e)
        {
            spe.SpeakAsyncCancelAll();
            cancel.IsEnabled = false;
            pauseB.IsEnabled = false;
            unpauseB.IsEnabled = false;
            speech.IsEnabled = true;
            spe.Resume();
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
            switch (spe.Voice.Gender)
            {
                case VoiceGender.Male:
                    spe.SelectVoiceByHints(VoiceGender.Female);
                    GenderChange.Header = "_Female Voice";
                    GenderChange.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/icon/female.png", UriKind.Relative))
                    };
                    break;
                case VoiceGender.Female:
                    spe.SelectVoiceByHints(VoiceGender.Male);
                    GenderChange.Header = "_Male Voice";
                    GenderChange.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/icon/male.png", UriKind.Relative))
                    };
                    break;
            }
            systemVoice.Text = $"Voice Name: {spe.Voice.Description}\nVoice Culture: {spe.Voice.Culture}\nVoice ID: {spe.Voice.Id}\nVoice Gender: {spe.Voice.Gender}\nMode: {SSMLEnable.Header.ToString().Substring(1)}";
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

        private void SSMLEnabled(object sender, RoutedEventArgs e)
        {
            switch (SSMLEnable.Header) {
                case "_Raw Text":
                    warning.Text = spe.State.ToString() + " – " + "SSML Enabled";
                    SSMLEnable.Header = "_SSML";
                    SSMLEnable.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/icon/page_white_code.png", UriKind.Relative))
                    };
                    break;
                case "_SSML":
                    warning.Text = spe.State.ToString() + " – " + "SSML Disabled";
                    SSMLEnable.Header = "_Raw Text";
                    SSMLEnable.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/icon/page_white_text.png", UriKind.Relative))
                    };
                    break;
            }
            systemVoice.Text = $"Voice Name: {spe.Voice.Description}\nVoice Culture: {spe.Voice.Culture}\nVoice ID: {spe.Voice.Id}\nVoice Gender: {spe.Voice.Gender}\nMode: {SSMLEnable.Header.ToString().Substring(1)}";
        }

        private void DefaultDevice(object sender, RoutedEventArgs e)
        {
            spe.SetOutputToDefaultAudioDevice();
        }

        private void linkA(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog readFile = new OpenFileDialog();
            readFile.Filter = "Raw text or SSML (*.txt, *.xml)|*.txt;*.xml";
            if (readFile.ShowDialog() == true)
            {
                text.Text = File.ReadAllText(readFile.FileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Raw text or SSML (*.txt, *.xml)|*.txt;*.xml";
            if (saveFile.ShowDialog() == true)
            {
                File.WriteAllText(saveFile.FileName, text.Text);
            }
        }
    }

}