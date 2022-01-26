using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCPExtension
{
    public interface IDispatcher
    {
        void Invoke(Action callback);

        Task InvokeAsync(Action callback);
    }
}