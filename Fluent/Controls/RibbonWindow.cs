﻿#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Metro.Native;
#if NET40
    using Microsoft.Windows.Shell;
#else
    using System.Windows.Shell;
#endif
    using Microsoft.Win32;
    using System.Globalization;

    /// <summary>
    /// Represents basic window for ribbon
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class RibbonWindow: Window
    {
        private const string PART_Icon = "PART_Icon";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_WindowCommands = "PART_WindowCommands";

        private FrameworkElement iconImage;
        internal bool isSystemMenuOpened = false;

        #region Properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for WindowCommands.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowCommandsProperty = DependencyProperty.Register("WindowCommands", typeof(WindowCommands), typeof(RibbonWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the window commands
        /// </summary>
        public WindowCommands WindowCommands
        {
            get { return (WindowCommands)GetValue(WindowCommandsProperty); }
            set { SetValue(WindowCommandsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SaveWindowPosition.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SavePositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false));

        /// <summary>
        ///  Gets or sets whether window position will be saved and loaded.
        /// </summary>
        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SavePositionProperty); }
            set { SetValue(SavePositionProperty, value); }
        }


        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public bool UseDefaultBorderSize
        {
            get { return (bool)this.GetValue(UseDefaultBorderSizeProperty); }
            set { this.SetValue(UseDefaultBorderSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BorderWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UseDefaultBorderSizeProperty =
            DependencyProperty.Register("UseDefaultBorderSize", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(false, OnWindowChromeRelevantPropertyChanged));


        /// <summary>
        /// Gets or sets resize border thickness
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderTickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(8), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public Thickness WindowMargin
        {
            get { return (Thickness)this.GetValue(WindowMarginProperty); }
            set { this.SetValue(WindowMarginProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WindowMarginProperty =
            DependencyProperty.Register("WindowMargin", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(0), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public double BorderWidth
        {
            get { return (double)this.GetValue(BorderWidthProperty); }
            set { this.SetValue(BorderWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BorderWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(0.0, OnWindowChromeRelevantPropertyChanged));


        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public double TitleBarHeight
        {
            get { return (double)this.GetValue(TitleBarHeightProperty); }
            set { this.SetValue(TitleBarHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TitleBarHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.Register("TitleBarHeight", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(0.0, OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public Thickness GlassBorderThickness
        {
            get { return (Thickness)GetValue(GlassBorderThicknessProperty); }
            set { SetValue(GlassBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GlassBorderThicknessProperty =
            DependencyProperty.Register("GlassBorderThickness", typeof(Thickness), typeof(RibbonWindow), new UIPropertyMetadata(new Thickness(0), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Gets or sets glass border thickness
        /// </summary>
        public double ExtendedGlassBorderHeight
        {
            get { return (double)this.GetValue(ExtendedGlassBorderHeightProperty); }
            set { this.SetValue(ExtendedGlassBorderHeightProperty, value); }
        }
        public static readonly DependencyProperty ExtendedGlassBorderHeightProperty =
            DependencyProperty.Register("ExtendedGlassBorderHeight", typeof(double), typeof(RibbonWindow), new UIPropertyMetadata(0.0, OnWindowChromeRelevantPropertyChanged));


        /// <summary>
        /// Gets or sets corner radius 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonWindow), new UIPropertyMetadata(new CornerRadius(15), OnWindowChromeRelevantPropertyChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DontUseDwm.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DontUseDwmProperty =
            DependencyProperty.Register("DontUseDwm", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false, OnDontUseDwmChanged));

        /// <summary>
        ///  Gets or sets whether DWM should be used.
        /// </summary>
        public bool DontUseDwm
        {
            get { return (bool)GetValue(DontUseDwmProperty); }
            set { SetValue(DontUseDwmProperty, value); }
        }

        /// <summary>
        /// Gets wheter DWM can be used (<see cref="NativeMethods.IsDwmEnabled"/> is true and <see cref="DontUseDwm"/> is false).
        /// </summary>
        public bool CanUseDwm
        {
            get { return (bool)GetValue(CanUseDwmProperty); }
            private set { SetValue(CanUseDwmPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey CanUseDwmPropertyKey = DependencyProperty.RegisterReadOnly("CanUseDwm", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanUseDwm.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanUseDwmProperty = CanUseDwmPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public bool IsIconVisible
        {
            get { return (bool)GetValue(IsIconVisibleProperty); }
            set { SetValue(IsIconVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether icon is visible
        /// </summary>
        public static readonly DependencyProperty IsIconVisibleProperty = DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(true));

        // todo check if IsCollapsed and IsAutomaticCollapseEnabled should be reduced to one shared property for RibbonWindow and Ribbon
        /// <summary>
        /// Gets whether window is collapsed
        /// </summary>              
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool),
            typeof(RibbonWindow), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines if the Ribbon should automatically set <see cref="IsCollapsed"/> when the width or height of the owner window drop under <see cref="Ribbon.MinimalVisibleWidth"/> or <see cref="Ribbon.MinimalVisibleHeight"/>
        /// </summary>
        public bool IsAutomaticCollapseEnabled
        {
            get { return (bool)GetValue(IsAutomaticCollapseEnabledProperty); }
            set { SetValue(IsAutomaticCollapseEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty =
            DependencyProperty.Register("IsAutomaticCollapseEnabled", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(true));

        public Transform DpiScaleTransform
        {
            get { return (Transform)this.GetValue(DpiScaleTransformProperty); }
            set { this.SetValue(DpiScaleTransformProperty, value); }
        }

        public static readonly DependencyProperty DpiScaleTransformProperty =
            DependencyProperty.Register("DpiScaleTransform", typeof(Transform), typeof(RibbonWindow), new UIPropertyMetadata(Transform.Identity));


        private readonly WindowSizing windowSizing;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonWindow()
        {
            StyleProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(null, OnCoerceStyle));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));

            RibbonProperties.TitleBarHeightProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(OnWindowChromeRelevantPropertyChanged));
        }

        // Coerce object style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue != null)
            {
                return basevalue;
            }

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                basevalue = frameworkElement.TryFindResource(typeof(RibbonWindow));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonWindow()
        {
            this.SizeChanged += this.OnSizeChanged;
            this.StateChanged += (s, e) => this.UpdateWindowChrome();

            this.windowSizing = new WindowSizing(this);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.UpdateCanUseDwm();

            this.windowSizing.WindowInitialized();

            this.UpdateWindowChrome();
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">A reference to the root of the old content tree.</param><param name="newContent">A reference to the root of the new content tree.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            var content = newContent as IInputElement;

            if (content != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(content, true);
            }
        }

        #endregion

        private static void OnWindowChromeRelevantPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as RibbonWindow;
            if (window == null)
            {
                return;
            }

            if (!window.UseDefaultBorderSize)
            {
                window.UpdateWindowChrome();
            }
        }

        private static void OnDontUseDwmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as RibbonWindow;

            if (window == null)
            {
                return;
            }

            window.UpdateCanUseDwm();
        }

        // Size change to collapse ribbon
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MaintainIsCollapsed();
        }

        private void MaintainIsCollapsed()
        {
            if (this.IsAutomaticCollapseEnabled == false)
            {
                return;
            }

            if (this.ActualWidth < Ribbon.MinimalVisibleWidth
                || this.ActualHeight < Ribbon.MinimalVisibleHeight)
            {
                this.IsCollapsed = true;
            }
            else
            {
                this.IsCollapsed = false;
            }
        }

        internal void UpdateWindowChrome()
        {
            var windowChrome = WindowChrome.GetWindowChrome(this);
            if (windowChrome == null)
            {
                windowChrome = new WindowChrome();
                WindowChrome.SetWindowChrome(this, windowChrome);
            }

            var metrics = this.windowSizing.GetBorderWidthAndCaptionHeight();
            var borderWidth = metrics.Item1;
            if (this.UseDefaultBorderSize)
            {
                var captionHeight = metrics.Item2;

                this.BorderWidth = borderWidth;
                this.TitleBarHeight = Math.Ceiling(this.WindowState == WindowState.Maximized ? captionHeight : borderWidth + captionHeight);
                this.GlassBorderThickness = new Thickness(borderWidth, Math.Ceiling(borderWidth + captionHeight + this.ExtendedGlassBorderHeight), borderWidth, borderWidth);
                this.WindowMargin = this.WindowState == WindowState.Maximized
                    ? new Thickness(borderWidth, Math.Ceiling(borderWidth), borderWidth, borderWidth)
                    : new Thickness(borderWidth, 0, borderWidth, borderWidth);

                windowChrome.GlassFrameThickness = this.GlassBorderThickness;
                windowChrome.ResizeBorderThickness = new Thickness(borderWidth);
            }
            else
            {
                this.WindowMargin = this.WindowState == WindowState.Maximized
                    ? new Thickness(borderWidth, Math.Ceiling(borderWidth), borderWidth, borderWidth)
                    : new Thickness(0);

                windowChrome.CaptionHeight = RibbonProperties.GetTitleBarHeight(this);
                windowChrome.CornerRadius = this.CornerRadius;
                windowChrome.GlassFrameThickness = this.GlassBorderThickness;
                windowChrome.ResizeBorderThickness = this.ResizeBorderThickness;
            }
#if NET45
            windowChrome.UseAeroCaptionButtons = this.CanUseDwm;
#endif
        }

        private void UpdateCanUseDwm()
        {
            this.CanUseDwm = NativeMethods.IsDwmEnabled()
                          && this.DontUseDwm == false;
        }

        #region Metro

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.UpdateCanUseDwm();

            this.UpdateWindowChrome();

            if (this.iconImage != null)
            {
                this.iconImage.MouseDown -= this.HandleIconMouseDown;
            }

            if (this.WindowCommands == null)
            {
                this.WindowCommands = new WindowCommands();
            }

            this.iconImage = this.GetTemplateChild(PART_Icon) as FrameworkElement;

            if (this.iconImage != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(this.iconImage, true);

                this.iconImage.MouseDown += this.HandleIconMouseDown;
            }

            var partContentPresenter = this.GetTemplateChild(PART_ContentPresenter) as UIElement;

            if (partContentPresenter != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(partContentPresenter, true);
            }

            var partWindowCommands = this.GetTemplateChild(PART_WindowCommands) as UIElement;

            if (partWindowCommands != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(partWindowCommands, true);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.StateChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowCommands != null)
            {
                this.WindowCommands.RefreshMaximizeIconState();
            }

            base.OnStateChanged(e);
        }

        private void HandleIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 1)
                {
                    if (!isSystemMenuOpened)
                    {
                        e.Handled = true;

                        var physicalPopupPosition = this.iconImage.PointToScreen(new Point(0, this.iconImage.ActualHeight));
                        var virtualPopupPosition = this.windowSizing.PhysicalToVirtualBySystemDpi(physicalPopupPosition);
                        SystemCommands.ShowSystemMenu(this, virtualPopupPosition);

                        isSystemMenuOpened = true;
                    }
                    else
                    {
                        e.Handled = true;

                        isSystemMenuOpened = false;
                    }
                }
                else if (e.ClickCount == 2)
                {
                    e.Handled = true;

                    this.Close();
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                e.Handled = true;

                this.RunInDispatcherAsync(() =>
                {
                    var mousePosition = e.GetPosition(this);
                    var physicalPopupPosition = this.PointToScreen(mousePosition);
                    var virtualPopupPosition = this.windowSizing.PhysicalToVirtualBySystemDpi(physicalPopupPosition);
                    SystemCommands.ShowSystemMenu(this, virtualPopupPosition);
                });
            }
        }

        #endregion
    }
}