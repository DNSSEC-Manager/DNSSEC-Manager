using System.IO;
using System.Text;

namespace Providers.Providers.Openprovider.Dto
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
