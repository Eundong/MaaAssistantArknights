// <copyright file="WindowManager.cs" company="MaaAssistantArknights">
// MaaWpfGui - A part of the MaaCoreArknights project
// Copyright (C) 2021 MistEO and Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using MaaWpfGui.Constants;
using Stylet;
using Screen = System.Windows.Forms.Screen;

namespace MaaWpfGui.Helper
{
    public class WindowManager : Stylet.WindowManager
    {
        public WindowManager(IViewManager viewManager, Func<IMessageBoxViewModel> messageBoxViewModelFactory, IWindowManagerConfig config)
    : base(viewManager, messageBoxViewModelFactory, config)
        {
        }

        private readonly string ScreenName = ConfigurationHelper.GetValue(ConfigurationKeys.MonitorNumber, string.Empty);
        private readonly int ScreenWidth = int.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.MonitorWidth, "-1"));
        private readonly int ScreenHeight = int.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.MonitorHeight, "-1"));

        private static readonly double DefaultDouble = -114514;
        private readonly double Left = double.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.PositionLeft, DefaultDouble.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
        private readonly double Top = double.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.PositionTop, DefaultDouble.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
        private readonly double Width = double.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.WindowWidth, DefaultDouble.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
        private readonly double Height = double.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.WindowHeight, DefaultDouble.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);

        private readonly string RootView = "MaaWpfGui.RootView";

        public void MoveWindowToDisplay(string displayName, Window window)
        {
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == displayName);
            if (screen != null)
            {
                var screenRect = screen.Bounds;
                if (screenRect.Height == ScreenHeight && screenRect.Width == ScreenWidth)
                {
                    window.Left = (int)(screenRect.Left + Left);
                    window.Top = (int)(screenRect.Top + Top);
                    window.Width = Width;
                    window.Height = Height;
                    return;
                }
            }

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <inheritdoc/>
        protected override Window CreateWindow(object viewModel, bool isDialog, IViewAware ownerViewModel)
        {
            Window window = base.CreateWindow(viewModel, isDialog, ownerViewModel);
            if (bool.Parse(ConfigurationHelper.GetValue(ConfigurationKeys.LoadPositionAndSize, bool.TrueString)))
            {
                if (isDialog || ownerViewModel != null || Math.Abs(Left - DefaultDouble) < 0.01f || Math.Abs(Top - DefaultDouble) < 0.01f)
                {
                    return window;
                }

                // In Stylet, CreateWindow().WindowStartupLocation is CenterScreen or CenterOwner (if w.WSLoc == Manual && w.Left == NaN && w.Top == NaN && ...)
                window.WindowStartupLocation = WindowStartupLocation.Manual;

                if (window.ToString() == RootView)
                {
                    MoveWindowToDisplay(ScreenName, window);
                }
                else
                {
                    // Center other windows in MaaWpfGui.RootView
                    var mainWindow = Application.Current.MainWindow;
                    window.Left = mainWindow!.Left + ((mainWindow.Width - window.Width) / 2);
                    window.Top = mainWindow.Top + ((mainWindow.Height - window.Height) / 2);
                }
            }

            var app = Application.Current as App;
            app!.darkToStart();

            return window;
        }
    }
}
