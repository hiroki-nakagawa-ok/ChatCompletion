namespace ChatCompletion
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
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
            inputText = new TextBox();
            sendButton = new Button();
            outputText = new TextBox();
            SuspendLayout();
            // 
            // inputText
            // 
            inputText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputText.Location = new Point(12, 12);
            inputText.Multiline = true;
            inputText.Name = "inputText";
            inputText.ScrollBars = ScrollBars.Both;
            inputText.Size = new Size(879, 75);
            inputText.TabIndex = 0;
            // 
            // sendButton
            // 
            sendButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sendButton.Location = new Point(897, 12);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(75, 75);
            sendButton.TabIndex = 1;
            sendButton.Text = "送信";
            sendButton.UseVisualStyleBackColor = false;
            sendButton.Click += sendButton_Click;
            // 
            // outputText
            // 
            outputText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputText.Location = new Point(12, 93);
            outputText.Multiline = true;
            outputText.Name = "outputText";
            outputText.ScrollBars = ScrollBars.Both;
            outputText.Size = new Size(960, 456);
            outputText.TabIndex = 2;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(outputText);
            Controls.Add(sendButton);
            Controls.Add(inputText);
            Name = "MainForm";
            Text = "AIとチャット";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox inputText;
        private Button sendButton;
        private TextBox outputText;
    }
}
