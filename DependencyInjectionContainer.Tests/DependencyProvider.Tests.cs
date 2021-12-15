using System.Collections.Generic;
using NUnit.Framework;

namespace DependencyInjectionContainer.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ResolveAService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testService = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.NotNull(testService);
        }

        [Test]
        public void ResolveAServiceAllImplementations()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            configuration.AddTransient<IAService, AServiceDuplicate>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            var testServices = dependencyProvider.Resolve<IEnumerable<IAService>>();

            //Assert
            Assert.NotNull(testServices);
            Assert.AreEqual(2, ((List<IAService>)testServices).Count);
        }

        [Test]
        public void ResolveSingletonAServiceAllImplementations()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton<IAService, AService>();
            configuration.AddSingleton<IAService, AServiceDuplicate>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            var testServicesOne = dependencyProvider.Resolve<IEnumerable<IAService>>();
            var testServicesTwo = dependencyProvider.Resolve<IEnumerable<IAService>>();

            //Assert
            Assert.NotNull(testServicesOne);
            Assert.NotNull(testServicesTwo);
            Assert.AreEqual(2, ((List<IAService>)testServicesOne).Count);
            Assert.AreEqual(2, ((List<IAService>)testServicesTwo).Count);
            foreach (var aService in testServicesOne)
            {
                Assert.Contains(aService, (List<IAService>)testServicesTwo);
            }
        }

        [Test]
        public void ResolveAServiceAllImplementationsThroughCService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            configuration.AddTransient<IAService, AServiceDuplicate>();
            configuration.AddTransient<IAServiceCollectionService, IaServiceCollectionService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            var testService = dependencyProvider.Resolve<IAServiceCollectionService>();

            //Assert
            Assert.NotNull(testService);
            Assert.AreEqual(2, ((List<IAService>)testService.AServices).Count);
        }

        [Test]
        public void ResolveBService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            configuration.AddTransient<IDependOnAService, DependOnAService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IDependOnAService testService = dependencyProvider.Resolve<IDependOnAService>();

            //Assert
            Assert.NotNull(testService);
            Assert.NotNull(testService.AService);
        }

        [Test]
        public void ResolveTransientAServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testServiceOne = dependencyProvider.Resolve<IAService>();
            IAService testServiceTwo = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.AreNotEqual(testServiceOne, testServiceTwo);
        }

        [Test]
        public void ResolveSingletonAServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testServiceOne = dependencyProvider.Resolve<IAService>();
            IAService testServiceTwo = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.AreEqual(testServiceOne, testServiceTwo);
        }

        [Test]
        public void ResolveSingletonAServicesAndTransientBServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton<IAService, AService>();
            configuration.AddTransient<IDependOnAService, DependOnAService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IDependOnAService testServiceOne = dependencyProvider.Resolve<IDependOnAService>();
            IDependOnAService testServiceTwo = dependencyProvider.Resolve<IDependOnAService>();

            //Assert
            Assert.AreNotEqual(testServiceOne, testServiceTwo);
            Assert.AreEqual(testServiceOne.AService, testServiceTwo.AService);
        }


        [Test]
        public void ResolveImplicitCycleDependencyServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IImplicitCycleDService, ImplicitCycleDService>();
            configuration.AddTransient<IImplicitCycleEService, ImplicitCycleEService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IImplicitCycleDService testServiceOne = dependencyProvider.Resolve<IImplicitCycleDService>();

            //Assert
            Assert.NotNull(testServiceOne);
            Assert.NotNull(testServiceOne.ImplicitCycleEService);
            Assert.Null(testServiceOne.ImplicitCycleEService.ImplicitCycleDService);
        }

        [Test]
        public void ResolveExplicitCycleDependencyService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IExplicitCycleService, ExplicitCycleService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IExplicitCycleService testServiceOne = dependencyProvider.Resolve<IExplicitCycleService>();

            //Assert
            Assert.NotNull(testServiceOne);
            Assert.Null(testServiceOne.ExplicitCycleServiceImpl);
        }

        [Test]
        public void ResolveGenericService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IGenericService<int>, GenericService<int>>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IGenericService<int> testServiceOne = dependencyProvider.Resolve<IGenericService<int>>();

            //Assert
            Assert.NotNull(testServiceOne);
        }

        [Test]
        public void ResolveOpenGenericService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IGenericService<int> testServiceOne = dependencyProvider.Resolve<IGenericService<int>>();
            IGenericService<int> testServiceTwo = dependencyProvider.Resolve<IGenericService<int>>();

            //Assert
            Assert.NotNull(testServiceOne);
            Assert.AreNotEqual(testServiceOne, testServiceTwo);
        }

        [Test]
        public void ResolveOpenGenericSingletonService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IGenericService<int> testServiceOne = dependencyProvider.Resolve<IGenericService<int>>();
            IGenericService<int> testServiceTwo = dependencyProvider.Resolve<IGenericService<int>>();

            //Assert
            Assert.NotNull(testServiceOne);
            Assert.NotNull(testServiceTwo);
            Assert.AreEqual(testServiceOne, testServiceTwo);
        }

        [Test]
        public void ResolveNamedService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>(AServices.First);
            configuration.AddTransient<IAService, AServiceDuplicate>(AServices.Second);
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testServiceOne = dependencyProvider.Resolve<IAService>(AServices.First);
            IAService testServiceTwo = dependencyProvider.Resolve<IAService>(AServices.Second);

            //Assert
            Assert.NotNull(testServiceOne);
            Assert.NotNull(testServiceTwo);
            Assert.AreEqual(typeof(AService), testServiceOne.GetType());
            Assert.AreEqual(typeof(AServiceDuplicate), testServiceTwo.GetType());
        }
    }
}