using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;

namespace SpeechToVoiceApp;

public partial class Form1 : Form
{
    private SpeechRecognitionEngine? recognizer;
    private SpeechSynthesizer? synthesizer;
    private bool isListening = false;

    public Form1()
    {
        InitializeComponent();
        InitializeSpeechRecognition();
        InitializeSpeechSynthesis();
        UpdateUIState();
    }

    private void InitializeSpeechRecognition()
    {
        try
        {
            // Create a new speech recognition engine with the default system recognizer
            recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
            
            // Set up the audio input (default audio device)
            recognizer.SetInputToDefaultAudioDevice();
            
            // Load dictation grammar for free-form speech
            recognizer.LoadGrammar(new DictationGrammar());
            
            // Configure recognition settings for better dictation
            recognizer.BabbleTimeout = TimeSpan.FromSeconds(3);
            recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            recognizer.EndSilenceTimeout = TimeSpan.FromMilliseconds(500);
            recognizer.EndSilenceTimeoutAmbiguous = TimeSpan.FromMilliseconds(1500);
            
            // Enable audio input adaptation for better recognition
            try
            {
                recognizer.UpdateRecognizerSetting("AdaptationOn", 1);
                recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 5);
            }
            catch
            {
                // Some settings might not be available on all systems
            }
            
            // Set up event handlers
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected;
            recognizer.RecognizeCompleted += Recognizer_RecognizeCompleted;
            
            // Start with a recognition attempt to warm up the engine
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            
            UpdateStatus("Dictation mode ready. Click 'Start Listening' to begin.");
        }
        catch (Exception ex)
        {
            string errorMsg = $"Error initializing speech recognition: {ex.Message}";
            UpdateStatus(errorMsg);
            MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void Recognizer_RecognizeCompleted(object? sender, RecognizeCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            UpdateStatus($"Recognition error: {e.Error.Message}");
        }
        else if (e.Cancelled)
        {
            UpdateStatus("Recognition stopped");
        }
        else if (e.Result != null && !string.IsNullOrEmpty(e.Result.Text))
        {
            // If we have a result, it will be handled by the SpeechRecognized event
        }
    }
    
    private void Recognizer_SpeechRecognitionRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => Recognizer_SpeechRecognitionRejected(sender, e)));
            return;
        }
        
        UpdateStatus("Speech not recognized. Please try speaking more clearly.");
    }

    private void InitializeSpeechSynthesis()
    {
        try
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing speech synthesis: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
    {
        if (e.Result != null && !string.IsNullOrEmpty(e.Result.Text))
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Recognizer_SpeechRecognized(sender, e)));
                return;
            }
            
            // Get the confidence level of the recognition
            float confidence = e.Result.Confidence * 100;
            string textToAdd = e.Result.Text;
            
            // Only add text if confidence is above threshold
            if (confidence > 50) // 50% confidence threshold
            {
                txtRecognizedText.AppendText(textToAdd + " ");
                txtRecognizedText.ScrollToCaret();
                UpdateStatus($"Recognized: {textToAdd} ({confidence:0}%)");
            }
            else
            {
                UpdateStatus($"Low confidence ({confidence:0}%) - Try speaking more clearly");
            }
        }
    }

    private void btnStartStop_Click(object sender, EventArgs e)
    {
        if (isListening)
        {
            StopListening();
        }
        else
        {
            StartListening();
        }
        
        UpdateUIState();
    }

    private void StartListening()
    {
        if (recognizer != null && !isListening)
        {
            try
            {
                // Clear any previous results
                txtRecognizedText.Clear();
                
                // Start with a fresh recognition context
                recognizer.RecognizeAsyncCancel();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                
                isListening = true;
                
                // Add a visual cue that we're listening
                txtRecognizedText.BackColor = Color.LightYellow;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error starting speech recognition: {ex.Message}";
                UpdateStatus(errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void StopListening()
    {
        if (recognizer != null && isListening)
        {
            try
            {
                // Let any final recognition complete
                System.Threading.Thread.Sleep(200);
                recognizer.RecognizeAsyncCancel();
                isListening = false;
                
                // Reset the text box color
                txtRecognizedText.BackColor = SystemColors.Window;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error stopping speech recognition: {ex.Message}";
                UpdateStatus(errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void btnSpeak_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtTextToSpeak.Text))
        {
            try
            {
                synthesizer?.SpeakAsync(txtTextToSpeak.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error with text-to-speech: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void UpdateUIState()
    {
        if (isListening)
        {
            btnStartStop.Text = " Stop Listening";
            UpdateStatus("Listening... Speak now!");
        }
        else
        {
            btnStartStop.Text = " Start Listening";
            UpdateStatus("Ready");
        }
    }
    
    private void UpdateStatus(string message)
    {
        if (lblStatus.InvokeRequired)
        {
            lblStatus.Invoke(new Action(() => UpdateStatus(message)));
            return;
        }
        
        lblStatus.Text = message;
        if (message.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
        {
            lblStatus.ForeColor = Color.Red;
        }
        else if (message.Contains("Listening", StringComparison.OrdinalIgnoreCase))
        {
            lblStatus.ForeColor = Color.Green;
        }
        else
        {
            lblStatus.ForeColor = SystemColors.ControlText;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        StopListening();
        recognizer?.Dispose();
        synthesizer?.Dispose();
    }
}
