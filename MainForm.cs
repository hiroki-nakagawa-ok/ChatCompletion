namespace ChatCompletion;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private async void sendButton_Click(object sender, EventArgs e)
    {
        var chat = new ChatCompletionService().StartChat();
        chat.AddUserMessage(this.inputText.Text);

        this.sendButton.Enabled = false;

        var result = await chat.GetChatMessageContentsAsync();

        this.sendButton.Enabled = true;

        this.outputText.Text = result;
    }
}
