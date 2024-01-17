using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    internal class SyntaxAnalyzer
    {
        public void GetSelectClauseWithoutOrderClause(string filePath)
        {

            var code = File.ReadAllText(filePath);

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var methodNodes = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            //メソッド毎に該当箇所があるかチェックを行う
            foreach (var methodNode in methodNodes)
            {
                var methodName = methodNode.Identifier.Text;

                // メソッド内のすべての Append メソッド呼び出しを取得
                var appendMethodInvocations = methodNode.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(IsAppendMethod);

                // Append メソッドリスト内に "select" が含まれ、かつ "order" が含まれない場合があるかチェックする
                bool selectflag;
                bool orderflag;
                selectflag = false;
                orderflag = true;
                StringBuilder targetSql = new StringBuilder();
                foreach (var appendInvocation in appendMethodInvocations)
                {

                    var argumentList = appendInvocation.ArgumentList;

                    targetSql.Append(argumentList.ToString().Replace(@"(""""", "").Replace(@""""")", ""));

                    if (argumentList.ToString().ToLower().Contains("select"))
                    {
                        selectflag = true;
                    }
                    if (argumentList.ToString().ToLower().Contains("order"))
                    {
                        orderflag = false;
                    }

                }

                // Append メソッドリスト内に "select" が含まれ、かつ "order" が含まれない場合、該当箇所を出力する
                if (selectflag && orderflag)
                {
                    Console.WriteLine("ファイル名:" + filePath);
                    Console.WriteLine("該当メソッド名:" + methodName);
                    Console.WriteLine("該当箇所:" + targetSql.ToString()); 

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
    }
}
