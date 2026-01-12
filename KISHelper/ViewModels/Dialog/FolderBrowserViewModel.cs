using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using KISHelper.Common;

namespace KISHelper.ViewModels.Dialog
{
    public class FolderBrowserViewModel 
    {
        private string? _selectedPath;
        private readonly ObservableCollection<string> _recentPaths = new();

        public ObservableCollection<FolderItemViewModel> RootFolders { get; } = new();

        public string? SelectedPath
        {
            get => _selectedPath;
            set
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged();

                    // 添加到最近路径
                    if (!string.IsNullOrEmpty(value) && !_recentPaths.Contains(value))
                    {
                        _recentPaths.Insert(0, value);
                        if (_recentPaths.Count > 10)
                        {
                            _recentPaths.RemoveAt(_recentPaths.Count - 1);
                        }
                    }
                }
            }
        }


        public FolderBrowserViewModel()
        {
            LoadRootFolders();
            LoadRecentPaths();
        }

        private void LoadRecentPaths()
        {
            try
            {
                // 从配置文件或注册表加载最近路径
                _recentPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                _recentPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                _recentPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                _recentPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            }
            catch (Exception)
            {
                // 忽略加载错误
            }
        }

        private void LoadRootFolders()
        {
            RootFolders.Clear();

            try
            {
                // 添加特殊文件夹
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        RootFolders.Add(new FolderItemViewModel(this)
                        {
                            Name = $"{drive.Name} ({drive.VolumeLabel})",
                            FullPath = drive.RootDirectory.FullName,
                            IsDirectory = true
                        });
                    }
                }

                // 添加常用文件夹
                var specialFolders = new[]
                {
                    Environment.SpecialFolder.Desktop,
                    Environment.SpecialFolder.MyDocuments,
                    Environment.SpecialFolder.MyPictures,
                    Environment.SpecialFolder.MyMusic,
                    Environment.SpecialFolder.MyVideos
                };

                foreach (var folder in specialFolders)
                {
                    var path = Environment.GetFolderPath(folder);
                    if (!string.IsNullOrEmpty(path))
                    {
                        RootFolders.Add(new FolderItemViewModel(this)
                        {
                            Name = GetSpecialFolderName(folder),
                            FullPath = path,
                            IsDirectory = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载文件夹失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSpecialFolderName(Environment.SpecialFolder folder)
        {
            return folder switch
            {
                Environment.SpecialFolder.Desktop => "桌面",
                Environment.SpecialFolder.MyDocuments => "文档",
                Environment.SpecialFolder.MyPictures => "图片",
                Environment.SpecialFolder.MyMusic => "音乐",
                Environment.SpecialFolder.MyVideos => "视频",
                _ => folder.ToString()
            };
        }


        public void NavigateToPath(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    SelectedPath = path;
                }
                else
                {
                    MessageBox.Show("路径不存在或无法访问", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导航失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FolderItemViewModel : INotifyPropertyChanged
    {
        private readonly FolderBrowserViewModel _parent;
        private bool _isExpanded;
        private bool _isSelected;
        private ObservableCollection<FolderItemViewModel>? _children;

        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public string? LastModified { get; set; }
        public string? Type { get; set; }
        public string? Size { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                    if (value && IsDirectory)
                    {
                        LoadChildren();
                    }
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    if (value && IsDirectory)
                    {
                        _parent.NavigateToPath(FullPath);
                    }
                }
            }
        }

        public ObservableCollection<FolderItemViewModel> Children
        {
            get
            {
                if (_children == null && IsDirectory)
                {
                    _children = new ObservableCollection<FolderItemViewModel>();
                    // 延迟加载
                }
                return _children ??= new ObservableCollection<FolderItemViewModel>();
            }
        }

        public FolderItemViewModel(FolderBrowserViewModel parent)
        {
            _parent = parent;
        }

        

        public void LoadChildren()
        {
            if (!IsDirectory || _children == null || _children.Count > 0)
                return;

            try
            {
                var directoryInfo = new DirectoryInfo(FullPath);
                var directories = directoryInfo.GetDirectories();

                foreach (var dir in directories)
                {
                    if (dir.Attributes.HasFlag(FileAttributes.Hidden))
                        continue;

                    var child = new FolderItemViewModel(_parent)
                    {
                        Name = dir.Name,
                        FullPath = dir.FullName,
                        IsDirectory = true
                    };
                    _children.Add(child);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 无权限访问，跳过
            }
            catch (Exception)
            {
                // 其他异常，跳过
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}