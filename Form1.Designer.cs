namespace SpeechToVoiceApp;

partial class Form1
{
    private System.Windows.Forms.Button btnStartStop;
    private System.Windows.Forms.TextBox txtRecognizedText;
    private System.Windows.Forms.Label lblSpeechRecognition;
    private System.Windows.Forms.Label lblTextToSpeech;
    private System.Windows.Forms.TextBox txtTextToSpeak;
    private System.Windows.Forms.Button btnSpeak;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        btnStartStop = new Button();
        txtRecognizedText = new TextBox();
        lblSpeechRecognition = new Label();
        lblTextToSpeech = new Label();
        txtTextToSpeak = new TextBox();
        btnSpeak = new Button();
        panel1 = new Panel();
        panel2 = new Panel();
        lblStatus = new Label();
        toolTip1 = new ToolTip(components);
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        SuspendLayout();
        // 
        // btnStartStop
        // 
        btnStartStop.BackColor = Color.FromArgb(0, 122, 204);
        btnStartStop.FlatAppearance.BorderSize = 0;
        btnStartStop.FlatStyle = FlatStyle.Flat;
        btnStartStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnStartStop.ForeColor = Color.White;
        btnStartStop.Location = new Point(12, 234);
        btnStartStop.Name = "btnStartStop";
        btnStartStop.Size = new Size(710, 50);
        btnStartStop.TabIndex = 0;
        btnStartStop.Text = "🎤 Start Listening";
        toolTip1.SetToolTip(btnStartStop, "Click to start/stop speech recognition");
        btnStartStop.UseVisualStyleBackColor = false;
        btnStartStop.Click += btnStartStop_Click;
        // 
        // txtRecognizedText
        // 
        txtRecognizedText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtRecognizedText.BackColor = Color.White;
        txtRecognizedText.BorderStyle = BorderStyle.FixedSingle;
        txtRecognizedText.Font = new Font("Segoe UI", 10F);
        txtRecognizedText.Location = new Point(20, 40);
        txtRecognizedText.Multiline = true;
        txtRecognizedText.Name = "txtRecognizedText";
        txtRecognizedText.ScrollBars = ScrollBars.Vertical;
        txtRecognizedText.Size = new Size(675, 200);
        txtRecognizedText.TabIndex = 1;
        toolTip1.SetToolTip(txtRecognizedText, "Displays recognized speech");
        // 
        // lblSpeechRecognition
        // 
        lblSpeechRecognition.AutoSize = true;
        lblSpeechRecognition.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSpeechRecognition.Location = new Point(20, 20);
        lblSpeechRecognition.Name = "lblSpeechRecognition";
        lblSpeechRecognition.Size = new Size(99, 15);
        lblSpeechRecognition.TabIndex = 2;
        lblSpeechRecognition.Text = "SPEECH TO TEXT";
        // 
        // lblTextToSpeech
        // 
        lblTextToSpeech.AutoSize = true;
        lblTextToSpeech.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTextToSpeech.Location = new Point(20, 20);
        lblTextToSpeech.Name = "lblTextToSpeech";
        lblTextToSpeech.Size = new Size(91, 15);
        lblTextToSpeech.TabIndex = 3;
        lblTextToSpeech.Text = "TEXT TO VOICE";
        // 
        // txtTextToSpeak
        // 
        txtTextToSpeak.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTextToSpeak.BorderStyle = BorderStyle.FixedSingle;
        txtTextToSpeak.Font = new Font("Segoe UI", 10F);
        txtTextToSpeak.Location = new Point(20, 40);
        txtTextToSpeak.Multiline = true;
        txtTextToSpeak.Name = "txtTextToSpeak";
        txtTextToSpeak.Size = new Size(675, 100);
        txtTextToSpeak.TabIndex = 4;
        toolTip1.SetToolTip(txtTextToSpeak, "Enter text to be spoken");
        // 
        // btnSpeak
        // 
        btnSpeak.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSpeak.BackColor = Color.FromArgb(0, 192, 0);
        btnSpeak.FlatAppearance.BorderSize = 0;
        btnSpeak.FlatStyle = FlatStyle.Flat;
        btnSpeak.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSpeak.ForeColor = Color.White;
        btnSpeak.Location = new Point(12, 12);
        btnSpeak.Name = "btnSpeak";
        btnSpeak.Size = new Size(710, 50);
        btnSpeak.TabIndex = 5;
        btnSpeak.Text = "🔊 Speak";
        toolTip1.SetToolTip(btnSpeak, "Click to speak the entered text");
        btnSpeak.UseVisualStyleBackColor = false;
        btnSpeak.Click += btnSpeak_Click;
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panel1.BackColor = Color.White;
        panel1.BorderStyle = BorderStyle.FixedSingle;
        panel1.Controls.Add(lblTextToSpeech);
        panel1.Controls.Add(txtTextToSpeak);
        panel1.Location = new Point(12, 68);
        panel1.Name = "panel1";
        panel1.Padding = new Padding(10);
        panel1.Size = new Size(710, 160);
        panel1.TabIndex = 6;
        // 
        // panel2
        // 
        panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panel2.BackColor = Color.White;
        panel2.BorderStyle = BorderStyle.FixedSingle;
        panel2.Controls.Add(lblStatus);
        panel2.Controls.Add(lblSpeechRecognition);
        panel2.Controls.Add(txtRecognizedText);
        panel2.Location = new Point(12, 289);
        panel2.Name = "panel2";
        panel2.Padding = new Padding(10);
        panel2.Size = new Size(710, 260);
        panel2.TabIndex = 7;
        // 
        // lblStatus
        // 
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblStatus.AutoSize = true;
        lblStatus.ForeColor = Color.Gray;
        lblStatus.Location = new Point(134, 20);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 8;
        lblStatus.Text = "Ready";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(240, 240, 240);
        ClientSize = new Size(734, 561);
        Controls.Add(panel2);
        Controls.Add(btnSpeak);
        Controls.Add(panel1);
        Controls.Add(btnStartStop);
        MinimumSize = new Size(750, 600);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "🎙️ Speech to Voice App";
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        ResumeLayout(false);
    }

    #endregion
}
