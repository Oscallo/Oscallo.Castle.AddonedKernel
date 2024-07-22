﻿/*
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

using Oscallo.Castle.AddonedKernel.Integrators;
using Castle.MicroKernel.Registration;

namespace Oscallo.Castle.AddonedKernel.Demo.IntegrableClasses
{
	public class UpperOrderClass : IUpperOrderClass, IIntegrator
	{
		private IMiddleOrderWithDepClass? _IMiddleOrderWithDepClass;
		private IMiddleOrderWithoutDepClass? _IMiddleOrderWithoutDepClass;

		public UpperOrderClass() { }

		public UpperOrderClass(IMiddleOrderWithDepClass middleOrderWithDepClass, IMiddleOrderWithoutDepClass middleOrderWithoutDepClass) 
		{
			this._IMiddleOrderWithDepClass = middleOrderWithDepClass;
			this._IMiddleOrderWithoutDepClass = middleOrderWithoutDepClass;
		}

		public void Integrate(IRegistrar injector) 
		{
			injector.RegisterIfAbsent<IMiddleOrderWithDepClass>(Component.For<IMiddleOrderWithDepClass>().ImplementedBy<MiddleOrderWithDepClass>().LifeStyle.Singleton);
			injector.RegisterIfAbsent<IMiddleOrderWithoutDepClass>(Component.For<IMiddleOrderWithoutDepClass>().ImplementedBy<MiddleOrderWithoutDepClass>().LifeStyle.Singleton);
		}
	}
}