using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemListener.EventArgs
{
    public class CreatedEventArgs<T> : System.EventArgs
    {
        public T CreatedItem { get; set; }
    }
}