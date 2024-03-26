using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Integracao.SAP.SalesForce.Core.Adapters;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Services;
using Integracao.SAP.SalesForce.Core.Adapters;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Services.Mapper.Profiles.Business_Partner;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Infra.Repositories;
using Microsoft.Extensions.Configuration;
using Integracao.SAP.SalesForce.Services.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var devCorsPolicy = "devCorsPolicy";
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

builder.Host.ConfigureDefaults(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Integração SAP X SalesForce";
    })
    .ConfigureServices(services =>
    {
        #region [ Configuration ]
        services.Configure<Configuration>(configuration.GetSection("AppConfig"));
        #endregion

        #region [ Services ]

        //services.AddScoped<IItemsService, ItemsService>();
        #endregion

        #region [ Adapters ]
        services.AddScoped<IHttpAdapter, HttpAdapter>();
        services.AddScoped<IServiceLayerAdapter, ServiceLayerAdapter>();
        services.AddScoped<ISqlAdapter, SqlAdapter>();
        #endregion

        #region [ Repositories ]
        services.AddScoped<ILoggerRepository, LoggerRepository>();
        #endregion

        #region [ Hangfire ]
        services.AddControllers();

        services.AddHangfire((provider, configuration) =>
        {
            configuration.UseMemoryStorage();
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            configuration.UseSimpleAssemblyNameTypeSerializer();
            configuration.UseRecommendedSerializerSettings();
        });

        services.AddHangfireServer();
        #endregion

        #region [ Cors ]
        services.AddCors(services =>
        {
            services.AddPolicy(devCorsPolicy, builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
        #endregion

        #region [ ConfigMap ]
        var config = new AutoMapper.MapperConfiguration(cfg =>
        {

            cfg.AddProfile<CreateBusinessPartnerProfile>();
        });

        IMapper mapper = config.CreateMapper();
        services.AddSingleton(mapper);
        #endregion

    });

using (var app = builder.Build())
{
    app.UseStaticFiles();


    var bps = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:bps", Environment.MachineName),
        Queues = new[] { "bpsqueue" },
        WorkerCount = 1
    };
    var cots = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:cots", Environment.MachineName),
        Queues = new[] { "cotsqueue" },
        WorkerCount = 1
    };
    var cotsCanc = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:cotsCanc", Environment.MachineName),
        Queues = new[] { "cotscancqueue" },
        WorkerCount = 1
    };
    var cotsRet = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:cotsRet", Environment.MachineName),
        Queues = new[] { "cotsretqueue" },
        WorkerCount = 1
    };
    var peds = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:peds", Environment.MachineName),
        Queues = new[] { "pedsqueue" },
        WorkerCount = 1
    };
    var pedsCanc = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:pedsCanc", Environment.MachineName),
        Queues = new[] { "pedscancqueue" },
        WorkerCount = 1
    };
    var pedsRet = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:pedsRet", Environment.MachineName),
        Queues = new[] { "pedsretqueue" },
        WorkerCount = 1
    };


    app.UseHangfireDashboard("/scheduler", new DashboardOptions
    {
        //Authorization = new[] { new HangFireAuthorization() },
        AppPath = "/"
    });


    app.UseHangfireServer(bps);
    app.UseHangfireServer(cots);
    app.UseHangfireServer(cotsCanc);
    app.UseHangfireServer(cotsRet);
    app.UseHangfireServer(peds);
    app.UseHangfireServer(pedsRet);
    app.UseHangfireServer(pedsCanc);
    app.UseHangfireDashboard();




    #region [ Scheduler ]

    #region [ DEBUG ]

#if DEBUG

    var cronServiceTeste = Cron.Minutely();
    var cronServiceDebug = Cron.Never();

    RecurringJob.AddOrUpdate<BusinessPartnersService>("BusinessPartnersService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<QuotationsService>("QuotationsService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<QuotationsCancelService>("QuotationsCancelService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<QuotationsRetornoSF>("QuotationsRetornoSF", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<OrdersService>("OrdersService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<OrdersCancelService>("OrdersCancelService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<OrdersRetornoSF>("OrdersRetornoSF", job => job.ProcessAsync(), cronServiceDebug);




#endif

    #endregion

    #region [ RELEASE ]

#if DEBUG == false

    var cronService_ = Cron.MinuteInterval(5);
    var cronService = Cron.Minutely();

    RecurringJob.AddOrUpdate<BusinessPartnersService>("BusinessPartnersService", job => job.ProcessAsync(), cronService_, null, "bpsqueue");
    RecurringJob.AddOrUpdate<QuotationsService>("QuotationsService", job => job.ProcessAsync(), cronService_, null, "cotsqueue");
    RecurringJob.AddOrUpdate<QuotationsCancelService>("QuotationsCancelService", job => job.ProcessAsync(), cronService_, null, "cotscancqueue");
    RecurringJob.AddOrUpdate<QuotationsRetornoSF>("QuotationsRetornoSF", job => job.ProcessAsync(), cronService_, null, "cotsretqueue");
    RecurringJob.AddOrUpdate<OrdersService>("OrdersService", job => job.ProcessAsync(), cronService_, null, "pedsqueue");
    RecurringJob.AddOrUpdate<OrdersCancelService>("OrdersCancelService", job => job.ProcessAsync(), cronService_, null, "pedscancqueue");
    RecurringJob.AddOrUpdate<OrdersRetornoSF>("OrdersRetornoSF", job => job.ProcessAsync(), cronService_, null, "pedsretqueue");





#endif

    #endregion

    #endregion

    await app.RunAsync();
}

