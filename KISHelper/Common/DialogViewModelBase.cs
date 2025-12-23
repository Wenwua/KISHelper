using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KISHelper.Common
{
    /// <summary>
    /// 对话框ViewModel基类，支持关闭事件和属性通知
    /// </summary>
    public abstract class DialogViewModelBase : ViewModelBase, IDialogAware
    {
        // ===== 对话框关闭事件 =====
        public event EventHandler<DialogCloseEventArgs>? RequestClose;

        // ===== 关闭对话框（供子类调用） =====
        /// <summary>
        /// 请求关闭对话框
        /// </summary>
        /// <param name="dialogResult">对话框结果 true/false/null</param>
        protected void CloseDialog(bool? dialogResult = null)
        {
            if (CanCloseDialog())
            {
                RequestClose?.Invoke(this, new DialogCloseEventArgs(dialogResult));
                OnDialogClosed();
            }
        }

        // ===== 可重写方法 =====
        /// <summary>
        /// 关闭前检查，返回false可阻止关闭
        /// </summary>
        protected virtual bool CanCloseDialog() => true;

        /// <summary>
        /// 对话框关闭后执行的清理操作
        /// </summary>
        protected virtual void OnDialogClosed() { }

        // ===== 便捷关闭命令（可选） =====
        private RelayCommand? _closeCommand;
        public RelayCommand CloseCommand => _closeCommand ??= new RelayCommand(() => CloseDialog(null));
    }

    /// <summary>
    /// 对话框关闭事件参数
    /// </summary>
    public class DialogCloseEventArgs : EventArgs
    {
        public bool? DialogResult { get; }
        public DialogCloseEventArgs(bool? result) => DialogResult = result;
    }

    /// <summary>
    /// 对话框接口（可选，用于依赖注入）
    /// </summary>
    public interface IDialogAware
    {
        event EventHandler<DialogCloseEventArgs> RequestClose;
    }
}