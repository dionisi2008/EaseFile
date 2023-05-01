using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace EaseFile
{
    public class АПИ
    {
        public System.Net.HttpListener ВебСервер;
        public Dictionary<string, EaseFileBase> СписокБаз;
        protected string Логин;
        protected string Пароль;

        public АПИ()
        {
            Process[] processes = Process.GetProcessesByName("code");
            foreach (Process process in processes)
            {
                process.Kill(); // or process.CloseMainWindow()
            }



            string[] ФайлКонфигураций = File.ReadAllLines("Setting.txt");
            Логин = ФайлКонфигураций[2];
            Пароль = ФайлКонфигураций[3];
            СписокБаз = new Dictionary<string, EaseFileBase>();
            ВебСервер = new HttpListener();
            ВебСервер.Prefixes.Add("http://" + ФайлКонфигураций[0] + ":" + ФайлКонфигураций[1] + "/");
            for (int shag = 4; shag <= ФайлКонфигураций.Length - 1; shag++)
            {
                СписокБаз.Add(ФайлКонфигураций[shag].Split(' ')[0], new EaseFileBase(ФайлКонфигураций[shag].Split(' ')[1]));
            }
            ВебСервер.Start();
            do
            {
                HttpListenerContext КонтекстЗапроса = ВебСервер.GetContext();
                Console.WriteLine(КонтекстЗапроса.Request.HttpMethod);
                if (КонтекстЗапроса.Request.HttpMethod == "POST")
                {
                    byte[] МассивБайтДляЧтенияЗапроса = new byte[1024];
                    КонтекстЗапроса.Request.InputStream.Read(МассивБайтДляЧтенияЗапроса, 0, 1024);
                    string[] СыройРазобранныйЗапрос = UTF8Encoding.UTF8.GetString(МассивБайтДляЧтенияЗапроса).Split(' ');
                    List<string> ПодготовительныйРазобранныйЗапрос = new List<string>();
                    for (int shag = 0; shag <= СыройРазобранныйЗапрос.Length - 1; shag++)
                    {
                        if (СыройРазобранныйЗапрос[shag] != "")
                        {
                            ПодготовительныйРазобранныйЗапрос.Add(СыройРазобранныйЗапрос[shag]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    string[] РазобранныйЗапрос = ПодготовительныйРазобранныйЗапрос.ToArray();
                    int РазмерЗапроса = UTF8Encoding.UTF8.GetBytes(string.Join(' ', РазобранныйЗапрос)).Length - 1;
                    if (РазобранныйЗапрос.Length >= 3)
                    {
                        // Login Pass base Function
                        if (РазобранныйЗапрос[0] == Логин & РазобранныйЗапрос[1] == Пароль & СписокБаз.ContainsKey(РазобранныйЗапрос[2]))
                        {
                            switch (РазобранныйЗапрос[3])
                            {

                                case "ЗаписьДанных":
                                    // Login Pass base ЗаписьДанных Индификатор ЧислоБайтЗаписываемыхДанных//данные//
                                    Console.WriteLine(string.Join('\n', РазобранныйЗапрос));
                                    byte[] МассивСчитанныхДанныхДляЗаписиВБазу = new byte[КонтекстЗапроса.Request.InputStream.Length - РазмерЗапроса];
                                    КонтекстЗапроса.Request.InputStream.Read(МассивСчитанныхДанныхДляЗаписиВБазу, РазмерЗапроса, МассивСчитанныхДанныхДляЗаписиВБазу.Length);
                                    this.СписокБаз[РазобранныйЗапрос[2]].ЗаписьДанных(РазобранныйЗапрос[4], МассивСчитанныхДанныхДляЗаписиВБазу.ToArray());

                                    break;
                                case "ЗапроситьСписокИндификаторов":
                                    byte[] ОтветНаЗапрос = System.Text.UTF8Encoding.UTF8.GetBytes(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join('\n', this.СписокБаз[РазобранныйЗапрос[2]].ЗапроситьСписокИндификаторов()))));
                                    КонтекстЗапроса.Response.ContentLength64 = ОтветНаЗапрос.Length;
                                    КонтекстЗапроса.Response.OutputStream.Write(new ReadOnlySpan<byte>(ОтветНаЗапрос));
                                    break;
                                case "СчитатьДанные":

                                    Console.WriteLine('\n' + "Start " + DateTime.Now.ToString());
                                    byte[] ОтветНаЗапросСчитатьДанные = this.СписокБаз[РазобранныйЗапрос[2]].СчитатьДанные(РазобранныйЗапрос[4]);
                                    Console.WriteLine("Выгрузка из базы: " + DateTime.Now.ToString());
                                    КонтекстЗапроса.Response.ContentLength64 = ОтветНаЗапросСчитатьДанные.Length;
                                    Console.WriteLine("Начало Записи: " + DateTime.Now.ToString());
                                    КонтекстЗапроса.Response.OutputStream.Write(new ReadOnlySpan<byte>(ОтветНаЗапросСчитатьДанные));
                                    Console.WriteLine("Конец Записи: " + DateTime.Now.ToString());
                                    КонтекстЗапроса.Response.OutputStream.Close();
                                    break;
                            }


                        }
                    }

                }
                КонтекстЗапроса.Response.Close();
                Console.WriteLine("Сессия закрыта: " + DateTime.Now.ToString());
            } while (ВебСервер.IsListening == true);


        }

        public byte[] Base64(string ИсходнаяСтрока)
        {
            return System.Convert.FromBase64String(ИсходнаяСтрока);
        }

        public string Base64(byte[] ИсходныеДанные)
        {
            return System.Convert.ToBase64String(ИсходныеДанные);
        }


    }


}

