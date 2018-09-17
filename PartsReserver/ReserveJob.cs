using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PartsReserver.Models;
using Quartz;

namespace PartsReserver
{
	internal class ReserveJob : IJob
	{
		public Task Execute(IJobExecutionContext context)
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
					var reservers = new List<Reserver> { new Reserver() };
					using (var client = new HttpClientWrapper(settings.ServerAddress))
					{
						var successLogon = client.Logon(settings.Login, settings.Password, cancellationToken).Result;
						Logger.Debug($"Login = {successLogon}");
						if (successLogon)
						{
							foreach (var reserver in reservers.Where(x => x.Activity).AsParallel())
							{
								Logger.Debug($"Reserver {reserver.Name}.");
							}
						}
					}
				}
			
			}
			catch (Exception ex)
			{
				Logger.Write("Ошибка при запросе", ex);
			}
          
			return null;
		}
	}
}