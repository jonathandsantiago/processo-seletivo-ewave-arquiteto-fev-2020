using FavoDeMel.Service.Interfaces;
using FavoDeMel.Service.Services;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FavoDeMel.IoC
{
    public static class MessagingInject
    {
        public static IServiceCollection AddMensageria(this IServiceCollection services)
        {
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("rabbitmq", "vhost", h =>
                {
                    h.Username("user");
                    h.Password("pass");
                });

                cfg.UseMessageRetry(x => x.Interval(3, TimeSpan.FromDays(1)));
                EndpointConvention.Map<IMessage>(new Uri($"{host.Address.AbsoluteUri}/mensageria_queue"));
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IHostedService, MessagingService>();
            return services;
        }
    }
}
