using Markdig;

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


        // Markdig���g�p����Markdown��HTML�ɕϊ�
        // �V���v���ȕϊ�:
        // string htmlContent = Markdown.ToHtml(markdownText);

        // Markdig�̃p�C�v���C�����g�p���Ċg���@�\��L���ɂ��邱�Ƃ��\
        // �Ⴆ�΁AGitHub Flavored Markdown (GFM) �̂悤�Ȉ�ʓI�Ȋg���@�\�Z�b�g���g���ꍇ:
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        string htmlContent = Markdown.ToHtml(result, pipeline);

        string styledHtmlContent = $@"
            <html>
            <head>
                <meta charset='UTF-8'>
            </head>
            <body style='font-size: 0.8em'>
                {htmlContent}
            </body>
            </html>";

        webView.CoreWebView2.NavigateToString(styledHtmlContent); // �X�^�C������̏ꍇ
    }

    private async void MainForm_Shown(object sender, EventArgs e)
    {
        await webView.EnsureCoreWebView2Async(null);
    }
}
