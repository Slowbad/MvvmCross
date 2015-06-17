using System;
using Android.Widget;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.Target;
using Cirrious.MvvmCross.Droid.Support.AppCompat.Widget;

namespace Cirrious.MvvmCross.Droid.Support.AppCompat.Target
{
    public class MvxAppCompatSpinnerSelectedItemBinding
        : MvxAndroidTargetBinding
    {
        protected MvxAppCompatSpinner Spinner
        {
            get { return (MvxAppCompatSpinner)Target; }
        }

        private object _currentValue;
        private bool _subscribed;

        public MvxAppCompatSpinnerSelectedItemBinding(MvxAppCompatSpinner spinner)
            : base(spinner) {}

        private void SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = this.Spinner;
            if (spinner == null)
                return;

            var newValue = spinner.Adapter.GetRawItem(e.Position);

            bool changed;
            if (newValue == null)
            {
                changed = (this._currentValue != null);
            }
            else
            {
                changed = !(newValue.Equals(this._currentValue));
            }

            if (!changed)
            {
                return;
            }

            this._currentValue = newValue;
            FireValueChanged(newValue);
        }

        protected override void SetValueImpl(object target, object value)
        {
            var spinner = (MvxAppCompatSpinner)target;

            if (value == null)
            {
                MvxBindingTrace.Warning("Null values not permitted in spinner SelectedItem binding currently");
                return;
            }

            if (!value.Equals(this._currentValue))
            {
                var index = spinner.Adapter.GetPosition(value);
                if (index < 0)
                {
                    MvxBindingTrace.Trace(MvxTraceLevel.Warning, "Value not found for spinner {0}", value.ToString());
                    return;
                }
                this._currentValue = value;
                spinner.SetSelection(index);
            }
        }

        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.TwoWay; }
        }

        public override void SubscribeToEvents()
        {
            var spinner = this.Spinner;
            if (spinner == null)
                return;

            spinner.ItemSelected += this.SpinnerItemSelected;
            this._subscribed = true;
        }

        public override Type TargetType
        {
            get { return typeof(object); }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                var spinner = this.Spinner;
                if (spinner != null && this._subscribed)
                {
                    spinner.ItemSelected -= this.SpinnerItemSelected;
                    this._subscribed = false;
                }
            }
            base.Dispose(isDisposing);
        }
    }
}