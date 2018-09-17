using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.CommandWpf;
using PartsReserver.Models;
using PartsReserver.Pages;

namespace PartsReserver.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		private CancellationTokenSource _cancellationTokenSource;

		private ObservableCollection<Reserver> _reserverList;

		private bool _serviceIsRunning;

		private Scheduler _scheduler;
		public MainPageViewModel()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			
			Reservers = new ObservableCollection<Reserver>(new[] { new Reserver() {  Name = "first" }, new Reserver() {  Name = "second" } });
			StartCommand = new RelayCommand(StartCommandExecute, CanStartCommandExecute);
			StopCommand = new RelayCommand(StopCommandExecute, CanStopCommandExecute);
			OpenSettingsCommand = new RelayCommand(OpenSettingsCommandExecute);
			SaveReserversCommand = new RelayCommand(SaveReserversExecute);
			LoadReserversCommand = new RelayCommand<string>(LoadReserversExecute);
			LoadReserversExecute(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reservers.xml"));
		}

		public ObservableCollection<Reserver> Reservers
		{
			get => _reserverList;
			set
			{
				_reserverList = value;
				OnPropertyChanged(nameof(Reservers));
			}
		}

		public RelayCommand SaveReserversCommand { get; set; }

		public RelayCommand<string> LoadReserversCommand { get; set; }

		public RelayCommand OpenSettingsCommand { get; set; }

		public RelayCommand StartCommand { get; set; }

		public RelayCommand StopCommand { get; set; }

		private void SaveReserversExecute()
		{
			Logger.Debug("Сохранение задач");
			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reservers.xml");
			try
			{
				var serializer = new XmlSerializer(typeof(List<Reserver>));
				using (TextWriter writer = new StreamWriter(path))
				{
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);
					serializer.Serialize(writer, Reservers.ToList(), ns);
				}
			}
			catch (Exception ex)
			{
				Logger.Write("Ошибка сохранения Reservers.xml. ", ex);
			}
		}

		private void LoadReserversExecute( string path = null)
		{
			Logger.Debug("Загрузка задач");
			if (!string.IsNullOrEmpty(path))
			{
				if (File.Exists(path))
				{
					try
					{
						using (var stream = new FileStream(path, FileMode.Open))
						{
							var ns = new XmlSerializerNamespaces();
							ns.Add(string.Empty, string.Empty);
							var serializer = new XmlSerializer(typeof(List<Reserver>));

							if (!(serializer.Deserialize(stream) is List<Reserver> reservers))
							{
								throw new Exception();
							}

							Reservers = new ObservableCollection<Reserver>(reservers);
						}
					}
					catch (Exception ex)
					{
						Logger.Write($"Ошибка загрузки файла {path} ", ex);
					}

				}
			}
			else
			{
				string fileName = null;
				try
				{
					using (OpenFileDialog fileDialog = new OpenFileDialog())
					{
						fileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
						fileDialog.Filter = "xml file (*.xml)|*.xml";
						fileDialog.RestoreDirectory = true;
						if (fileDialog.ShowDialog() == DialogResult.OK)
						{
							fileName = fileDialog.FileName;
						}
					}
					if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
					{
						using (var stream = new FileStream(fileName, FileMode.Open))
						{
							var ns = new XmlSerializerNamespaces();
							ns.Add(string.Empty, string.Empty);
							var serializer = new XmlSerializer(typeof(List<Reserver>));

							if (!(serializer.Deserialize(stream) is List<Reserver> reservers))
							{
								throw new Exception();
							}

							Reservers = new ObservableCollection<Reserver>(reservers);
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Write($"Ошибка загрузки файла {fileName} ", ex);
				}
			}
		
		}

		private void OpenSettingsCommandExecute()
		{
			var settingsWindow = new SettingsPage();
			settingsWindow.DataContext = new SettingsViewModel();
			settingsWindow.ShowDialog();
		}

		private bool ServiceIsRunning
		{
			get => _serviceIsRunning;
			set
			{
				_serviceIsRunning = value;
				OnPropertyChanged(nameof(ServiceIsRunning));
			}
		}
		private void StartCommandExecute()
		{
			_scheduler = new Scheduler(_cancellationTokenSource.Token);
			Task.Run(() => _scheduler.Start());
			ServiceIsRunning = true;
		}

		private bool CanStartCommandExecute() => !ServiceIsRunning;

		private void StopCommandExecute()
		{
			_cancellationTokenSource.Cancel();
			_scheduler.Stop();
			_cancellationTokenSource = new CancellationTokenSource();
			ServiceIsRunning = false;
		}

		private bool CanStopCommandExecute() => ServiceIsRunning;
	}
}