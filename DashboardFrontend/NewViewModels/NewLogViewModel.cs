using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DashboardFrontend.NewViewModels
{
    public class NewLogViewModel : BaseViewModel
    {
        public NewLogViewModel(Conversion conversion)
        {
            _conversion = conversion;
            if (conversion.ActiveExecution is null)
            {
                conversion.Executions.CollectionChanged += delegate
                {
                    SetupLog();
                };
            }
            else
            {
                SetupLog();
            }
        }
        private readonly Conversion _conversion;

        private Log? _log;
        public Log? Log 
        { 
            get => _log; 
            set
            {
                _log = value;
                OnPropertyChanged(nameof(Log));
                SetupView();
            } 
        }
        private CollectionView _messageView;
        public CollectionView MessageView
        {
            get => _messageView;
            set
            {
                _messageView = value;
                OnPropertyChanged(nameof(MessageView));
            }
        }

        private void SetupLog()
        {
            if (!_conversion.Executions.Any())
            {
                throw new ArgumentException("Expected a nonempty list of executions", nameof(_conversion.Executions));
            }
            Log = _conversion.Executions.Last().Log;
            Log.OnFilterChanged += delegate { MessageView?.Refresh(); };
            //Log.Messages.CollectionChanged += delegate { SetupView(); }
        }

        public void SetupView()
        {
            if (Log is null)
            {
                throw new ArgumentNullException(nameof(Log), "Cannot create default view when Log is null");
            }
            MessageView = (CollectionView)CollectionViewSource.GetDefaultView(Log.Messages);
            MessageView.Filter += OnMessagesFilter;
        }

        public bool OnMessagesFilter(object item)
        {
            LogMessage msg = (LogMessage)item;
            return (Log!.ShowInfo && msg.Type.HasFlag(LogMessageType.Info))
                || (Log.ShowErrors && msg.Type.HasFlag(LogMessageType.Error))
                || (Log.ShowFatal && msg.Type.HasFlag(LogMessageType.Fatal))
                || (Log.ShowWarn && msg.Type.HasFlag(LogMessageType.Warning))
                || (Log.ShowValidations && msg.Type.HasFlag(LogMessageType.Validation));
        }
    }
}
