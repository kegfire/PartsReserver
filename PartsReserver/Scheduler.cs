using System.Threading;
using PartsReserver.Models;
using Quartz;
using Quartz.Impl;

namespace PartsReserver
{
	public class Scheduler
	{
		private readonly CancellationToken _cancellationToken;
		public Scheduler(CancellationToken cancellationToken)
		{
			_cancellationToken = cancellationToken;
		}

		public async void Start()
		{
			try
			{
				Logger.Debug($"Запуск планировщика");
				var settings = new Settings();
				settings.Load();
				var jobGroupTitle = "Reserve";
				var scheduler = await StdSchedulerFactory.GetDefaultScheduler(_cancellationToken);
				var job = JobBuilder.Create<ReserveJob>().WithIdentity("Reserve", jobGroupTitle).Build();
				job.JobDataMap.Put("CancellationToken", _cancellationToken);
				var trigger =
					TriggerBuilder.Create()
						.WithIdentity("Reserve", jobGroupTitle)
						.WithSimpleSchedule(s => s.WithIntervalInSeconds(settings.Period).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires()).Build();
				await scheduler.ScheduleJob(job, trigger, _cancellationToken);
				await scheduler.Start(_cancellationToken);
				// Logger.Write($"Инициализация расписания задачи обработки документов СФЕРА Курьер будет выполняться раз в {сourierSettings.ExchangePeriod + 2} минут");
			}
			catch (SchedulerException ex)
			{
				Logger.Write($"Ошибка создания расписания для задачи бронирования  {ex.Message}");
			}
		}
	}
}