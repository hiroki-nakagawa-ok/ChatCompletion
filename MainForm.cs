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


        // Markdigを使用してMarkdownをHTMLに変換
        // シンプルな変換:
        // string htmlContent = Markdown.ToHtml(markdownText);

        // Markdigのパイプラインを使用して拡張機能を有効にすることも可能
        // 例えば、GitHub Flavored Markdown (GFM) のような一般的な拡張機能セットを使う場合:
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

        webView.CoreWebView2.NavigateToString(styledHtmlContent); // スタイルありの場合
    }

    private async void MainForm_Shown(object sender, EventArgs e)
    {
        await webView.EnsureCoreWebView2Async(null);
    }
}
