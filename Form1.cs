using System;
using System.Diagnostics;
using System.Drawing;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeechToVoiceApp
{
    public partial class Form1 : Form
    {
        private SpeechRecognitionEngine? recognizer;
        private SpeechSynthesizer? synthesizer;
        private bool isListening = false;
        private readonly object syncLock = new();

        public Form1()
        {
            InitializeComponent();
            if (!InitializeSpeechRecognition())
            {
                MessageBox.Show("Speech recognition initialization failed. Please restart the application.", 
                    "Speech Recognition Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            InitializeSpeechSynthesis();
            UpdateUIState();
        }

        private bool InitializeSpeechRecognition()
        {
            try
            {
                // Check if speech recognition is available
                var recognizers = SpeechRecognitionEngine.InstalledRecognizers();
                if (recognizers.Count == 0)
                {
                    UpdateStatus("No speech recognizer is installed. Installing Windows Speech Recognition...");
                    
                    // Try to open the Windows Speech Recognition settings to help the user enable it
                    try { System.Diagnostics.Process.Start("ms-settings:speech"); }
                    catch { /* Ignore if we can't open settings */ }
                    
                    MessageBox.Show("Speech recognition is not installed on this system. Please enable Windows Speech Recognition in Windows Settings.", 
                        "Speech Recognition Not Available", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                    return false;
                }

                // Create a new recognizer
                recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
                
                // Try to set input device
                try
                {
                    recognizer.SetInputToDefaultAudioDevice();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting up audio input: {ex.Message}\n\nPlease check your microphone connection and try again.", 
                        "Audio Input Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    recognizer.Dispose();
                    recognizer = null;
                    return false;
                }

                // Create a simple grammar for better accuracy
                var grammarBuilder = new GrammarBuilder();
                var choices = new Choices();
                choices.Add("hello");
                choices.Add("hi");
                choices.Add("how are you");
                choices.Add("what's up");
                choices.Add("good morning");
                choices.Add("good afternoon");
                choices.Add("good evening");
                choices.Add("test");
                
                grammarBuilder.Append(choices);
                
                // Load a simple grammar first
                recognizer.LoadGrammar(new Grammar(grammarBuilder));
                
                // Then try to load dictation grammar
                try
                {
                    var dictationGrammar = new DictationGrammar();
                    recognizer.LoadGrammar(dictationGrammar);
                }
                catch (Exception ex)
                {
                    // If dictation fails, we'll continue with just the basic grammar
                    Debug.WriteLine($"Warning: Could not load dictation grammar: {ex.Message}");
                }

                // Configure timeouts
                recognizer.BabbleTimeout = TimeSpan.FromMilliseconds(500);
                recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(3);
                recognizer.EndSilenceTimeout = TimeSpan.FromMilliseconds(500);
                recognizer.EndSilenceTimeoutAmbiguous = TimeSpan.FromMilliseconds(500);

                // Set up event handlers
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized!;
                recognizer.SpeechHypothesized += Recognizer_SpeechHypothesized!;
                recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected!;
                recognizer.RecognizeCompleted += Recognizer_RecognizeCompleted!;

                UpdateStatus("Ready to listen. Click 'Start Listening' to begin.");
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error initializing speech recognition: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\n{ex.InnerException.Message}";
                }
                
                MessageBox.Show(errorMessage, 
                    "Speech Recognition Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                    
                UpdateStatus("Speech recognition initialization failed. See error message for details.");
                recognizer?.Dispose();
                recognizer = null;
                return false;
            }
        }

        private void InitializeSpeechSynthesis()
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                UpdateStatus("Speech synthesis ready");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error initializing speech synthesis: {ex.Message}";
                UpdateStatus("Error: Speech synthesis not available");
                MessageBox.Show(errorMsg, "Speech Synthesis Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartStop.Enabled = false;
                
                if (isListening)
                {
                    StopListening();
                }
                else
                {
                    StartListening();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in btnStartStop_Click: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateUIState();
                btnStartStop.Enabled = true;
            }
        }

        private void StartListening()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StartListening));
                return;
            }

            try
            {
                if (recognizer == null) 
                {
                    UpdateStatus("Error: Speech recognition not initialized");
                    return;
                }
                
                // Enable the text box for input
                txtRecognizedText.ReadOnly = false;
                txtRecognizedText.BackColor = Color.White;
                
                // Start recognition
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                isListening = true;
                
                // Visual feedback
                txtRecognizedText.BackColor = Color.LightYellow;
                txtRecognizedText.SelectionStart = txtRecognizedText.TextLength;
                txtRecognizedText.ScrollToCaret();
                
                UpdateStatus("Listening... Speak clearly into your microphone");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting recognition: {ex.Message}");
                UpdateStatus("Error starting recognition");
                isListening = false;
            }
        }

        private void StopListening()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StopListening));
                return;
            }

            try
            {
                if (recognizer != null && isListening)
                {
                    recognizer.RecognizeAsyncCancel();
                    isListening = false;
                    
                    // Reset UI
                    txtRecognizedText.BackColor = Color.White;
                    UpdateStatus("Ready");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping recognition: {ex.Message}");
                isListening = false;
                UpdateStatus("Error stopping recognition");
            }
        }

        private void btnSpeak_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTextToSpeak.Text)) 
            {
                MessageBox.Show("Please enter some text to speak.", "No Text", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                synthesizer?.SpeakAsync(txtTextToSpeak.Text);
                UpdateStatus("Speaking...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in text-to-speech: {ex.Message}");
                MessageBox.Show($"Error in text-to-speech: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string currentHypothesis = string.Empty;
        private int lastHypothesisEnd = 0;

        private void Recognizer_SpeechHypothesized(object? sender, SpeechHypothesizedEventArgs e)
        {
            if (e.Result == null || string.IsNullOrEmpty(e.Result.Text))
                return;

            // Only update if we have a good confidence level
            if (e.Result.Confidence < 0.6f) // Increased confidence threshold
                return;
                
            // Clean up the recognized text
            string newText = e.Result.Text.Trim();
            
            // Remove any partial words or repeated fragments
            if (newText.Contains("...") || newText.Length < 2)
                return;
                
            if (!string.IsNullOrEmpty(newText))
            {
                // Only update if the text has changed significantly
                if (currentHypothesis == null || 
                    !newText.Equals(currentHypothesis, StringComparison.OrdinalIgnoreCase))
                {
                    currentHypothesis = newText;
                    UpdateRecognizedText(currentHypothesis, true);
                }
            }
        }

        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result == null || string.IsNullOrEmpty(e.Result.Text))
                return;

            string recognizedText = e.Result.Text.Trim();
            
            // Skip very short or low-confidence results
            if (recognizedText.Length < 2 || e.Result.Confidence < 0.6f)
                return;
                
            Debug.WriteLine($"Final Recognition: '{recognizedText}' (Confidence: {e.Result.Confidence:P0})");

            // Clean up the recognized text
            recognizedText = recognizedText.ToLower();
            
            // Only update if we have a good confidence level
            if (e.Result.Confidence > 0.6f)
            {
                // Add the recognized text
                UpdateRecognizedText(recognizedText, false);
                
                // Add a space after recognized text for the next word
                if (txtRecognizedText.InvokeRequired)
                {
                    txtRecognizedText.BeginInvoke(new Action(() => 
                    {
                        if (!txtRecognizedText.Text.EndsWith(" ") && !string.IsNullOrEmpty(txtRecognizedText.Text))
                        {
                            txtRecognizedText.AppendText(" ");
                        }
                        lastHypothesisEnd = txtRecognizedText.TextLength;
                    }));
                }
                else
                {
                    if (!txtRecognizedText.Text.EndsWith(" ") && !string.IsNullOrEmpty(txtRecognizedText.Text))
                    {
                        txtRecognizedText.AppendText(" ");
                    }
                    lastHypothesisEnd = txtRecognizedText.TextLength;
                }
                
                // Clear the current hypothesis
                currentHypothesis = string.Empty;
            }
        }

        private void UpdateRecognizedText(string text, bool isHypothesis)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (txtRecognizedText.InvokeRequired)
            {
                txtRecognizedText.BeginInvoke(new Action(() => UpdateRecognizedText(text, isHypothesis)));
                return;
            }

            try
            {
                // Save the current cursor position and selection
                int originalStart = txtRecognizedText.SelectionStart;
                int originalLength = txtRecognizedText.SelectionLength;
                
                // If this is a hypothesis, remove any previous hypothesis first
                if (isHypothesis && lastHypothesisEnd < txtRecognizedText.TextLength)
                {
                    txtRecognizedText.Select(lastHypothesisEnd, txtRecognizedText.TextLength - lastHypothesisEnd);
                    txtRecognizedText.SelectedText = string.Empty;
                }

                // Add a space if needed
                if (txtRecognizedText.TextLength > 0 && 
                    !char.IsWhiteSpace(txtRecognizedText.Text[txtRecognizedText.TextLength - 1]) &&
                    !string.IsNullOrWhiteSpace(text))
                {
                    txtRecognizedText.AppendText(" ");
                }

                // Save the position where we'll insert the new text
                int insertPosition = txtRecognizedText.TextLength;

                // Add the text
            
                if (isHypothesis)
                {
                    // For hypotheses, use gray text
                    txtRecognizedText.AppendText(text);
                    txtRecognizedText.Select(insertPosition, text.Length);
                    txtRecognizedText.SelectionColor = Color.Gray;
                    
                    // Update the last hypothesis end position
                    lastHypothesisEnd = txtRecognizedText.TextLength;
                }
                else
                {
                    // For confirmed text, use black text
                    txtRecognizedText.AppendText(text);
                    txtRecognizedText.Select(insertPosition, text.Length);
                    txtRecognizedText.SelectionColor = Color.Black;
                    
                    // Update the last hypothesis end position to the end of the new text
                    lastHypothesisEnd = txtRecognizedText.TextLength;
                }

                // Restore the cursor position and selection
                txtRecognizedText.SelectionStart = originalStart;
                txtRecognizedText.SelectionLength = originalLength;
                
                // Ensure the new text is visible
                txtRecognizedText.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating recognized text: {ex.Message}");
            }
        }

        private void Recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Recognizer_SpeechRecognitionRejected(sender, e)));
                return;
            }
            UpdateStatus("Speech not recognized. Please try again.");
        }

        private void Recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine($"Recognition completed with error: {e.Error.Message}");
                UpdateStatus("Recognition error: " + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Debug.WriteLine("Recognition was cancelled");
                UpdateStatus("Recognition stopped");
            }
        }

        private void Recognizer_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            // Optional: Could add visual feedback for audio level
            // For example, update a progress bar or label with e.AudioLevel
        }

        private void Recognizer_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            if (e.AudioSignalProblem == AudioSignalProblem.TooFast || 
                e.AudioSignalProblem == AudioSignalProblem.TooSlow ||
                e.AudioSignalProblem == AudioSignalProblem.NoSignal)
            {
                BeginInvoke(new Action(() => {
                    string problem = e.AudioSignalProblem == AudioSignalProblem.NoSignal 
                        ? "clearly" 
                        : "more " + e.AudioSignalProblem.ToString().ToLower()
                            .Replace("toofast", "slowly")
                            .Replace("tooslow", "quickly");
                    UpdateStatus($"Please speak {problem}");
                }));
            }
        }

        private void Recognizer_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            // If audio input stops while we're still listening, try to restart it
            if (e.AudioState == AudioState.Stopped && isListening)
            {
                BeginInvoke(new Action(() => {
                    StopListening();
                    System.Threading.Thread.Sleep(200);
                    StartListening();
                }));
            }
        }

        private void UpdateUIState()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUIState));
                return;
            }

            btnStartStop.Text = isListening ? "Stop Listening" : "Start Listening";
            
            if (recognizer == null)
            {
                UpdateStatus("Error: Speech recognition not available");
                btnStartStop.Enabled = false;
            }
            else if (synthesizer == null)
            {
                UpdateStatus("Speech recognition ready (text-to-speech not available)");
                btnSpeak.Enabled = false;
            }
            else if (!isListening)
            {
                UpdateStatus("Ready to listen");
            }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateStatus(message)));
                return;
            }

            lblStatus.Text = message;
            Debug.WriteLine($"Status: {message}");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            // Clean up resources
            StopListening();
            recognizer?.Dispose();
            synthesizer?.Dispose();
        }
    }
}
