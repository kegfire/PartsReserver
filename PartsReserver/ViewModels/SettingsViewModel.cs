using System;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using PartsReserver.Models;

namespace PartsReserver.ViewModels
{
	public class SettingsViewModel :BaseViewModel
	{
		private readonly Settings _settings;

		public SettingsViewModel()
		{
			_settings = new Settings();
			_settings.Load();
			SaveCommand = new RelayCommand<Window>(SaveExecute);
		}

		public string ServerAddress
		{
			get => _settings.ServerAddress;
			set => _settings.ServerAddress = value;
		}

		public string Login
		{
			get => _settings.Login;
			set => _settings.Login = value;
		}

		public string Password
		{
			get => _settings.Password;
			set => _settings.Password = value;
		}

		public int Period
		{
			get => _settings.Period;
			set => _settings.Period = value;
		}

		public TimeSpan TimeToStart
		{
			get => _settings.TimeToStart;
			set => _settings.TimeToStart = value;
		}

		public RelayCommand<Window> SaveCommand { get; set; }

		private void SaveExecute(Window window)
		{
			_settings.ServerAddress = ServerAddress;
			_settings.Login = Login;
			_settings.Password = Password;
			_settings.Period = Period;
			_settings.TimeToStart = TimeToStart;
			_settings.UpdateSettings();
			window?.Close();
		}
		
	}
}