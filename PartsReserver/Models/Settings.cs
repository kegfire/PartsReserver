using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;


namespace PartsReserver.Models
{

	[Serializable]
	public class Settings
	{
		public string ServerAddress { get; set; }

		public string Login { get; set; }

		public string Password { get; set; }
		
		/// <summary>
		/// Частота запуска задачи в секундах.
		/// </summary>
		public int Period { get; set; }

		/// <summary>
		/// Время начала работы задач.
		/// </summary>
		[XmlIgnore]
		public TimeSpan TimeToStart { get; set; }

		[Browsable(false)]
		[XmlElement(DataType = "duration", ElementName = "TimeToStart")]
		public string TimeTimeToStartString
		{
			get => XmlConvert.ToString(TimeToStart);
			set => TimeToStart = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
		}

		/// <summary>
		/// Загрузить настройки.
		/// </summary>
		public void Load()
		{
			Logger.Debug("Загрузка настроек");
			var o = new object();

			lock (o)
			{
				var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings.xml");
				if (File.Exists(path))
				{
					using (var stream = new FileStream(path, FileMode.Open))
					{
						var ns = new XmlSerializerNamespaces();
						ns.Add(string.Empty, string.Empty);
						var serializer = new XmlSerializer(typeof(Settings));

						if (!(serializer.Deserialize(stream) is Settings settings))
						{
							throw new Exception();
						}

						ServerAddress = settings.ServerAddress;
						Login = settings.Login;
						Password = settings.Password;
						Period = settings.Period;
						TimeToStart = settings.TimeToStart;
					}
				}
				else
				{
					ServerAddress = "http://aurora";
					Period = 10;
					TimeToStart = TimeSpan.Zero;
					UpdateSettings();
				}
			}
		}

		/// <summary>
		/// Сохранить настройки в файл.
		/// </summary>
		public void UpdateSettings()
		{
			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings.xml");
			try
			{
				var serializer = new XmlSerializer(typeof(Settings));
				using (TextWriter writer = new StreamWriter(path))
				{
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);
					serializer.Serialize(writer, this, ns);
				}
			}
			catch (Exception ex)
			{
				Logger.Write("Ошибка создания settings.xml. ", ex);
			}
		
		}
	}
}