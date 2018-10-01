using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

		private Reserver _selectedReserver;

		private bool _serviceIsRunning;

		private Scheduler _scheduler;
		public MainPageViewModel()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			
			Reservers = new BindingList<Reserver>(new[] { new Reserver{  Name = "first"}, new Reserver() {  Name = "second"} });
			StartCommand = new RelayCommand(StartCommandExecute, CanStartCommandExecute);
			StopCommand = new RelayCommand(StopCommandExecute, CanStopCommandExecute);
			OpenSettingsCommand = new RelayCommand(OpenSettingsCommandExecute);
			SaveReserversCommand = new RelayCommand(SaveReserversExecute);
			LoadReserversCommand = new RelayCommand<string>(LoadReserversExecute);
			AddReserverCommand = new RelayCommand(AddReserverCommandExecute);
			RemoveReserverCommand = new RelayCommand(RemoveReserverCommandExecute);
			TestFilterCommand = new RelayCommand(TestFilterCommandExecute);
			LoadReserversExecute(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservers.xml"));
			if (Reservers.Any())
			{
				SelectedReserver = Reservers[0];
			}
		}

		public BindingList<Reserver> Reservers { get; set; }

		public Reserver SelectedReserver
		{
			get => _selectedReserver;
			set
			{
				_selectedReserver = value;
				OnPropertyChanged(nameof(EnableFilter));
			}
		}

		public bool EnableFilter => SelectedReserver != null;

		#region FilterFields

		public bool Activity
		{
			get => SelectedReserver?.Activity ?? false;
			set => SelectedReserver.Activity = value;
		}

		public string Name
		{
			get => SelectedReserver?.Name ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.Name = value;
				}
			}
		}

		public string CarMark
		{
			get => SelectedReserver?.CarMark ?? string.Empty;

			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.CarMark = value;
				}
			}
		}

		public string VinNumberOfCarMaker
		{
			get => SelectedReserver?.VinNumberOfCarMaker ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.VinNumberOfCarMaker = value;
				}
			}
		}

		public int ModelYear
		{
			get => SelectedReserver?.ModelYear ?? 0;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.ModelYear = value;
				}
			}
		}

		public Dictionary<string, string> Status => SelectedReserver?.Status;

		public string SelectedStatus
		{
			get => SelectedReserver?.SelectedStatus ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedStatus = value;
				}
			}
		}

		public Dictionary<string, string> OrderType => SelectedReserver?.OrderType;

		public string SelectedOrderType
		{
			get => SelectedReserver?.SelectedOrderType ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedOrderType = value;
				}
			}
		}

		public string ModelCode
		{
			get => SelectedReserver?.ModelCode ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.ModelCode = value;
				}
			}
		}

		public Dictionary<string, string> Stock => SelectedReserver?.Stock;

		public string SelectedStock
		{
			get => SelectedReserver?.SelectedStock ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedStock = value;
				}
			}
		}

		public Dictionary<string, string> AssemblyType => SelectedReserver?.AssemblyType;

		public string SelectedAssemblyType
		{
			get => SelectedReserver?.SelectedAssemblyType ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedAssemblyType = value;
				}
			}
		}

		public Dictionary<string, string> FundingStatus => SelectedReserver?.FundingStatus;

		public string SelectedFundingStatus
		{
			get => SelectedReserver?.SelectedFundingStatus ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedFundingStatus = value;
				}
			}
		}

		public Dictionary<string, string> DealerReserve => SelectedReserver?.DealerReserve;

		public string SelectedDealerReserve
		{
			get => SelectedReserver?.SelectedDealerReserve ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedDealerReserve = value;
				}
			}
		}

		public Dictionary<string, string> Funded => SelectedReserver?.Funded;

		public string SelectedFunded
		{
			get => SelectedReserver?.SelectedFunded ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedFunded = value;
				}
			}
		}

		public Dictionary<string, string> VbrError => SelectedReserver?.VbrError;

		public string SelectedVbrError
		{
			get => SelectedReserver?.SelectedVbrError ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedVbrError = value;
				}
			}
		}

		public Dictionary<string, string> AdditionalFeature => SelectedReserver?.AdditionalFeature;

		public string SelectedAdditionalFeature
		{
			get => SelectedReserver?.SelectedAdditionalFeature ?? string.Empty;
			set
			{
				if (SelectedReserver != null)
				{
					SelectedReserver.SelectedAdditionalFeature = value;
				}
			}
		}

		#endregion

		#region Commands
		public RelayCommand SaveReserversCommand { get; set; }

		public RelayCommand<string> LoadReserversCommand { get; set; }

		public RelayCommand OpenSettingsCommand { get; set; }

		public RelayCommand StartCommand { get; set; }

		public RelayCommand StopCommand { get; set; }

		public RelayCommand AddReserverCommand { get; set; }

		public RelayCommand RemoveReserverCommand { get; set; }

		public RelayCommand TestFilterCommand { get; set; }

		private void SaveReserversExecute()
		{
			Logger.Debug("Сохранение задач");
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservers.xml");
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

							Reservers = new BindingList<Reserver>(reservers);
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

							Reservers = new BindingList<Reserver>(reservers);
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

		private void AddReserverCommandExecute()
		{
			Reservers.Add(new Reserver() { Name = "new reserver" });
			SelectedReserver = Reservers.Last();
		}

		private void RemoveReserverCommandExecute()
		{
			Reservers.Remove(SelectedReserver);
		}

		private void TestFilterCommandExecute()
		{
			try
			{
				var settings = new Settings();
				settings.Load();
				using (var httpClient = new HttpClientWrapper(settings.ServerAddress))
				{
#if DEBUG
					var result = httpClient.GetCarListTest(SelectedReserver, _cancellationTokenSource.Token);
					var dt = Helper.ToDataTable(result);
					var autoRequestWindow = new AutoRequestResultWindow(dt);
					autoRequestWindow.ShowDialog();

#else
					if (httpClient.Logon(settings.Login, settings.Password, _cancellationTokenSource.Token).Result)
					{
						var result = httpClient.GetCarListAsync(SelectedReserver, _cancellationTokenSource.Token).Result;
						var dt = Helper.ToDataTable(result);
						var autoRequestWindow = new AutoRequestResultWindow(dt);
						autoRequestWindow.ShowDialog();
					}
#endif
				}
			}
			catch (Exception e)
			{
				Logger.Write(e.Message);
			}
		}

#endregion
	}
}