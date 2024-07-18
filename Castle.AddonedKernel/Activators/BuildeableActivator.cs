/*
 * Copyright 2024 Oscallo
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Castle.AddonedKernel.Activators
{
    public class BuildeableActivator : DefaultComponentActivator
    {
        public static Builder Builder => BuildeableActivatorFacility.Builder;

        public BuildeableActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction) { }

        protected override object CreateInstance(CreationContext context, ConstructorCandidate candidate, object[] arguments)
        {
            ReadOnlyCollection<BuilderElement> preResolvedElements = Builder.PreResolvedElements;

            foreach (BuilderElement element in preResolvedElements)
            {
                if (this.Model.Implementation.IsAssignableTo(element.AbstractionType) == true)
                {
                    foreach (CalleableMethodInfo calleableMethodInfo in element.CalleableMethodInfos.Where(x => x.MethodCallTypeEnum == CalleableMethodCallTypeEnum.PreResolve))
                    {
                        Invoke(calleableMethodInfo, arguments, null);
                    }
                }
            }

            var component = RecreateInstance(context, candidate, arguments);

            ReadOnlyCollection<BuilderElement> afterResolvedElements = Builder.AfterResolvedElements;

            foreach (BuilderElement element in afterResolvedElements)
            {
                if (this.Model.Implementation.IsAssignableTo(element.AbstractionType) == true)
                {
                    foreach (CalleableMethodInfo calleableMethodInfo in element.CalleableMethodInfos.Where(x => x.MethodCallTypeEnum == CalleableMethodCallTypeEnum.AfterResolve))
                    {
                        Invoke(calleableMethodInfo, arguments, component);
                    }
                }

            }
            return component;
        }

        private void Invoke(CalleableMethodInfo calleableMethodInfo, object[] activatorArguments, object? createableComponent = null)
        {
            object? component;
            object?[]? parameters;

            if (calleableMethodInfo.MethodTypeEnum == CalleableMethodTypeEnum.Static)
            {
                component = null;
            }
            else
            {
                if (calleableMethodInfo.MethodCallTypeEnum == CalleableMethodCallTypeEnum.PreResolve)
                {
                    component = Activator.CreateInstance(this.Model.Implementation);
                }
                else
                {
                    component = createableComponent;
                }
            }

            switch (calleableMethodInfo.MethodArgumentsEnum)
            {
                case CalleableMethodArgumentsEnum.FromCalleabeMethod:
                    parameters = calleableMethodInfo.Arguments;
                    break;
                case CalleableMethodArgumentsEnum.FromInjector:
                    parameters = activatorArguments;
                    break;
                case CalleableMethodArgumentsEnum.FromInjectorWithComponent:
                    parameters = new object[2];
                    parameters[0] = createableComponent;
                    parameters[1] = activatorArguments;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            calleableMethodInfo.MethodInfo.Invoke(component, parameters);
        }

        private object RecreateInstance(CreationContext context, ConstructorCandidate candidate, object[] arguments)
        {
            candidate = SelectEligibleConstructor(context);

            arguments = CreateConstructorArguments(candidate, context);

            return base.CreateInstance(context, candidate, arguments);
        }
    }
}
