using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _104Crawler.Extension
{
    public static class ProgramQuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config) where T : IJob
        {
            var jobName = typeof(T).Name;

            var configKey = $"Quartz:{jobName}";

            var cron = config[configKey];

            if (string.IsNullOrWhiteSpace(cron))
                throw new Exception("Job has no cron !");

            var jobKey = new JobKey(jobName);

            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts.ForJob(jobKey)
                                            .WithIdentity($"{jobName} - trigger")
                                            .WithCronSchedule(cron));
        }
    }
}
