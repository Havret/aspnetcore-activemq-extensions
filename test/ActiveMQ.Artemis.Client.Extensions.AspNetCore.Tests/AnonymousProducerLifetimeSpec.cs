﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActiveMQ.Artemis.Client.Extensions.AspNetCore.Tests
{
    public class AnonymousProducerLifetimeSpec
    {
        [Fact]
        public async Task Should_register_producer_with_transient_service_lifetime_by_default_1()
        {
            await ShouldRegisterProducerWithTransientServiceLifetimeByDefault(builder => builder.AddAnonymousProducer<TestProducer>());
        }

        [Fact]
        public async Task Should_register_producer_with_transient_service_lifetime_by_default_2()
        {
            await ShouldRegisterProducerWithTransientServiceLifetimeByDefault(builder => builder.AddAnonymousProducer<TestProducer>(new ProducerOptions
            {
                MessagePriority = 9
            }));
        }

        private static async Task ShouldRegisterProducerWithTransientServiceLifetimeByDefault(Action<IActiveMqBuilder> registerProducerAction)
        {
            await using var testFixture = await TestFixture.CreateAsync(registerProducerAction);

            var typedProducer1 = testFixture.Services.GetService<TestProducer>();
            var typedProducer2 = testFixture.Services.GetService<TestProducer>();

            Assert.NotEqual(typedProducer1, typedProducer2);
            Assert.Equal(typedProducer1.Producer, typedProducer2.Producer);
        }

        [Fact]
        public async Task Should_register_producer_with_singleton_service_lifetime_1()
        {
            await ShouldRegisterProducerWithSingletonServiceLifetime(builder => builder.AddAnonymousProducer<TestProducer>(ServiceLifetime.Singleton));
        }

        [Fact]
        public async Task Should_register_producer_with_singleton_service_lifetime_2()
        {
            await ShouldRegisterProducerWithSingletonServiceLifetime(builder => builder.AddAnonymousProducer<TestProducer>(new ProducerOptions
            {
                MessagePriority = 9
            }, ServiceLifetime.Singleton));
        }

        private static async Task ShouldRegisterProducerWithSingletonServiceLifetime(Action<IActiveMqBuilder> registerProducerAction)
        {
            await using var testFixture = await TestFixture.CreateAsync(registerProducerAction);

            var typedProducer1 = testFixture.Services.GetService<TestProducer>();
            var typedProducer2 = testFixture.Services.GetService<TestProducer>();

            Assert.Equal(typedProducer1, typedProducer2);
            Assert.Equal(typedProducer1.Producer, typedProducer2.Producer);
        }

        [Fact]
        public async Task Should_register_producer_with_scoped_service_lifetime_1()
        {
            await ShouldRegisterProducerWithScopedServiceLifetime(builder => builder.AddAnonymousProducer<TestProducer>(ServiceLifetime.Scoped));
        }

        [Fact]
        public async Task Should_register_producer_with_scoped_service_lifetime_2()
        {
            await ShouldRegisterProducerWithScopedServiceLifetime(builder => builder.AddAnonymousProducer<TestProducer>(new ProducerOptions
            {
                MessagePriority = 9
            }, ServiceLifetime.Scoped));
        }

        private static async Task ShouldRegisterProducerWithScopedServiceLifetime(Action<IActiveMqBuilder> registerProducerAction)
        {
            await using var testFixture = await TestFixture.CreateAsync(registerProducerAction);

            using var scope = testFixture.Services.CreateScope();
            var typedProducer1Scope1 = scope.ServiceProvider.GetService<TestProducer>();
            var typedProducer2Scope1 = scope.ServiceProvider.GetService<TestProducer>();

            using var scope2 = testFixture.Services.CreateScope();
            var typedProducerScope2 = scope2.ServiceProvider.GetService<TestProducer>();

            Assert.Equal(typedProducer1Scope1, typedProducer2Scope1);
            Assert.Equal(typedProducer1Scope1.Producer, typedProducer2Scope1.Producer);
            Assert.NotEqual(typedProducerScope2, typedProducer1Scope1);
            Assert.Equal(typedProducerScope2.Producer, typedProducer2Scope1.Producer);
        }

        private class TestProducer
        {
            public IAnonymousProducer Producer { get; }
            public TestProducer(IAnonymousProducer producer) => Producer = producer;
        }
    }
}