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
        private string groupName = "group1";
        private string jobName = "jobName1";
 
        public JobScheduler()
        {
            // Grab the Scheduler instance from the Factory
            props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary"}
                };
            factory = new StdSchedulerFactory(props);
        }
        /// <summary>
        /// Creates Task
        /// </summary>
        /// <param name="triggerName">Name</param>
        /// <param name="croneSchedule">simple schedule or crone schedule</param>
        /// <param name="interval">for simple schedule</param>
        /// <param name="seconds">crone</param>
        /// <param name="minutes">crone</param>
        /// <param name="hours">crone</param>
        /// <param name="dayOfTheWeek">crone</param>
        /// <returns></returns>
        public async Task Start(string triggerName, bool croneSchedule, int interval = 0, int seconds = 0, int minutes = 0, int hours = 0, string dayOfTheWeek = "MON-FRI")
        {
            try
            {   
                scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(jobName, groupName)
                    .Build();

                ITrigger trigger = CreateTrigger(triggerName, croneSchedule,interval,seconds,minutes,hours,dayOfTheWeek);

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

        /// <summary>
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="croneSchedule"></param>
        /// <param name="interval"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        private ITrigger CreateTrigger(string triggerName, bool croneSchedule, int interval, int seconds, int minutes, int hours, string dayOfTheWeek)
        {
            if (!croneSchedule && interval == 0)
            {
                throw new ArgumentException("Interval is 0 for SimpleSchedule");
            }
            ITrigger trigger;
            string scheduleString = string.Format("{0} {1} {2} {3} {4} {5}",seconds,minutes,hours,"?","*", dayOfTheWeek);
            if (croneSchedule)
            {
                trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerName, groupName)
                    .WithCronSchedule(scheduleString) 
                    .ForJob(jobName, groupName)
                    .Build();
            }
            else
            {
                // Trigger the job to run now, and then repeat every 10 seconds
                trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerName, groupName)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(interval)
                        .RepeatForever())
                    .Build();
            }
            return trigger;
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
