using System;
using System.Threading.Tasks;

namespace AutoRing_SIB
{
    public interface IDialogService
    {
        void ShowProgressDialog(string message, Func<ISimpleProgress, Task> workAsync);
        void ShowProgressDialog(string message, int maximum, Func<ISimpleProgress, Task> workAsync);
    }
}
