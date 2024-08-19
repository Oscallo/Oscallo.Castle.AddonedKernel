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

namespace Oscallo.Castle.AddonedKernel.Activators
{
	/// <summary>
	/// Активатор, вызывающий методы, предустановленные в <seealso cref="Builder"/>
	/// </summary>
    public class BuildeableComponentActivator : DefaultComponentActivator
    {
		/// <summary>
		/// Активный <seealso cref="Builder"/>
		/// </summary>
		public static Builder Builder => BuildeableComponentActivatorFacility.Builder;

        public BuildeableComponentActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction) { }

		/// <summary>
		/// Метод создания экземпляра объекта
		/// </summary>
		/// <param name="context"></param>
		/// <param name="candidate"></param>
		/// <param name="arguments"></param>
		/// <returns>Экземпляр объекта</returns>
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

		/// <summary>
		/// Вызов метода
		/// </summary>
		/// <param name="calleableMethodInfo">Информация о методе</param>
		/// <param name="activatorArguments">Аргуметы активатора</param>
		/// <param name="createableComponent">Объект, для которого вызываются методы</param>
		/// <exception cref="IndexOutOfRangeException">Выход за границы перечисления</exception>
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
                    component = Activator.CreateInstance(this.Model.Implementation, activatorArguments);
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

		/// <summary>
		/// Пересоздание объекта
		/// </summary>
		/// <param name="context"></param>
		/// <param name="candidate"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
        private object RecreateInstance(CreationContext context, ConstructorCandidate candidate, object[] arguments)
        {
            candidate = SelectEligibleConstructor(context);

            arguments = CreateConstructorArguments(candidate, context);

            return base.CreateInstance(context, candidate, arguments);
        }
    }
}
