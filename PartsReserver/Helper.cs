using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

		public static DataTable ToDataTable(List<Dictionary<string, string>> list)
		{
			DataTable result = new DataTable();
			if (list.Count == 0)
				return result;

			var columnNames = list.SelectMany(dict => dict.Keys).Distinct();
			result.Columns.AddRange(columnNames.Select(c => new DataColumn(c)).ToArray());
			foreach (var item in list)
			{
				var row = result.NewRow();
				foreach (var key in item.Keys)
				{
					row[key] = item[key];
				}

				result.Rows.Add(row);
			}

			return result;
		}
	}
}