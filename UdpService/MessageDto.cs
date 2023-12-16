using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UdpServiceLib
{
    public class MessageDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public override string ToString()
        {
            return $"{UserName} [{SentAt:T}]: {Text}";
        }
    }
}
