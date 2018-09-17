using System;
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

	}
}