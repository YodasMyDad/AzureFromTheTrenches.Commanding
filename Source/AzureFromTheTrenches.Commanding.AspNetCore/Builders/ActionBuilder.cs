﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Builders
{
    internal class ActionBuilder : IActionBuilder
    {
        private readonly List<ActionDefinition> _actions = new List<ActionDefinition>();

        public IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method) where TCommand : ICommand where TBindingAttribute : Attribute
        {
            return Action<TCommand, TBindingAttribute>(method, null, null);
        }

        public IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, string route)
            where TCommand : ICommand where TBindingAttribute : Attribute
        {
            return Action<TCommand, TBindingAttribute>(method, route, null);
        }

        public IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, Action<IAttributeBuilder> attributeBuilder) where TCommand : ICommand where TBindingAttribute : Attribute
        {
            return Action<TCommand, TBindingAttribute>(method, null, attributeBuilder);
        }

        public IActionBuilder Action<TCommand, TBindingAttribute>(HttpMethod method, string route, Action<IAttributeBuilder> attributeBuilder) where TCommand : ICommand where TBindingAttribute : Attribute
        {
            ActionDefinition definition = new ActionDefinition
            {
                Route = route,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = GetResultType(typeof(TCommand)),
                BindingAttributeType = typeof(TBindingAttribute)
            };

            attributeBuilder?.Invoke(new AttributeBuilder(definition));
            _actions.Add(definition);

            return this;
        }

        public IActionBuilder Action<TCommand>(HttpMethod method) where TCommand : ICommand
        {
            return Action<TCommand>(method, null, null);
        }

        public IActionBuilder Action<TCommand>(HttpMethod method, string route) where TCommand : ICommand
        {
            return Action<TCommand>(method, route, null);
        }

        public IActionBuilder Action<TCommand>(HttpMethod method, Action<IAttributeBuilder> attributeBuilder) where TCommand : ICommand
        {
            return Action<TCommand>(method, null, attributeBuilder);
        }

        public IActionBuilder Action<TCommand>(HttpMethod method, string route, Action<IAttributeBuilder> attributeBuilder) where TCommand : ICommand
        {
            ActionDefinition definition = new ActionDefinition
            {
                Route = route,
                Verb = method,
                CommandType = typeof(TCommand),
                ResultType = GetResultType(typeof(TCommand))
            };

            attributeBuilder?.Invoke(new AttributeBuilder(definition));
            _actions.Add(definition);
            
            return this;
        }

        public IActionBuilder Action(ActionDefinition actionDefinition)
        {
            Type expectedResultType = GetResultType(actionDefinition.CommandType);
            if (actionDefinition.ResultType != null && expectedResultType == null)
            {
                throw new ArgumentException($"The command {actionDefinition.CommandType.Name} does not return a result but the action defintion supplied has specified a result type of {actionDefinition.ResultType.Name}");
            }
            // we DON'T check if a result type is null when a command returns a result as its ok to discard results

            // but we DO check to make sure the result types are compatible
            if (actionDefinition.ResultType != null && expectedResultType != null)
            {
                if (!expectedResultType.IsAssignableFrom(actionDefinition.ResultType))
                {
                    throw new ArgumentException($"The command {actionDefinition.CommandType.Name} returns a result type of {expectedResultType.Name} while the action definition supplied has an incompatible result type specified of {actionDefinition.ResultType.Name}");
                }
            }
            _actions.Add(actionDefinition);
            return this;
        }

        public IReadOnlyCollection<ActionDefinition> Actions => _actions;

        private Type GetResultType(Type commandType)
        {
            Type commandInterface = typeof(ICommand);
            Type commandWithResultInterface = commandType.GetInterfaces().SingleOrDefault(x => commandInterface.IsAssignableFrom(x) && x.IsGenericType);
            if (commandWithResultInterface != null)
            {
                return commandWithResultInterface.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
