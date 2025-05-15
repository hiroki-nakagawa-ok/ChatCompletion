// Semantic Kernel のコア機能を使うための名前空間
using Microsoft.SemanticKernel;
// チャット補完に関連する機能（履歴、サービスインターフェースなど）を使うための名前空間
using Microsoft.SemanticKernel.ChatCompletion;
// Google AI (Geminiなど) に接続するためのコネクタを使うための名前空間
using Microsoft.SemanticKernel.Connectors.Google;

namespace ChatCompletion;

/// <summary>
/// Semantic Kernel を使用してチャット補完サービスを初期化し、管理するクラス。
/// </summary>
internal class ChatCompletionService
{
    // Gemini API にアクセスするための API キー。環境変数から取得を試みる。
    // 環境変数 'GEMINI_API_KEY' が設定されていない場合は空文字列になる（API連携は機能しない）。
    private static readonly string API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";

    // 使用する Gemini モデルの名前を定義。
    private static readonly string MODEL = "gemini-2.0-flash-lite";

    // Semantic Kernel の中心となるオブジェクト。AIモデルとの対話などを管理する。
    private readonly Kernel kernel;

    /// <summary>
    /// ChatCompletionService クラスの新しいインスタンスを初期化します。
    /// Kernel を構築し、Google AI Gemini コネクタを登録します。
    /// </summary>
    public ChatCompletionService()
    {
        // Kernel オブジェクトの構築を開始するビルダーを作成
        kernel = Kernel.CreateBuilder()
            // ビルダーに Google AI Gemini チャット補完サービスを追加
            // 指定されたモデルとAPIキーを使用してサービスを構成
            .AddGoogleAIGeminiChatCompletion(MODEL, API_KEY)
            // 設定に基づいて Kernel オブジェクトを構築し、kernel フィールドに代入
            .Build();
    }

    /// <summary>
    /// 新しいチャットセッションを開始します。
    /// </summary>
    /// <returns>新しい ChatTimeLime オブジェクト。これを使用してチャットのやり取りを行います。</returns>
    public ChatTimeLime StartChat()
    {
        // 現在の Kernel オブジェクトを使用して、新しいチャットセッションを管理する ChatTimeLime インスタンスを作成し、返す
        return new ChatTimeLime(kernel);
    }

    /// <summary>
    /// 個々のチャットセッションの状態（会話履歴）と、そのセッション内での AI との対話を管理する内部クラス。
    /// </summary>
    /// <param name="kernel">このセッションで使用する Semantic Kernel オブジェクト。</param>
    public class ChatTimeLime(Kernel kernel)
    {
        // このチャットセッションの会話履歴を保持するプロパティ。
        // ユーザーとアシスタントのメッセージが時系列で格納される。
        public ChatHistory History { get; } = new();

        /// <summary>
        /// ユーザーからのメッセージをチャット履歴に追加します。
        /// </summary>
        /// <param name="content">追加するユーザーメッセージのテキスト。</param>
        public void AddUserMessage(string content) => History.AddUserMessage(content);

        /// <summary>
        /// 現在のチャット履歴に基づいて AI に応答を生成させ、履歴に追加し、応答テキストを返します。
        /// ストリーミング応答の場合、応答が完了するまで複数回 AI を呼び出す場合があります。
        /// </summary>
        /// <returns>AI によって生成された応答のテキスト。</returns>
        public async Task<string> GetChatMessageContentsAsync()
        {
            // Kernel から登録されているチャット補完サービス（Google Gemini コネクタ）を取得
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // 今回の AI 応答のみを一時的に収集するための履歴オブジェクト
            var complations = new ChatHistory(); // 変数名 'complations' は 'completions' のタイポの可能性あり

            // AI の応答が完了するまでループ（ストリーミング応答などを考慮）
            // 応答メッセージのメタデータに FinishReason="stop" が含まれるまで繰り返す
            while (!complations.Any(mbox => "stop" == mbox?.Metadata?["FinishReason"]?.ToString()?.ToLower()))
            {
                // 現在の全履歴を渡して AI に応答生成をリクエスト
                // Gemini 固有の実行設定オブジェクトを使用
                // この呼び出しは、応答のチャンクを返す可能性がある
                var completion = await chatCompletionService.GetChatMessageContentsAsync(History, new GeminiPromptExecutionSettings(), kernel);

                // AI から取得した応答のチャンクを、メインのチャット履歴に追加
                History.AddRange(completion);

                // AI から取得した応答のチャンクを、今回の応答収集用の一時履歴に追加
                complations.AddRange(completion);
            }

            // 今回の AI 応答収集用の一時履歴 (complations) から、
            // アシスタント (AI) のメッセージのみを抽出し、
            // そのコンテンツを改行で結合して一つの文字列にする。
            // その後、改行コード (\n) を Windows 標準の改行コード (\r\n) に置換して返す。
            return string.Join("\n", complations.Where(content => content.Role == AuthorRole.Assistant)
                .Select(content => content.Content ?? "")).Replace("\n", "\r\n");
        }
    }
}
