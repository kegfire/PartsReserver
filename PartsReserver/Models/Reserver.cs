using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PartsReserver.Models
{
	/// <summary>
	/// Класс фильтра по резервированию товара.
	/// </summary>
	[Serializable]
	public class Reserver
	{
		/// <summary>
		/// Наименование задачи.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Признак активности фильтра.
		/// </summary>
		public bool Activity { get; set; }

		public string CarMark { get; set; }

		public string CarModel { get; set; }

		/// <summary>
		/// Приведение объекта к строковому виду для http-запроса.
		/// </summary>
		/// <returns> Http строка.</returns>
		public override string ToString()
		{
			//var win1251 = Encoding.GetEncoding("Windows-1251");
			var properties = from p in GetType().GetProperties()
				where p.GetValue(this, null) != null
				select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(this, null).ToString(), Encoding.UTF8);

			string queryString = string.Join("&", properties.ToArray());
			return queryString;
		}

		/// <summary>
		/// Получить строку для запроса машин.
		/// </summary>
		/// <returns> Строка для запроса машин.</returns>
		public List<KeyValuePair<string, string>> AutoRequest()
		{
			var submitFilter = "%D0%9F%D1%80%D0%B8%D0%BC%D0%B5%D0%BD%D0%B8%D1%82%D1%8C";
			var result = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("18_9", $"{CarMark}"),
				new KeyValuePair<string, string>("stocktype", "3"),
				new KeyValuePair<string, string>("submit_filter", $"{submitFilter}"),
				new KeyValuePair<string, string>("start", "0"),
				new KeyValuePair<string, string>("limit", "100")
			};
			return result;
		}

	}
}