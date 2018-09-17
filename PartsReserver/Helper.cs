using System;
using System.IO;
using System.Xml.Serialization;

namespace PartsReserver
{
	public static class Helper
	{
		public static T Deserialize<T>(string path) where T: new ()
		{
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
							var serializer = new XmlSerializer(typeof(T));
							var result = serializer.Deserialize(stream);
							if (!(result is T))
							{
								throw new Exception();
							}

							return (T) result;
						}
					}
					catch (Exception ex)
					{
						Logger.Write($"Ошибка загрузки файла {path} ", ex);
					}
				}
			}
			return new T();
		}
	}
}