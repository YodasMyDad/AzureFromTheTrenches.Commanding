﻿using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Implementation
{
    public class FunctionHostBuilder : IFunctionHostBuilder
    {
        public IServiceCollection ServiceCollection { get; }
        public ICommandRegistry CommandRegistry { get; }
        public IFunctionBuilder FunctionBuilder { get; }

        public FunctionHostBuilder(
            IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry,
            IFunctionBuilder functionBuilder)
        {
            ServiceCollection = serviceCollection;
            CommandRegistry = commandRegistry;
            FunctionBuilder = functionBuilder;
        }

        public IFunctionHostBuilder Services(Action<IServiceCollection> services)
        {
            services(ServiceCollection);
            return this;
        }

        public IFunctionHostBuilder Register(Action<ICommandRegistry> registry)
        {
            registry(CommandRegistry);
            return this;
        }

        public IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions)
        {
            functions(FunctionBuilder);
            return this;
        }
    }
}