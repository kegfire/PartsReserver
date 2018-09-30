using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using PartsReserver.Annotations;

namespace PartsReserver.Models
{
	/// <summary>
	/// Класс фильтра по резервированию товара.
	/// </summary>
	[Serializable]
	public class Reserver : INotifyPropertyChanged
	{
		/// <summary>
		/// Наименование задачи.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Признак активности фильтра.
		/// </summary>
		public bool Activity { get; set; }

		/// <summary>
		/// 18_3. Коммерческий номер
		/// </summary>
		[Description("18_3")]
		public string CommercialNumber { get; set; }

		/// <summary>
		/// 18_4.VIN/VIN кузовопроизводителя
		/// </summary>
		[Description("18_4")]
		public string VinNumberOfCarMaker { get; set; }

		/// <summary>
		/// 18_5.Модельный год
		/// </summary>
		[Description("18_5")]
		public int ModelYear { get; set; }

		/// <summary>
		/// 18_6.Статус
		/// </summary>
		[XmlIgnore]
		public readonly Dictionary<string, string> Status = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{ "Ожидание", "0"},
			{ "Создан", "1"},
			{"Заказан", "2"},
			{"Подготовка к производству", "3"},
			{"Производится", "4"},
			{"Произведен", "5"},
			{"Транзит", "6"},
			{"Склад_импортера", "7"},
			{"Транзит-Россия", "8"},
			{"Склад_ТК", "9"},
			{"Транзит-дилер", "10"},
			{"Получен дилером", "11"},
			{"Продан дилеру", "12"},
			{"Продан клиенту", "13"},
			{"Удален", "14"},
			{"Уничтожен", "15"}
		};

		/// <summary>
		/// Выбранный статус.
		/// </summary>
		[Description("18_6")]
		public string SelectedStatus { get; set; }

		/// <summary>
		/// 18_7.Тип заказа.
		/// </summary>
		[XmlIgnore]
		public readonly Dictionary<string, string> OrderType = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Customer order", "0"},
			{"Special importer stock order", "1"},
			{"Dealer stock order", "2"},
			{"Voluntary importer stock", "3"}
		};

		/// <summary>
		/// Выбранный тип заказа.
		/// </summary>
		[Description("18_7")]
		public string SelectedOrderType { get; set; }

		/// <summary>
		/// 18_8.Код модели.
		/// </summary>
		[Description("18_8")]
		public string ModelCode { get; set; }

		/// <summary>
		/// 18_9.Наименование модели
		/// </summary>
		[Description("18_9")]
		public string CarMark { get; set; }

		///// <summary>
		///// 18_10.По дате отправки дилеру с"
		///// </summary>
		//[Description("18_10")]
		//public DateTime SendDateToDealerFrom { get; set; }

		///// <summary>
		///// 18_10.
		///// </summary>
		//[DisplayName("По дате отправки дилеру по")]
		//public DateTime SendDateToDealerTo { get; set; }

		/// <summary>
		/// 18_11.Склад ТК.
		/// </summary>
		[XmlIgnore]
		internal readonly Dictionary<string, string> Stock = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Склад_ТК", "9"},
			{"Транзит", "6"},
			{"Транзит-дилер", "10"},
			{"Транзит-Россия", "8"},
			{"BLG", "13"},
			{"Kaluga (KAZ orders)", "73"},
			{"MAJOR(Чисмена)", "66"},
			{"Quality Kaluga stop", "21"},
			{"Аббакумово (СВХ)", "37"},
			{"Автологистика (Пикино)", "6"},
			{"Автологистика (Таможенный)", "7"},
			{"Артан передельщик", "40"},
			{"АЦМ", "68"},
			{"АЦМ-ЮЗ", "69"},
			{"Бостон", "34"},
			{"Бронка (порт СПб)", "63"},
			{"Быково", "44"},
			{"Быково (броневики)", "27"},
			{"Владивосток, ДВ", "52"},
			{"Домодедово36", "36"},
			{"Домодедово70", "70"},
			{"Доскино (НН)", "42"},
			{"Дурыкино", "9"},
			{"Дурыкино (броневики)", "45"},
			{"ЗАО Исток передельщик", "59"},
			{"Истомино (НН)", "38"},
			{"Калуга", "17"},
			{"КРЫМ", "65"},
			{"Лобня (Рольф)", "35"},
			{"Луидор RUSN93508", "33"},
			{"ЛУИДОР передельщик", "39"},
			{"Михнево", "12"},
			{"Нижний Новгород", "25"},
			{"Ногинск", "11"},
			{"ООО СВХ &quot;МЕГА&quot;", "43"},
			{"ООО Центртранстехмаш Передельщик", "61"},
			{"Пикино", "10"},
			{"ПЛП (порт СПб)", "62"},
			{"Подольск", "5"},
			{"Полигон (Глонасс)", "58"},
			{"СВХ Щербинский", "19"},
			{"СВХ Элит-Транс", "20"},
			{"Склад Авто Ганза", "67"},
			{"Технический склад", "53"},
			{"Технополис Передельщик", "41"},
			{"Учебный центр ФГР", "47"},
			{"Янино (Спб)", "46"}
		};
		/// <summary>
		/// Выбранный склад.
		/// </summary>
		[Description("18_11")]
		public string SelectedStock { get; set; }

		/// <summary>
		/// 18_12. Тип сборки.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> AssemblyType = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"FBU", "0"},
			{"SKD", "1"},
			{"CKD", "2"}
		};

		/// <summary>
		/// Выбранный тип сборки.
		/// </summary>
		[Description("18_12")]
		public string SelectedAssemblyType { get; set; }

		/// <summary>
		/// 18_13. Статус финансирования.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> FundingStatus = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Оплата собственными средствами" , "256"},
			{"Переназначен" , "272"},
			{"Отменен" , "288"},
			{"Резерв" , "512"},
			{"Резерв изменен" , "528"},
			{"Подготовка документов" , "1024"},
			{"Документы подготовлены" , "1040"},
			{"Подписано дилером" , "1056"},
			{"Подписано VGR" , "1072"},
			{"Подписано банком" , "1088"},
			{"Профинансирован" , "2048"},
			{"Кредит погашен" , "2064"}
		};

		/// <summary>
		/// Выбранный статус финансирования.
		/// </summary>
		[Description("18_13")]
		public string SelectedFundingStatus { get; set; }

		/// <summary>
		/// 18_14. Резерв дилера.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> DealerReserve = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Отгрузка дилеру" , "D"},
			{"Резерв" , "Y"},
			{"Нет" , "N"}
		};

		/// <summary>
		/// Выбранный резерв дилера.
		/// </summary>
		[Description("18_14")]
		public string SelectedDealerReserve { get; set; }

		/// <summary>
		/// 18_15. Финансируется.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> Funded = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Да" , "Y"},
			{"Нет" , "N"}
		};

		/// <summary>
		/// Выбранный резерв дилера.
		/// </summary>
		[Description("18_15")]
		public string SelectedFunded { get; set; }

		/// <summary>
		/// 18_17. Ошибки VBR.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> VbrError = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"Получен неопределенный ответ от VBR (Broker OK) у дилера не хватает средств для отгрузки" , "-200"},
			{"Нет ответа от VWFS (Broker OK). Просьба направить VIN и код дилера на Corporate.Operations@vwfs.com" , "-100"},
			{"Ошибка получения ответа от VBR" , "-4"},
			{"Нет ответа от VBR" , "-3"},
			{"Очередь запросов VBR недоступна" , "-2"},
			{"Нет связи с VWFS" , "-1"},
			{"Missing/incorrect VIN(101001)" , "101001"},
			{"VIN already in reserve by another request" , "101002"},
			{"Missing/incorrect TAX/Dealer number(101003)" , "101003"},
			{"Brand is missing/incorrect(101004)" , "101004"},
			{"Model is missing/incorrect(101005)" , "101005"},
			{"Missing/incorrect Price (Out of agreed Range)(101008)" , "101008"},
			{"Not financing dealer" , "101009"},
			{"Car price currency is missing/incorrect(101011)" , "101011"},
			{"User identification is missing(101012)" , "101012"},
			{"Missing/incorrect VIN(102001)" , "102001"},
			{"Missing/incorrect TAX/Dealer number(102002)" , "102002"},
			{"Brand is missing/incorrect(102003)" , "102003"},
			{"Model is missing/incorrect(102004)" , "102004"},
			{"Missing/incorrect Price (Out of agreed Range)(102007)" , "102007"},
			{"Dealer and CAR not conform Initial Request (VIN, INN, Dealer number, Currency not correspond)" , "102009"},
			{"Limit Y days exceed(102011)" , "102011"},
			{"Car price currency is missing/incorrect(102012)" , "102012"},
			{"User identification is missing(102013)" , "102013"},
			{"Request &quot;Reserve&quot; was nor received and applied before this one" , "102016"},
			{"Request after documents preparation(102017)" , "102017"},
			{"Missing/incorrect VIN(103001)" , "103001"},
			{"Missing/incorrect TAX/Dealer number(103002)" , "103002"},
			{"Brand is missing/incorrect(103003)" , "103003"},
			{"Model is missing/incorrect(103004)" , "103004"},
			{"Year of manufacture is empty" , "103005"},
			{"Car colour is missing(103006)" , "103006"},
			{"Missing/incorrect Price (Out of agreed Range)(103007)" , "103007"},
			{"Dealer not conform Initial Request" , "103009"},
			{"Not all invoice details are filled" , "103010"},
			{"Not all PTS details are filled" , "103011"},
			{"Car price currency is missing/incorrect(103012)" , "103012"},
			{"User identification is missing(103013)" , "103013"},
			{"IFP End Date is not filled" , "103014"},
			{"IFP End Date less (current date + X days)" , "103015"},
			{"Factual shipment date is missing/incorrect" , "103016"},
			{"Expected payment date is missing/incorrect" , "103017"},
			{"Limit Y days exceed(103018)" , "103018"},
			{"Request &quot;Reserve&quot; or &quot;Reserve updated&quot; was nor received and applied before this one" , "103019"},
			{"Car location is empty" , "103020"},
			{"Request after documents preparation(103021)" , "103021"},
			{"Missing/incorrect numberOfEngine" , "103022"},
			{"Missing/incorrect colorDescription" , "103023"},
			{"Missing/incorrect modelDescription" , "103024"},
			{"Missing/incorrect modelYear" , "103025"},
			{"Missing/incorrect VIN(104001)" , "104001"},
			{"Missing/incorrect TAX/Dealer number(104002)" , "104002"},
			{"Missing Reason Type" , "104003"},
			{"Transhe in status Disbursed, Ready for Payment is system, Used for EDS." , "106003"},
			{"User identification is missing(106004)" , "106004"},
			{"Initial Request is not found" , "106005"},
			{"Limit is not enough" , "201001"},
			{"Limit reserved" , "201003"},
			{"Limit is not enough(202001)" , "202001"},
			{"Request data updated(202003)" , "202003"},
			{"Limit is not enough(203001)" , "203001"},
			{"Request data updated(203003)" , "203003"},
			{"Application is in processing" , "204003"},
			{"Critical errors in application" , "204004"},
			{"Request data updated (Cancelled)" , "204101"},
			{"Request data updated (Car paid without VBR)" , "204201"},
			{"Request data updated (Relocated)" , "204301"}
		};

		/// <summary>
		/// Выбранная ошибка VBR.
		/// </summary>
		[Description("18_17")]
		public string SelectedVbrError { get; set; }

		/// <summary>
		/// 18_18. Доп признаки.
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> AdditionalFeature = new Dictionary<string, string>
		{
			{string.Empty, string.Empty },
			{"BLOCK" , "20"},
			{"BUTOV" , "12"},
			{"BUTOVO" , "11"},
			{"ES reserve" , "19"},
			{"FSP" , "21"},
			{"KLUCH" , "32"},
			{"MAJOR" , "14"},
			{"NEW KEY" , "17"},
			{"NEWDAKAR" , "23"},
			{"NEWSIPKO" , "22"},
			{"Otk_June2017" , "18"},
			{"ROLF" , "13"},
			{"RUSVLOIS" , "34"},
			{"stock corr 20170529" , "16"},
			{"отгрузить дилеру после разблокировки" , "31"},
			{"Тест" , "33"},
			{"Шоурум" , "1"}
		};

		/// <summary>
		/// Выбранный доп признак.
		/// </summary>
		[Description("18_18")]
		public string SelectedAdditionalFeature { get; set; }

		/// <summary>
		/// Приведение объекта к строковому виду для http-запроса.
		/// </summary>
		/// <returns> Http строка.</returns>
		public override string ToString()
		{
			//var win1251 = Encoding.GetEncoding("Windows-1251");
			var properties = from p in GetType().GetProperties()
				where p.GetValue(this, null) != null
				select  p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(this, null).ToString(), Encoding.UTF8);

			string queryString = string.Join("&", properties.ToArray());
			return queryString;
		}

		/// <summary>
		/// Формирование списка фильтров для запроса.
		/// </summary>
		/// <param name="start"> Получить ответ с</param>
		/// <param name="limit"> Количество возвращаемых значений.</param>
		/// <returns> Список фильтров для запроса.</returns>
		public List<KeyValuePair<string, string>> ToListKeyValuePair(int start = 0, int limit = 100)
		{
			var submitFilter = "%D0%9F%D1%80%D0%B8%D0%BC%D0%B5%D0%BD%D0%B8%D1%82%D1%8C";
			var result = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("stocktype", "3"),
				new KeyValuePair<string, string>("submit_filter", $"{submitFilter}"),
				new KeyValuePair<string, string>("start", start.ToString()),
				new KeyValuePair<string, string>("limit", limit.ToString())
			};
			var properties = GetType().GetProperties();
			foreach (var property in properties.Where(x => x.GetValue(this, null) != null && x.CanWrite))
			{
				object[] attrs = property.GetCustomAttributes(true);
				foreach (var attr in attrs)
				{
					if (attr is DescriptionAttribute authAttr)
					{
						var auth = authAttr.Description;
						result.Add(new KeyValuePair<string, string>(auth, HttpUtility.UrlEncode(property.GetValue(this, null).ToString(), Encoding.UTF8)));
						break;
					}
				}
			}

			return result;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}