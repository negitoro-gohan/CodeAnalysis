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
        // ORDER句が設定されていない、SELECT句を抽出する。そのために、各メソッド毎に、Appendメソッドのみを抽出し、SELECTが含まれる、かつ、ORDERが含まれない箇所を出力する
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
                bool selectFlag;
                bool orderFlag;
                bool updateFlag;
                bool insertFlag;
                bool deleteFlag;
                bool fromdualFlag;
                selectFlag = false;
                orderFlag = true;
                updateFlag = true;
                insertFlag = true;
                deleteFlag = true;
                fromdualFlag = true;
                StringBuilder targetSql = new StringBuilder();
                int lineNumber = 0;
                foreach (var appendInvocation in appendMethodInvocations)
                {

                    var argumentList = appendInvocation.ArgumentList;

                    targetSql.Append(RemoveNouseAppendChar(argumentList.ToString()));

                    if (lineNumber == 0)
                    {
                        //該当箇所の行番号の取得
                        lineNumber = tree.GetLineSpan(argumentList.Span).StartLinePosition.Line + 1;
                    }
                    if (argumentList.ToString().ToLower().Contains("select"))
                    {
                        selectFlag = true;
                    }
                    if (argumentList.ToString().ToLower().Contains("order"))
                    {
                        orderFlag = false;
                    }
                    if (argumentList.ToString().ToLower().Contains("update"))
                    {
                        updateFlag = false;
                    }
                    if (argumentList.ToString().ToLower().Contains("insert"))
                    {
                        insertFlag = false;
                    }
                    if (argumentList.ToString().ToLower().Contains("delete"))
                    {
                        deleteFlag = false;
                    }
                    if (argumentList.ToString().ToLower().Contains("from dual"))
                    {
                        fromdualFlag = false;
                    }

                }

                // Append メソッドリスト内に "select" が含まれ、かつ "order" が含まれない場合、該当箇所を出力する
                if (selectFlag && orderFlag)
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
        static string RemoveNouseAppendChar(string input)
        {
            // Apepndに続く不要なかっことダブルクォーテーションを削除する
            input = input.Substring(1, input.Length - 2).Trim();
            return input.Substring(1, input.Length - 2);
        }
    }
}
