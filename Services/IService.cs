using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Services
{
    public interface IService
    {
        Guid ObtenerTransient();
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
        void RealizarTarea();
    }

    public class ServiceA : IService
    {
        private readonly ILogger<ServiceA> logger;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;

        public ServiceA(ILogger<ServiceA> logger, ServiceTransient serviceTransient, ServiceScoped serviceScoped, ServiceSingleton serviceSingleton)
        {
            this.logger = logger;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
        }

        public Guid ObtenerTransient() { return serviceTransient.Guid; }
        public Guid ObtenerScoped() { return serviceScoped.Guid; }
        public Guid ObtenerSingleton() { return serviceSingleton.Guid; }

        public void RealizarTarea()
        {
        }
    }

    public class ServiceB : IService
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
        }
    }

    public class ServiceTransient
    {
        public Guid Guid = Guid.NewGuid();
    }

    public class ServiceScoped
    {
        public Guid Guid = Guid.NewGuid();
    }

    public class ServiceSingleton
    {
        public Guid Guid = Guid.NewGuid();
    }
}