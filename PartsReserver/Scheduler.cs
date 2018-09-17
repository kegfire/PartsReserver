using System.Threading;
using PartsReserver.Models;
using Quartz;
using Quartz.Impl;

namespace PartsReserver
{
	public class Scheduler
	{
		private readonly CancellationToken _cancellationToken;

		private IScheduler _scheduler;
		public Scheduler(CancellationToken cancellationToken)
		{
			_cancellationToken = cancellationToken;
		}

		public async void Start()
		{
			try
			{
				_cancellationToken.ThrowIfCancellationRequested();
				Logger.Debug("Запуск планировщика");
				var settings = new Settings();
				settings.Load();
				var jobGroupTitle = "Reserve";
				_scheduler = await StdSchedulerFactory.GetDefaultScheduler(_cancellationToken);
				var job = JobBuilder.Create<ReserveJob>().WithIdentity("Reserve", jobGroupTitle).Build();
				job.JobDataMap.Put("CancellationToken", _cancellationToken);
				var trigger =
					TriggerBuilder.Create()
						.WithIdentity("Reserve", jobGroupTitle)
						.WithSimpleSchedule(s => s.WithIntervalInSeconds(settings.Period).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires()).Build();
				await _scheduler.ScheduleJob(job, trigger, _cancellationToken);
				await _scheduler.Start(_cancellationToken);
			}
			catch (SchedulerException ex)
			{
				Logger.Write($"Ошибка создания расписания для задачи бронирования  {ex.Message}");
			}
		}

		public void Stop()
		{
			_scheduler?.Shutdown(_cancellationToken);
		}
	}
}