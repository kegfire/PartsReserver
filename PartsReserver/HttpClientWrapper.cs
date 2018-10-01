using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using PartsReserver.Models;
using static PartsReserver.LicHelper;

namespace PartsReserver
{
	public class HttpClientWrapper : IDisposable
	{

		private readonly HttpClient _client;

		private readonly string _address;

		private readonly CookieContainer _cookies = new CookieContainer();

		private Stopwatch _licenseWatcher;

		public HttpClientWrapper(string address)
		{
			var handler = new HttpClientHandler
			{
				CookieContainer = _cookies,
				UseCookies = true,
				UseDefaultCredentials = false
			};
			_client = new HttpClient(new LoggingHandler(handler));
			_address = address;
		}
		
		/// <summary>
		/// Авторизоваться на сервисе.
		/// </summary>
		/// <param name="login">Логин пользователя.</param>
		/// <param name="password"> Пароль пользователя.</param>
		/// <param name="token"> Токен отмены.</param>
		/// <returns> Результат авторизации.</returns>
		public async Task <bool> Logon(string login, string password, CancellationToken token)
		{
			
			try
			{
				if (await GetCookies(token))
				{
					Logger.Debug("HttpClient. Login");
					var nvc = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("j_username", $"{login}"),
						new KeyValuePair<string, string>("j_password", $"{password}")
					};
					var url = _address + "/j_security_check";
					var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(nvc) };
					var response = await _client.SendAsync(req, token);
					Logger.Debug(await response.Content.ReadAsStringAsync());
					response.EnsureSuccessStatusCode();
					return _cookies.Count > 0;
				}
			}
			catch (Exception e)
			{
				Logger.Write("HttpClient. Login. ", e);
			}
			return false;
		}

		/// <summary>
		/// Получить список автомобилей по фильтру.
		/// </summary>
		/// <param name="filter"> Фильтр.</param>
		/// <param name="token"> Токен отмены.</param>
		/// <returns> Ответ от сервера</returns>
		public async Task<List<Dictionary<string, string>>> GetCarListAsync(Reserver filter, CancellationToken token)
		{
			Logger.Debug("HttpClient. /a/auto/auto.json");
			var url = _address + "/a/auto/auto.json";
			var start = 0;
			var list = new List<Dictionary<string, string>>(100);
			try
			{
				ValidateLicense();
				while (true)
				{
					var req = new HttpRequestMessage(HttpMethod.Post, url) {Content = new FormUrlEncodedContent(filter.ToListKeyValuePair(start))};
					var response = await _client.SendAsync(req, token);
					var content = await response.Content.ReadAsStringAsync();
					var autoResponse = JsonConvert.DeserializeObject<AutoResponse>(content);
					var parsedResponse = ParseResponse(autoResponse);
					list.AddRange(parsedResponse);
					start++;
					if (list.Count >= autoResponse.Results)
					{
						break;
					}
				}

				return list;
			}
			catch (Exception e)
			{
				Logger.Write("HttpClient. GetCarListAsync. ", e);
			}

			return null;
		}

		public async Task ReserveCarAsync(List<Dictionary<string, string>> list, CancellationToken token)
		{
			Logger.Debug("HttpClient. ReserveCarAsync");
			
			var url = _address + "/xml_savehistory.do";
			try
			{
				ValidateLicense();
			}
			catch (Exception e)
			{
				Logger.Write("HttpClient. ReserveCarAsync. Ошибка проверки лицензии.", e);
				return;
			}
			foreach (var car in list.Where(x => string.IsNullOrEmpty(x["10"])))
			{
				try
				{
					var reserve = new Dictionary<string, string>
					{
						{"daysreserv", "1"},
						{"reservetype", "Y"},
						{"oper", "on"},
						{"stock", "3"},
						{"orderid", car["orderid"]}
					};
					Logger.Write($"Reserve: carId = {car["orderid"]} orderId = {car["orderid"]}");
					var req = new HttpRequestMessage(HttpMethod.Post, url) {Content = new FormUrlEncodedContent(reserve)};
					var response = await _client.SendAsync(req, token);

					if (response.IsSuccessStatusCode)
					{
						Logger.Write($"Reserved: {car["1"]} ({car["6"]}) {car["0"]} {car["orderid"]}");
					}
					else
					{
						Logger.Write(
							$"Reserve: carId = {car["orderid"]} orderId = {car["orderid"]}\n{response.Content.ReadAsStringAsync()}");
					}
				}
				catch (Exception e)
				{
					Logger.Write($"Error at reserve: {car["1"]} ({car["6"]}) {car["0"]} {car["orderid"]}", e);
				}
			}
		}

		public List<Dictionary<string, string>> GetCarListTest(Reserver filter, CancellationToken token)
		{
			ValidateLicense();
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auto.json");
			var content = File.ReadAllText(path);
			var list = new List<Dictionary<string, string>>(100);
			var autoResponse = JsonConvert.DeserializeObject<AutoResponse>(content);
			var parsedResponse = ParseResponse(autoResponse);
			list.AddRange(parsedResponse);
			return list;
		}

		public void Dispose()
		{
			_client?.Dispose();
		}

		/// <summary>
		/// Получить куки с сайта.
		/// </summary>
		/// <param name="token"> Токен отмены.</param>
		/// <returns> Резкультат выполнения запроса.</returns>
		private async Task<bool> GetCookies(CancellationToken token)
		{
			Logger.Debug("HttpClient. GetCookies");
			var url = _address + "/";
			var response = await _client.GetAsync(url, token);
			return response.IsSuccessStatusCode;
		}

		/// <summary>
		/// Распарсить данные со страницы ответа сервера.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private XmlDocument FromHtml(TextReader reader)
		{
			// setup SgmlReader
			var sgmlReader = new Sgml.SgmlReader
			{
				DocType = "HTML",
				WhitespaceHandling = WhitespaceHandling.All,
				CaseFolding = Sgml.CaseFolding.ToLower,
				InputStream = reader
			};

			// create document
			var doc = new XmlDocument
			{
				PreserveWhitespace = true,
				XmlResolver = null
			};
			doc.Load(sgmlReader);
			return doc;
		}

		/// <summary>
		/// Распарсить ответ от сервера.
		/// </summary>
		/// <param name="response"> Ответ от сервера.</param>
		/// <returns> Словарь с описанием характеристик машины.</returns>
		private IReadOnlyCollection<Dictionary<string, string>> ParseResponse(AutoResponse response)
		{
			var list = new List<Dictionary<string, string>>();

			foreach (var xml in response.Rows)
			{
				using (var r = new StringReader(xml))
				{
					var dict = new Dictionary<string, string>();
					var doc = FromHtml(r);

					for (int i = 0; i < response.MetaData.Fields.Count; i++)
					{
						var node = doc.SelectSingleNode($"//td[position()={i + 1}]");
						dict[response.MetaData.Fields[i].Name] = node?.InnerText.Trim();
					}

					list.Add(dict);
				}
			}

			return list;
		}

		private void ValidateLicense()
		{
			if (_licenseWatcher == null || _licenseWatcher.ElapsedMilliseconds - 1000 * 60 > 0)
			{
				_licenseWatcher = Stopwatch.StartNew();
				LicHelper.ValidateLicense();
			}
		}
	}
}