using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataModel
{
    public abstract class Media
    {
        private byte[] file;

        public int Id { get; set; }
        public byte[] File {
            get { return file; }
            set { file = value; if (file != null) { HasFile = true; } else { HasFile = false; } }
        }
        public string Title { get; set; }
        public bool HasFile { get; set; }
    }
}
