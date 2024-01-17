using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace CaslaySample
{
    public class SampleSyntaxWalker : SyntaxWalker
    {
        private SemanticModel semanticModel;

        /// <summary>
        /// コードの解析を行い、builderに記録します
        /// </summary>
        public void Analyze(string code)
        {
            // ①単体のソースコードを構文解析します。
            var tree = CSharpSyntaxTree.ParseText(code);


            // 各トークンを見ていく
            foreach (var token in tree.GetRoot().DescendantTokens())
            {
                this.VisitToken(token);
            }
        }
        public void GetSyntax(string sourceCode)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetRoot();

            var methodNodes = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var methodNode in methodNodes)
            {
                var methodName = methodNode.Identifier.Text;

                // メソッド内のすべての Append メソッド呼び出しを取得
                var appendMethodInvocations = methodNode.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(IsAppendMethod);

                Console.WriteLine(methodName);
                bool selectflag;
                bool orderflag;
                selectflag = false;
                orderflag = true;
                foreach (var appendInvocation in appendMethodInvocations)
                {
                    // 引数リスト内に "select" が含まれ、かつ "order" が含まれない場合、メソッド名を出力
                    var argumentList = appendInvocation.ArgumentList;

                    Console.WriteLine(argumentList.ToString());
                    if (argumentList.ToString().ToLower().Contains("select"))
                    {
                        selectflag = true;
                    }
                    if (argumentList.ToString().ToLower().Contains("order"))
                    {
                        orderflag = false;
                    }
                     
                }
                if (selectflag && orderflag)
                {
                    Console.WriteLine("match:" + methodName);
                }
            }

        }
        // Appendメソッドかどうかを判定するメソッド
        static bool IsAppendMethod(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName == "Append" || methodName == "AppendLine")
                {
                    // Appendメソッドの呼び出し
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// ④VisitTokenメソッドによるトークンに対する処理
        /// </summary>
        /// <param name="token"></param>
        protected override void VisitToken(SyntaxToken token)
        {
            // 詳細を持っているトークンである場合
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }

            bool isProcessed = false;

            // キーワードであるか
            if (token.IsKeyword())
            {
                Console.WriteLine($"{"Keyword",-30}：{token.ValueText}");
                isProcessed = true;

            }
            else
            {
                Console.WriteLine($"{token.Kind(),-30}：{token.ValueText}");
                isProcessed = true;
            }

            // それ以外の項目
            if (!isProcessed)
            {
                Console.WriteLine($"{"Etc",-30}：{token.ValueText}");
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }

        }

        /// <summary>
        /// ⑤VisitTriviaメソッドによるトークンのトリビアに対する処理
        /// </summary>
        /// <param name="trivia"></param>
        protected override void VisitTrivia(SyntaxTrivia trivia)
        {
            // 空白と改行の場合は無視
            if (trivia.Kind() == SyntaxKind.WhitespaceTrivia || trivia.Kind() == SyntaxKind.EndOfLineTrivia)
            {
                return;
            }

            Console.WriteLine($"{trivia.Kind(),-30}：{trivia.ToFullString()}");
            base.VisitTrivia(trivia);
        }

    }
}