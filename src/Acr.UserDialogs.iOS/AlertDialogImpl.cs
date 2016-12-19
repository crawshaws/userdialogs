﻿using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;


namespace Acr.UserDialogs
{
    public class AlertDialogImpl : AbstractAlertDialog
    {
        readonly Func<UIViewController> viewControllerFunc;
        UIAlertController alert;


        public AlertDialogImpl(Func<UIViewController> viewControllerFunc)
        {
            this.viewControllerFunc = viewControllerFunc;
        }


        public override void Show()
        {
            var style = this.Actions.Any() ? UIAlertControllerStyle.ActionSheet : UIAlertControllerStyle.Alert;
            this.alert = UIAlertController.Create(this.Title, this.Message, style);

            foreach (var action in this.Actions.OfType<DialogAction>())
            {
                this.alert.AddAction(action.Create());
            }
            foreach (var txt in this.TextEntries.OfType<TextEntry>())
            {
                this.alert.AddTextField(x => txt.Hook(x));
            }
            this.AddNativeMainAction(this.alert, this.Positive);
            this.AddNativeMainAction(this.alert, this.Negative);
            this.AddNativeMainAction(this.alert, this.Neutral);
            //var vc = this.viewControllerFunc();
        }


        public override void Dismiss()
        {
            this.alert?.DismissViewController(true, null);
        }


        public Action Dismissed { get; set; }
        public IAlertDialog SetMainAction(DialogChoice choice, Action<IDialogAction> action)
        {
            var obj = new DialogAction { Choice = choice };
            action(obj);
            switch (choice)
            {
                case DialogChoice.Positive:
                    this.Positive = obj;
                    break;

                case DialogChoice.Negative:
                    this.Negative = obj;
                    break;

                case DialogChoice.Neutral:
                    this.Neutral = obj;
                    break;
            }
            return this;
        }


        void AddNativeMainAction(UIAlertController ctrl, IDialogAction action)
        {
            var impl = action as DialogAction;
            if (impl != null)
                ctrl.AddAction(impl.Create());
        }
        /*
            UIAlertController alert = null;
            var app = UIApplication.SharedApplication;
            app.InvokeOnMainThread(() =>
            {
                alert = alertFunc();
                var top = this.viewControllerFunc();
                if (alert.PreferredStyle == UIAlertControllerStyle.ActionSheet && UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    var x = top.View.Bounds.Width / 2;
                    var y = top.View.Bounds.Bottom;
                    var rect = new CGRect(x, y, 0, 0);

                    alert.PopoverPresentationController.SourceView = top.View;
                    alert.PopoverPresentationController.SourceRect = rect;
                    alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Unknown;
                }
                top.PresentViewController(alert, true, null);
            });
            return new DisposableAction(() =>
            {
                try
                {
                    app.InvokeOnMainThread(() => alert.DismissViewController(true, null));
                }
                catch { }
            });
         */
    }
}
