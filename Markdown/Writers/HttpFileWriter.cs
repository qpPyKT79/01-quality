using System.IO;

namespace Markdown
{
    public class HttpFileWriter : IWriter
    {
        private const string HttpHeader = "<meta http-equiv = \"Content-Type\" content = \"text/html; charset=utf-8\">";

        public void Write(string text, string name) => 
            File.WriteAllText(name, HttpHeader + text);
    }
}
