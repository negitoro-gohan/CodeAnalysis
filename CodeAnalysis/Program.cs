using CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CaslaySample
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string directoryPath = args[0];
                if (Directory.Exists(directoryPath))
                {
                    // ディレクトリ内の.csファイルを取得
                    string[] csFiles = GetCsFiles(directoryPath);
                    foreach (string csFile in csFiles)
                    {
                        //Console.WriteLine("ファイル名:" + csFile);
                        SyntaxAnalyzer syntaxAnalysis = new SyntaxAnalyzer();
                        syntaxAnalysis.GetSelectClauseWithoutOrderClause(csFile);
                    }
                }
                else
                {
                    Console.WriteLine("指定されたディレクトリは存在しません。");
                }
            }
            else
            {
                Console.WriteLine("使用法: プログラム名 <ディレクトリパス>");
            }
        }
        static string[] GetCsFiles(string directoryPath)
        {
            // ディレクトリ内のすべての.csファイルを再帰的に取得
            string[] csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
            return csFiles;
        }
    } 
}
