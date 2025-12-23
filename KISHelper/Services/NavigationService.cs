
namespace KISHelper.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    public class NavigationService : INotifyPropertyChanged
    {
        private static NavigationService _instance;
        public static NavigationService Instance => _instance ??= new NavigationService();

        // 类型注册表（仅注册类型）
        private readonly Dictionary<string, Type> _viewModelRegistry = new();

        // 实例缓存表（关键！）
        private readonly Dictionary<string, object> _viewModelCache = new();

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        // 注册页面
        public void Register<TViewModel>(string name) where TViewModel : new()
        {
            _viewModelRegistry[name] = typeof(TViewModel);
        }

        public void Register(string name, Type viewModelType)
        {
            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(viewModelType))
                throw new ArgumentException("ViewModel 必须实现 INotifyPropertyChanged");

            _viewModelRegistry[name] = viewModelType;
        }

        //  修改后的导航方法
        public void NavigateTo(string pageName)
        {
            if (!_viewModelRegistry.TryGetValue(pageName, out var viewModelType))
                throw new KeyNotFoundException($"页面 '{pageName}' 未注册");

            // 检查缓存中是否已有实例
            if (!_viewModelCache.TryGetValue(pageName, out var instance))
            {
                // 没有则创建并缓存
                instance = Activator.CreateInstance(viewModelType);
                _viewModelCache[pageName] = instance;
            }

            CurrentView = instance;
        }

        // 手动清除指定页面缓存（用于需要刷新数据的场景）
        public void ClearCache(string pageName)
        {
            _viewModelCache.Remove(pageName);
        }

        // 清除所有缓存
        public void ClearAllCache() => _viewModelCache.Clear();

        // 清除注册表
        public void ClearRegistry()
        {
            _viewModelRegistry.Clear();
            ClearAllCache(); // 同时清除缓存
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public object GetCachedViewModel(string pageName)
        {
            _viewModelCache.TryGetValue(pageName, out var instance);
            return instance;
        }
    }

        // 子导航服务
    public class ChildNavigationService : INotifyPropertyChanged
        {
            private object _currentView;
            public object CurrentView
            {
                get => _currentView;
                set
                {
                    _currentView = value;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }

            private readonly Dictionary<string, Type> _viewModelRegistry = new();
            // 新增缓存
            private readonly Dictionary<string, object> _viewModelCache = new();

            public void Register<TViewModel>(string name) where TViewModel : new()
            {
                _viewModelRegistry[name] = typeof(TViewModel);
            }

            public void NavigateTo(string pageName)
            {
                if (!_viewModelRegistry.TryGetValue(pageName, out var viewModelType))
                    throw new KeyNotFoundException($"子页面 '{pageName}' 未注册");

                if (!_viewModelCache.TryGetValue(pageName, out var instance))
                {
                    instance = Activator.CreateInstance(viewModelType);
                    _viewModelCache[pageName] = instance;
                }

                CurrentView = instance;
            }

            public void ClearCache(string pageName) => _viewModelCache.Remove(pageName);
            public void ClearAllCache() => _viewModelCache.Clear();

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }


    }


