using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SLLSFunction
{
    public static class FileIO {
        public static string FileTextInput(string filepath) {
            try {
                //ファイルを開く
                using(StreamReader sr = new StreamReader(filepath, Encoding.Default)) {
                    return sr.ReadToEnd();
                }
            } catch(FileNotFoundException ex) {
                //FileNotFoundExceptionをキャッチした時
                Console.WriteLine("ファイルが見つかりませんでした。");
                Console.WriteLine(ex.Message);
                return null;
            } catch(IOException ex) {
                //IOExceptionをキャッチした時
                Console.WriteLine("ファイルがロックされている可能性があります。");
                Console.WriteLine(ex.Message);
                return null;
            } catch(UnauthorizedAccessException ex) {
                //UnauthorizedAccessExceptionをキャッチした時
                Console.WriteLine("必要なアクセス許可がありません。");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static List<string> FileTextsInput(string filepath) {
            try {
                //ファイルを開く
                using(StreamReader sr = new StreamReader(filepath, Encoding.Default)) {
                    List<string> ret = new List<string>();
                    while(sr.EndOfStream == false) {
                        ret.Add(sr.ReadLine());
                    }
                    return ret;
                }
            } catch(FileNotFoundException ex) {
                //FileNotFoundExceptionをキャッチした時
                Console.WriteLine("ファイルが見つかりませんでした。");
                Console.WriteLine(ex.Message);
                return null;
            } catch(IOException ex) {
                //IOExceptionをキャッチした時
                Console.WriteLine("ファイルがロックされている可能性があります。");
                Console.WriteLine(ex.Message);
                return null;
            } catch(UnauthorizedAccessException ex) {
                //UnauthorizedAccessExceptionをキャッチした時
                Console.WriteLine("必要なアクセス許可がありません。");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void FileOutputText(string filepath, string text) {
            try {
                using(StreamWriter sw = new StreamWriter(filepath, false, Encoding.Default)) {
                    sw.WriteLine(text);
                }
            } catch(FileNotFoundException ex) {
                //FileNotFoundExceptionをキャッチした時
                Console.WriteLine("ファイルが見つかりませんでした。");
                Console.WriteLine(ex.Message);
                return;
            } catch(IOException ex) {
                //IOExceptionをキャッチした時
                Console.WriteLine("ファイルがロックされている可能性があります。");
                Console.WriteLine(ex.Message);
                return;
            } catch(UnauthorizedAccessException ex) {
                //UnauthorizedAccessExceptionをキャッチした時
                Console.WriteLine("必要なアクセス許可がありません。");
                Console.WriteLine(ex.Message);
                return;
            }
        }


        //ファイルから値を取り出す
        public static void FileRead(string fileAdress, List<DataArray> list) {
            const int ARRAYSIZE = 3;
            float[] result = new float[ARRAYSIZE];

            using (StreamReader sr = new StreamReader(fileAdress, Encoding.Default)) {
                while (sr.Peek() >= 0) {
                    string strBuffer = sr.ReadLine();
                    String[] stResult = strBuffer.Split(',');
                    for (int i = 0; i < ARRAYSIZE; i++) {
                        try {
                            result[i] = float.Parse(stResult[i]);
                        } catch (FormatException ex) {
                            Console.WriteLine(ex.Message);
                            return;
                        } catch (OverflowException ex) {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                    }
                    list.Add(new DataArray(result[0], result[1], result[2]));
                }
            }           
        }

        public static void DeleteDirectryFile(string directry) {
            string[] files = Directory.GetFiles(directry);
            foreach(string file in files) {
                File.Delete(file);
            }
        }

        public static void FileOutPutTexts(string filepath, List<DataArray> Texts) {
            try {
                using(StreamWriter sw = new StreamWriter(filepath, false, Encoding.Default)) {
                    foreach(DataArray Text in Texts) {
                        sw.WriteLine(Text.X + "," + Text.Y + "," + Text.Z);
                    }
                }
            } catch(FileNotFoundException ex) {
                //FileNotFoundExceptionをキャッチした時
                Console.WriteLine("ファイルが見つかりませんでした。");
                Console.WriteLine(ex.Message);
                return;
            } catch(IOException ex) {
                //IOExceptionをキャッチした時
                Console.WriteLine("ファイルがロックされている可能性があります。");
                Console.WriteLine(ex.Message);
                return;
            } catch(UnauthorizedAccessException ex) {
                //UnauthorizedAccessExceptionをキャッチした時
                Console.WriteLine("必要なアクセス許可がありません。");
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
