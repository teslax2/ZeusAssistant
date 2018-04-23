using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace ZeusAssistant.Model
{
    class JobScheduler
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private NameValueCollection props;
        private StdSchedulerFactory factory;
        private IScheduler scheduler;

        public JobScheduler()
        {
            // Grab the Scheduler instance from the Factory
            props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary"}
                };
            factory = new StdSchedulerFactory(props);
        }

        public async Task Start()
        {
            try
            {   
                scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                logger.Error(se);
            }
        }
        public async Task StopProgram()
        {
            try
            {
                if (scheduler == null) return;
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                logger.Error(se);
            }
        }
    }

    public class HelloJob : IJob
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task Execute(IJobExecutionContext context)
        {
            TunePlayer.PlaySound();
            logger.Info("Tada!");
        }
    }
    
}
