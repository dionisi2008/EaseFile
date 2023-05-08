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

                if (КонтекстЗапроса.Request.HttpMethod == "POST")
                {
                    Span<byte> БуферСчитанныхБайт = new byte[1073741824];
                    byte[] МассивСчитанныхБайтВЗавпросе = new byte[КонтекстЗапроса.Request.InputStream.ReadAtLeast(БуферСчитанныхБайт, 1073741824, false)];
                    Array.Copy(БуферСчитанныхБайт.ToArray(), 0, МассивСчитанныхБайтВЗавпросе, 0, МассивСчитанныхБайтВЗавпросе.Length);
                    byte[] МассивБайтДляЧтенияЗапроса = new byte[32768];
                    byte[] ПолезныеДанные = new byte[МассивСчитанныхБайтВЗавпросе.Length - 32768];
                    КонтекстЗапроса.Request.InputStream.Read(МассивСчитанныхБайтВЗавпросе);
                    Array.Copy(МассивСчитанныхБайтВЗавпросе.ToArray(), 0, МассивБайтДляЧтенияЗапроса, 0, 32768);
                    Array.Copy(МассивСчитанныхБайтВЗавпросе.ToArray(), 32768, ПолезныеДанные, 0, ПолезныеДанные.Length);
                    string[] СыройРазобранныйЗапрос = UTF8Encoding.UTF8.GetString(МассивБайтДляЧтенияЗапроса).Split('.')[0].Split(' ');
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
                    int РазмерЗапроса = UTF8Encoding.UTF8.GetBytes(string.Join(' ', ПодготовительныйРазобранныйЗапрос)).Length - 1;
                    if (ПодготовительныйРазобранныйЗапрос.Count >= 3)
                    {
                        // Login Pass base Function
                        if (ПодготовительныйРазобранныйЗапрос[0] == Логин & ПодготовительныйРазобранныйЗапрос[1] == Пароль & СписокБаз.ContainsKey(ПодготовительныйРазобранныйЗапрос[2]))
                        {
                            switch (ПодготовительныйРазобранныйЗапрос[3])
                            {

                                case "ЗаписьДанных":
                                    // Login Pass base ЗаписьДанных Индификатор//данные//

                                    this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].ЗаписьДанных(ПодготовительныйРазобранныйЗапрос[4], ПолезныеДанные);
                                    // Console.WriteLine(DateTime.Now.ToString() + " ЗаписьДанных");
                                    break;
                                case "ЗапроситьСписокИндификаторов":
                                    byte[] ОтветНаЗапрос = System.Text.Encoding.UTF8.GetBytes(string.Join('\n', this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].ЗапроситьСписокИндификаторов()));
                                    КонтекстЗапроса.Response.OutputStream.Write(new ReadOnlySpan<byte>(ОтветНаЗапрос));
                                    // Console.WriteLine(DateTime.Now.ToString() + " ЗапроситьСписокИндификаторов");
                                    break;
                                case "СчитатьДанные":
                                    byte[] ОтветНаЗапросСчитатьДанные = this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].СчитатьДанные(ПодготовительныйРазобранныйЗапрос[4]);
                                    КонтекстЗапроса.Response.ContentLength64 = ОтветНаЗапросСчитатьДанные.Length;
                                    КонтекстЗапроса.Response.OutputStream.Write(new ReadOnlySpan<byte>(ОтветНаЗапросСчитатьДанные));
                                    КонтекстЗапроса.Response.OutputStream.Close();
                                    // Console.WriteLine(DateTime.Now.ToString() + " СчитатьДанные");
                                    break;
                                case "МножественноеЧтение":
                                    byte[] РазобранныйЗапросИзБэйс64 = System.Convert.FromBase64String(ПодготовительныйРазобранныйЗапрос[4]);
                                    string[] ИсходнаяСписокСАйдишникамиДляЗапроса = Encoding.UTF8.GetString(РазобранныйЗапросИзБэйс64).Split('\n');

                                    List<string> ДанныйБэйс64ДлыВывода = new List<string>();
                                    for (int shag = 0; shag <= ИсходнаяСписокСАйдишникамиДляЗапроса.Length - 1; shag++)
                                    {
                                        if (this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].ПоискУникальныхИдентификаторов(ИсходнаяСписокСАйдишникамиДляЗапроса[shag]))
                                        {
                                            byte[] ИсходныеБайтыДляКодирования = this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].СчитатьДанные(ИсходнаяСписокСАйдишникамиДляЗапроса[shag]);
                                            ДанныйБэйс64ДлыВывода.Add(System.Convert.ToBase64String(ИсходныеБайтыДляКодирования));
                                        }
                                    }
                                    if (ДанныйБэйс64ДлыВывода.Count >= 1)
                                    {
                                        byte[] ОтветНаЗапросМножественноеЧтение = Encoding.UTF8.GetBytes(string.Join('\n', ДанныйБэйс64ДлыВывода));
                                        КонтекстЗапроса.Response.ContentLength64 = ОтветНаЗапросМножественноеЧтение.Length;
                                        КонтекстЗапроса.Response.OutputStream.Write(new ReadOnlySpan<byte>(ОтветНаЗапросМножественноеЧтение));
                                        КонтекстЗапроса.Response.OutputStream.Close();
                                        // Console.WriteLine(string.Join('\n', ДанныйБэйс64ДлыВывода));
                                    }
                                    else
                                    {
                                        КонтекстЗапроса.Response.OutputStream.Close();
                                    }

                                    break;
                                case "ПерезаписатьДанные":
                                    this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].ПерезаписатьДанные(ПодготовительныйРазобранныйЗапрос[4], ПолезныеДанные);
                                    // Console.WriteLine(DateTime.Now.ToString() + " ПерезаписатьДанные");
                                    break;
                                case "ПереименоватьИндификатор":
                                    this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].ПереименоватьИндификатор(ПодготовительныйРазобранныйЗапрос[4], ПодготовительныйРазобранныйЗапрос[5]);
                                    // Console.WriteLine(DateTime.Now.ToString() + " ПереименоватьИндификатор");
                                    break;
                                case "УдалитьДанные":
                                    this.СписокБаз[ПодготовительныйРазобранныйЗапрос[2]].УдалитьДанные(ПодготовительныйРазобранныйЗапрос[4]);
                                    // Console.WriteLine(DateTime.Now.ToString() + " УдалитьДанные");
                                    break;

                            }


                        }
                    }

                }
                КонтекстЗапроса.Response.Close();

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

