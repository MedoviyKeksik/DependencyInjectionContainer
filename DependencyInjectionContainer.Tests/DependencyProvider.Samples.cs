using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer.Tests
{
    enum AServices
    {
        First,
        Second
    }

    interface IAService
    {

    }

    class AService : IAService
    {

    }

    class AServiceDuplicate : IAService
    {

    }

    interface IDependOnAService
    {
        public IAService AService { get; }
    }

    class DependOnAService : IDependOnAService
    {
        public IAService AService { get; }

        public DependOnAService(IAService aService)
        {
            AService = aService;
        }
    }

    interface IAServiceCollectionService
    {
        public IEnumerable<IAService> AServices { get; }
    }

    class IaServiceCollectionService : IAServiceCollectionService
    {
        public IEnumerable<IAService> AServices { get; }

        public IaServiceCollectionService(IEnumerable<IAService> aServices)
        {
            AServices = aServices;
        }
    }

    interface IImplicitCycleDService
    {
        public IImplicitCycleEService ImplicitCycleEService { get; }
    }

    class ImplicitCycleDService : IImplicitCycleDService
    {
        public IImplicitCycleEService ImplicitCycleEService { get; }

        public ImplicitCycleDService(IImplicitCycleEService implicitCycleEService)
        {
            ImplicitCycleEService = implicitCycleEService;
        }
    }

    interface IImplicitCycleEService
    {
        public IImplicitCycleDService ImplicitCycleDService { get; }
    }

    class ImplicitCycleEService : IImplicitCycleEService
    {
        public IImplicitCycleDService ImplicitCycleDService { get; }

        public ImplicitCycleEService(IImplicitCycleDService implicitCycleDService)
        {
            ImplicitCycleDService = implicitCycleDService;
        }
    }


    interface IExplicitCycleService
    {
        public IExplicitCycleService ExplicitCycleServiceImpl { get; }
    }

    class ExplicitCycleService : IExplicitCycleService
    {
        public IExplicitCycleService ExplicitCycleServiceImpl { get; }
        public ExplicitCycleService(IExplicitCycleService explicitCycleServiceImpl)
        {
            ExplicitCycleServiceImpl = explicitCycleServiceImpl;
        }
    }

    interface IGenericService<T>
    {
    }

    class GenericService<T> : IGenericService<T>
    {
        public GenericService()
        {
        }
    }
}