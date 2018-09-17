using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PartsReserver.Models;

namespace PartsReserver
{
	public class HttpClientWrapper : IDisposable
	{

		private readonly HttpClient _client;

		private readonly string _address;

		private readonly CookieContainer _cookies = new CookieContainer();

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
			Logger.Debug("HttpClient. Login");
			try
			{
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
			catch (Exception e)
			{
				Logger.Write("HttpClient. Login. ", e);
				return false;
			}
		}

		/// <summary>
		/// Получить список автомобилей по фильтру.
		/// </summary>
		/// <param name="filter"> Фильтр.</param>
		/// <param name="token"> Токен отмены.</param>
		/// <returns> Ответ от сервера</returns>
		public async Task<HttpResponseMessage> GetCarList(Reserver filter, CancellationToken token)
		{
			Logger.Debug("HttpClient. /a/auto/auto.json");
			try
			{
				var url = _address + "/a/auto/auto.json";
				var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(filter.AutoRequest()) };
				var response = await _client.SendAsync(req, token);
				return response;
			}
			catch (Exception e)
			{
				Logger.Write("HttpClient. GetCarList. ", e);
			}

			return null;
		}

		public void Dispose()
		{
			_client?.Dispose();
		}
	}
}