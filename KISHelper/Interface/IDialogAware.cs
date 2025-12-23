using KISHelper.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KISHelper.Interface
{
    public interface IDialogAware
    {
        event Action RequestClose;
        bool CanCloseDialog(); // 可关闭检查
        void OnDialogClosed(); // 关闭后清理
    }

    public abstract class DialogViewModelBase : ViewModelBase,IDialogAware
    {
        public event Action? RequestClose;

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed()
        {
            // 清理资源
        }

        protected void RequestCloseDialog()
        {
            if (CanCloseDialog())
            {
                RequestClose?.Invoke();
                OnDialogClosed();
            }
        }
    }


}
