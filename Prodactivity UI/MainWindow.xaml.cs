﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Prodactivity_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer ticker = new DispatcherTimer();
        bool ticker_status = false;
        TimeSpan Duration = new TimeSpan(0);

        bool isDarkMode = false;

        public MainWindow()
        {
            InitializeComponent();
            ticker.Tick += On_Tick;

            // TODO: need to create a fix that when window is out of focus stop animation!
            StateChanged += WindowStateChanged;
            IsVisibleChanged += WindowVisabilityChanged;

            Loaded += OnStartUp;
            ticker.Interval = TimeSpan.FromSeconds(1);
        }

        private void WindowVisabilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("[DEBUG]" + e.Property.Name + " changed value: " + e.NewValue);
        }

        private void OnStartUp(object sender, RoutedEventArgs e)
        {
            int tier = (RenderCapability.Tier >> 0);
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            Debug.Write("[DEBUG]System Graphic tier: " + (RenderCapability.Tier >> 16) + "\n");
            // RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            
        }

        private void WindowStateChanged(object sender, EventArgs? e)
        {
            Debug.WriteLine("[DEBUG]Window state has been changed");
            if (ticker_status)
            {
                if (WindowState == WindowState.Minimized)
                {
                    Spinner_storyBoard.Pause();
                    Debug.Write("[DEBUG]minimized spin\n");
                }
                else
                {
                    Spinner_storyBoard.Resume();
                    Show_Duration_In_UI();
                    Debug.Write("[DEBUG]Got Focus resume spining\n");
                }
            }
        }

        private void Exit_button_Pressed(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown(0);
        }

        private void DragBar_Held(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        void On_Tick(object sender, EventArgs e)
        {
            if (Duration.Ticks == 0)
            {
                ChangeTickerStatus();
            }
            else
            {
                Duration -= TimeSpan.FromSeconds(1); // taking of second
                Show_Duration_In_UI();
            }
        }

        void Show_Duration_In_UI()
        {
            if (WindowState != WindowState.Minimized)
                TimeLabel.Text = Duration.ToString("hh") + ":" + Duration.ToString("mm") + ":" + Duration.ToString("ss");
        }

        private void add_Minutes_Button_Pressed(object sender, MouseButtonEventArgs e)
        {
            Duration += TimeSpan.FromMinutes(10);
            Show_Duration_In_UI();
        }

        private void StartStop_Button_Pressed(object sender, MouseButtonEventArgs e)
        {
            if (Duration.Ticks > 0)
            {
                ChangeTickerStatus();
            }
        }

        private void ChangeTickerStatus()
        {
            if (ticker_status)
            {
                startStop_Button_label.Text = "Start";
                ticker.Stop();
                Spinner_storyBoard.Stop();
            }
            else
            {
                startStop_Button_label.Text = "Stop";
                Spinner_storyBoard.RepeatBehavior = RepeatBehavior.Forever;
                Spinner_storyBoard.Begin();
                ticker.Start();
            }
            ticker_status = !ticker_status;
        }

        private void Switch_Dark_Light_Mode()
        {
            // Simple Version
            // TODO: Animation Color Changes
            this.Resources["FrontGround_Color"] = ((SolidColorBrush)(this.Resources["FrontGround_Color_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            this.Resources["Highlight_Color_Main"] = ((SolidColorBrush)(this.Resources["Highlight_Color_Main_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            this.Resources["Highlight_Color_Secondary"] = ((SolidColorBrush)(this.Resources["Highlight_Color_Secondary_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            this.Resources["Text_Color"] = ((SolidColorBrush)(this.Resources["Text_Color_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            this.Resources["Background_gradient"] = ((LinearGradientBrush)(this.Resources["Background_gradient_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            this.Resources["Spinner_Color"] = ((RadialGradientBrush)(this.Resources["Spinner_Color_" + (isDarkMode ? "Light" : "Dark")])).CloneCurrentValue();
            
            // storyboard Properties
            Storyboard sb = DarkMode_Switcher_storyBoard;

            DarkMode_Switcher_storyBoard_margin.To = new Thickness((isDarkMode ? 2.5 : 27.5), 0, 0, 0);

            DarkMode_Switcher_storyBoard_tumb.To = (isDarkMode ?
                ((SolidColorBrush)this.Resources["Text_Color_Dark"]).Color :
                ((SolidColorBrush)this.Resources["Text_Color_Light"]).Color);

            DarkMode_Switcher_storyBoard_background.To = (isDarkMode ? 
                ((SolidColorBrush)this.Resources["Highlight_Color_Main_Light"]).Color :
                ((SolidColorBrush)this.Resources["Highlight_Color_Main_Dark"]).Color);
            sb.Begin();

            isDarkMode = !isDarkMode;

        }

        private void Dark_Mode_Switcher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Switch_Dark_Light_Mode();
            
        }
    }
}
