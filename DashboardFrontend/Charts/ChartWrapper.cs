using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DashboardFrontend.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Model;

namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A class for the creating and controlling <see cref="ISeries"/>
    /// </summary>
    public abstract class ChartWrapper : BaseViewModel
    {
        public ChartWrapper(ChartTemplate chart, bool shouldAutoFocus)
        {
            Chart = chart;
            if (shouldAutoFocus)
            {
                AutoFocusOn();
            }
        }

        #region public

        public ChartTemplate Chart { get; set; }

        private double? _lastPrimaryReading = 0;
        public double? LastPrimaryReading
        {
            get => _lastPrimaryReading;
            set
            {
                _lastPrimaryReading = value;
                OnPropertyChanged(nameof(LastPrimaryReading));
            }
        }
        private double? _lastSecondaryReading = 0;
        public double? LastSecondaryReading
        {
            get => _lastSecondaryReading;
            set
            {
                _lastSecondaryReading = value;
                OnPropertyChanged(nameof(LastSecondaryReading));
            }
        }
        #endregion

        #region private
        private PeriodicTimer? _autoFocusTimer;
        private bool _isAutoFocusTimer = false;
        private int _maxView = 10;
        #endregion

        #region AutoFocus Functions
        /// <summary>
        /// Disables AutoFocusOn().
        /// </summary>
        public void AutoFocusOff()
        {
            _autoFocusTimer?.Dispose();
            _isAutoFocusTimer = false;
        }

        /// <summary>
        /// Aync function that focus the chart on the most resent entries, within a given time span.
        /// </summary>
        public async void AutoFocusOn()
        {
            if (!_isAutoFocusTimer)
            {
                _autoFocusTimer = new(TimeSpan.FromMilliseconds(100));
                _isAutoFocusTimer = true;

                while (await _autoFocusTimer.WaitForNextTickAsync())
                {
                    if (Chart.Values.Count > 0 && Chart.Values.First()?.Count > 0)
                    {
                        Chart.XAxis[0].MinLimit = Chart.Values.First().Count >= _maxView
                            ? Chart.Values.First().Last().X.Value - DateTime.FromBinary(TimeSpan.FromMinutes(_maxView).Ticks).ToOADate() 
                            : Chart.Values.First().First().X.Value;
                        Chart.XAxis[0].MaxLimit = Chart.Values.First().Last().X.Value;
                    }
                }
            }
        }
        #endregion

        #region Settings Functions
        /// <summary>
        /// Change max viewable time span on the graph while auto focusing.
        /// </summary>
        /// <param name="input"></param>
        public void ChangeMaxView(int input)
        {
            _maxView = input;
        }

        /// <summary>
        /// Sets the smoothness of curves on all <see cref="ISeries"/>.
        /// </summary>
        /// <param name = "input">A number between 0 and 1. Standart is 1.</param>
        public void ChangeLineSmoothness(double input)
        {
            if (input < 0 || input > 1) { return; }

            foreach (LineSeries<ObservablePoint> line in Chart.Series)
            {
                line.LineSmoothness = input;
            }
        }

        /// <summary>
        /// Sets the size of line points on all <see cref="ISeries"/>.
        /// </summary>
        /// <param name="input">A number between 0 and 60. less is smaller. Standart is 10.</param>
        public void ChangePointSize(int input)
        {
            if (input > 60) { return; }
            else if (input < 0) { return; }

            foreach (LineSeries<ObservablePoint> line in Chart.Series)
            {
                line.GeometrySize = input;
            }
        }
        #endregion
    }
}
