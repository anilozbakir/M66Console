using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
namespace TextReadWrite
{
    public class SysReadWrite
    {
        public string FileName;
        public string content;

        public SysReadWrite(string fileName)
        {
            FileName = fileName;
        }

        public bool Result { get; set; }

        public async  Task< string>  ReadText()
        {
            string result = "";
            try
            {
                using (FileStream FileRead = new FileStream(this.FileName, FileMode.Open))
                {


                    byte[] buffer = new byte[1000];
                    var token = CancellationToken.None;
                    while (true)
                    {
                        int i = await FileRead.ReadAsync(buffer, 0, 1000, token);
                        if (i != 0)
                        {
                            result += Encoding.ASCII.GetString(buffer.Take(i).ToArray());
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }
            catch (Exception)
            {

                this.Result = false;
                return "";
            }

            this.Result = true;
            this.content = result;
            return result;
        }
        public async Task<string> WriteText(string content)
        {
            string result = "";
            try
            {
                using (FileStream FileWrite = new FileStream(this.FileName, FileMode.Create))
                {


                    byte[] buffer = Encoding.ASCII.GetBytes(content);
                    var token = CancellationToken.None;
                    int offset = 0;
                    while (true) { 

                        int writeCount= Math.Min(buffer.Length - offset, 1000);
                        if (writeCount <= 0) break;
                        await FileWrite.WriteAsync(buffer, 0, writeCount, token);
                        offset += 1000;

                    }
                }
            }
            catch (Exception)
            {

                this.Result = false;
                return "";
            }

            this.Result = true;
            this.content = result;
            return result;
        }
    }
}
