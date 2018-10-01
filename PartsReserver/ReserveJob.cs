using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PartsReserver.Models;
using Quartz;

namespace PartsReserver
{
	internal class ReserveJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			try
			{
				var settings = new Settings();
				settings.Load();
				if (settings.TimeToStart < DateTime.Now.TimeOfDay)
				{
					Logger.Debug("Запуск job'a");
					var dataStore = context.MergedJobDataMap;
					var cancellationToken = dataStore.Get("CancellationToken") is CancellationToken ? (CancellationToken)dataStore.Get("CancellationToken") : new CancellationToken();
					cancellationToken.ThrowIfCancellationRequested();
					var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reservers.xml");
					var reservers = Helper.Deserialize<List<Reserver>>(path);
					using (var client = new HttpClientWrapper(settings.ServerAddress))
					{
						var successLogon = client.Logon(settings.Login, settings.Password, cancellationToken).Result;
						Logger.Debug($"Login = {successLogon}");
						if (successLogon)
						{
							foreach (var reserver in reservers.Where(x => x.Activity))
							{
								Logger.Debug($"Reserver {reserver.Name}.");
								var carList = client.GetCarListAsync(reserver, cancellationToken).Result;
								await client.ReserveCarAsync(carList, cancellationToken);
							}
						}
					}
				}
			
			}
			catch (Exception ex)
			{
				Logger.Write("Ошибка при запросе", ex);
			}
		}
	}
}