using System;
using System.IO;
using System.Collections;


namespace EaseFile // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            АПИ БетаАПИ;
            Console.WriteLine("Hello World!");
            if (args.Length >= 1)
            {
                if (args[0] == "Debug")
                {
                    БетаАПИ = new АПИ(true);
                }
            }

            else
            {
                БетаАПИ = new АПИ(false);
            }

            // EaseFileBase EaseFile = new EaseFileBase("test.bin");
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // EaseFile.ЗаписьДанных("123", File.ReadAllBytes("123.txt"));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // EaseFile.ПерезаписатьДанные("123", EaseFile.КонвертированиеСтрокиВБайты(DateTime.Now.ToString()));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // EaseFile.УдалитьДанные("123");
            // EaseFile.ЗаписьДанных("123", File.ReadAllBytes("123.txt"));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));
            // Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(EaseFile.СчитатьДанные("123")));

        }
    }
}