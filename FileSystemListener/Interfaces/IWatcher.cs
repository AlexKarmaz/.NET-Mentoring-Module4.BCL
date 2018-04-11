using FileSystemListener.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemListener.Interfaces
{
    public interface IWatcher<T>
    {
        event EventHandler<CreatedEventArgs<T>> Created;
    }
}
