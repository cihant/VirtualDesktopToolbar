using CSDeskBand;
using CSDeskBand.Annotations;
using CSDeskBand.ContextMenu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WindowsDesktop;

namespace VirtualDesktopToolbar
{
    /// <summary>
    /// Example WPF deskband. Shows taskbar info capabilities and context menus
    /// </summary>
    [ComVisible(true)]
    [Guid("F6C0EABF-1019-41C6-BCEE-BC4C2CFA2D06")]
    [CSDeskBandRegistration(Name = "Virtual Desktop DeskBand", ShowDeskBand = true)]
    public partial class UserControl1 : INotifyPropertyChanged
    {
        private Orientation _taskbarOrientation;
        private Orientation _wrapPanelOrientation;
        private int _taskbarWidth;
        private int _taskbarHeight;
        private Edge _taskbarEdge;
        private int rowCount = 2;
        public Orientation TaskbarOrientation
        {
            get => _taskbarOrientation;
            set
            {
                if (value == _taskbarOrientation) return;
                _taskbarOrientation = value;
                OnPropertyChanged();
            }
        }
        public Orientation WrapPanelOrientation
        {
            get => _wrapPanelOrientation;
            set
            {
                if (value == _wrapPanelOrientation) return;
                _wrapPanelOrientation = value;
                OnPropertyChanged();
            }
        }

        public int TaskbarWidth
        {
            get => _taskbarWidth;
            set
            {
                if (value == _taskbarWidth) return;
                _taskbarWidth = value;
                OnPropertyChanged();
            }
        }

        public int TaskbarHeight
        {
            get => _taskbarHeight;
            set
            {
                if (value == _taskbarHeight) return;
                _taskbarHeight = value;
                OnPropertyChanged();
            }
        }

        public Edge TaskbarEdge
        {
            get => _taskbarEdge;
            set
            {
                if (value == _taskbarEdge) return;
                _taskbarEdge = value;
                OnPropertyChanged();
            }
        }

        //private List<DeskBandMenuItem> ContextMenuItems
        //{
        //    get
        //    {
        //        var action = new DeskBandMenuAction("Action - Toggle submenu");
        //        var separator = new DeskBandMenuSeparator();
        //        var submenuAction = new DeskBandMenuAction("Submenu Action - Toggle checkmark");
        //        var submenu = new DeskBandMenu("Submenu")
        //        {
        //            Items = { submenuAction }
        //        };

        //        action.Clicked += (sender, args) => submenu.Enabled = !submenu.Enabled;
        //        submenuAction.Clicked += (sender, args) => submenuAction.Checked = !submenuAction.Checked;

        //        return new List<DeskBandMenuItem>() { action, separator, submenu };
        //    }
        //}

        private Guid _currentDesktopId;
        private const int _delay = 2000;

        public Guid CurrentDesktopId
        {
            get { return _currentDesktopId; }
            set
            {
                if (value == _currentDesktopId) return;
                _currentDesktopId = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl1()
        {
            InitializeComponent();
            GetTaskbarProperties();
        }

        private void GetTaskbarProperties()
        {
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(_delay / 100);
            }

            //Options.MinHorizontalSize.Width = 500;
            //Options.MinVerticalSize.Width = 130;
            //Options.MinVerticalSize.Height = 200;

            Options.MinVerticalSize.Height = TaskbarHeight;

            TaskbarEdge = TaskbarInfo.Edge;
            TaskbarOrientation = TaskbarInfo.Orientation == CSDeskBand.TaskbarOrientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
            WrapPanelOrientation = TaskbarInfo.Orientation == CSDeskBand.TaskbarOrientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            TaskbarWidth = TaskbarInfo.Size.Width;
            TaskbarHeight = TaskbarInfo.Size.Height;

            TaskbarInfo.TaskbarOrientationChanged += TaskbarInfo_TaskbarOrientationChanged;
            TaskbarInfo.TaskbarSizeChanged += TaskbarInfo_TaskbarSizeChanged;
            TaskbarInfo.TaskbarEdgeChanged += TaskbarInfo_TaskbarEdgeChanged;

            //Options.ContextMenuItems = ContextMenuItems;
        }

        private void TaskbarInfo_TaskbarOrientationChanged(object sender, TaskbarOrientationChangedEventArgs e)
        {
            TaskbarOrientation = e.Orientation == CSDeskBand.TaskbarOrientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
            WrapPanelOrientation = e.Orientation == CSDeskBand.TaskbarOrientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
        }

        private void TaskbarInfo_TaskbarSizeChanged(object sender, TaskbarSizeChangedEventArgs e)
        {
            TaskbarWidth = e.Size.Width;
            TaskbarHeight = e.Size.Height;
        }

        private void TaskbarInfo_TaskbarEdgeChanged(object sender, TaskbarEdgeChangedEventArgs e)
        {
            TaskbarEdge = e.Edge;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl1_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                VirtualDesktop.CurrentChanged += VirtualDesktop_CurrentChanged;
                VirtualDesktop.Created += VirtualDesktop_Created;
                VirtualDesktop.Destroyed += VirtualDesktop_Destroyed;

                GenerateButtons();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void GenerateButtons()
        {
            try
            {
                System.Threading.Thread.Sleep(_delay);
                pnlButtonContainer.Children.Clear();
                var desktops = VirtualDesktop.GetDesktops();
                if (desktops == null)
                {
                    System.Windows.Forms.MessageBox.Show("Can't get Virtual Desktops");
                    return;
                }
                int i = 1;
                foreach (var desktop in desktops)
                {
                    var button = new Button()
                    {
                        Content = i.ToString(),
                        Height = TaskbarHeight / rowCount,
                        Width = TaskbarHeight / rowCount
                    };
                    button.Tag = desktop.Id;
                    SetBindingByDesktopId(button, Button.BackgroundProperty, SystemColors.ActiveCaptionColor, Colors.Black);
                    SetBindingByDesktopId(button, Button.ForegroundProperty, SystemColors.ActiveCaptionTextColor, Colors.White);
                    button.Click += Button_Click;
                    pnlButtonContainer.Children.Add(button);
                    Options.HorizontalSize.Width += (int)button.Width;
                    Options.MaxHorizontalHeight += (int)button.Width;
                    i++;
                }
                //Options.HorizontalSize.Width = TaskbarHeight / rowCount * i;
                //Options.MaxVerticalWidth =
                OnPropertyChanged(nameof(CurrentDesktopId));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void SetBindingByDesktopId(Button button, DependencyProperty dependencyProperty, Color activeColor, Color inActiveColor)
        {
            SolidColorBrush _activeBrush = new SolidColorBrush(activeColor);
            SolidColorBrush _inActiveBrush = new SolidColorBrush(inActiveColor);

            Binding binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(CurrentDesktopId)),
                ConverterParameter = button.Tag,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new BrushByDesktopIdConverter(_activeBrush, _inActiveBrush)
            };
            BindingOperations.SetBinding(button, dependencyProperty, binding);

        }

        private void VirtualDesktop_Destroyed(object sender, VirtualDesktopDestroyEventArgs e)
        {
            Dispatcher.Invoke(GenerateButtons);
        }

        private void VirtualDesktop_Created(object sender, VirtualDesktop e)
        {
            Dispatcher.Invoke(GenerateButtons);
        }

        private void VirtualDesktop_CurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            CurrentDesktopId = e.NewDesktop.Id;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentButton = (Button)e.Source;
            var desktopId = (Guid)currentButton.Tag;
            var desktop = VirtualDesktop.FromId(desktopId);
            desktop.Switch();
        }
    }
}
