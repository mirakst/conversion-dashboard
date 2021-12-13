using System;
using System.Linq;
using System.Threading;
using DashboardFrontend.ViewModels;


namespace DashboardFrontend.Charts
{
    /// <summary>
    /// A wrapper class for charts. Contains the chart template <see cref="ChartTemplate"/>,
    /// and methods to help control the view of this.
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
        private int _maxView = 60;
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
        /// Async function that focus the chart on the most resent entries, within a given time span.
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
                        var firstPointDate = DateTime.FromOADate(Chart.Values.First().First().X.Value);
                        var lastPointDate = DateTime.FromOADate(Chart.Values.First().Last().X.Value);

                        Chart.XAxis[0].MinLimit = lastPointDate - firstPointDate >
                            TimeSpan.FromMinutes(_maxView) ?
                            (lastPointDate - TimeSpan.FromMinutes(_maxView)).ToOADate()  :
                             firstPointDate.ToOADate();

                        Chart.XAxis[0].MaxLimit = lastPointDate.ToOADate();
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
        #endregion
    }
}
