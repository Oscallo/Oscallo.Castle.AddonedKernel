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

using Oscallo.Castle.AddonedKernel.Demo.Services;
using Oscallo.Castle.AddonedKernel.Integrators;
using Oscallo.Castle.AddonedKernel.LifeCyrcle;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Windows;

namespace Oscallo.Castle.AddonedKernel.Demo.DI
{
	public class Injector : IInjector
	{
		private readonly IWindsorContainer _WindsorContainer;
		private readonly IEventeableLogger _EventeableLogger;

		public Injector(IWindsorContainer windsorContainer, IEventeableLogger eventeableLogger)
		{
			this._WindsorContainer = windsorContainer;
			this._EventeableLogger = eventeableLogger;
		}

		public static void Register(IWindsorContainer windsorContainer, params IRegistration[] registrations)
		{
			windsorContainer.Register(registrations);
		}

		public static T Resolve<T>(IWindsorContainer windsorContainer)
		{
			return windsorContainer.Resolve<T>();
		}

		void IRegistrar.AddFacility<TFacility>()
		{
			this._EventeableLogger.Log("Добавлена сущность: " +  typeof(TFacility).FullName);
			this._WindsorContainer.AddFacility<TFacility>();
		}

		void IRegistrar.AddFacilityIfAbsent<TFacility>() 
		{
			IRegistrar dIRegistrar = this;
			if (((IRegistrar)(this)).HasFacility<TFacility>() == false)
			{
				dIRegistrar.AddFacility<TFacility>();
			}
		}

		void IDisposable.Dispose() => this._WindsorContainer?.Dispose();

		public bool HasComponent<T>()
		{
			this._EventeableLogger.Log("Проверка компонента: " +  typeof(T).FullName);
			return this._WindsorContainer.Kernel.HasComponent(typeof(T));
		}

		public bool HasFacility<TFacility>() where TFacility : IFacility, new()
		{
			this._EventeableLogger.Log("Проверка сущности: " +  typeof(TFacility).FullName);
			foreach (IFacility facility in this._WindsorContainer.Kernel.GetFacilities())
			{
				if (facility.GetType() == typeof(TFacility))
				{
					return true;
				}
			}
			return false;
		}

		void IExiteable.OnExit(ExitEventArgs e) => Dispose();

		void IStartupable.OnStartup(StartupEventArgs e) 
		{
			if (e.Args.Length != 0)
			{
				throw new NotImplementedException("Need override method");
			}
		}

		void IRegistrar.Register(params IRegistration[] registrations)
		{
			foreach (IRegistration item in registrations)
			{
				this._EventeableLogger.Log("Регистрация компонента: " +  item.ToString());
			}
			this._WindsorContainer.Register(registrations); 
		}

		void IRegistrar.RegisterIfAbsent<T>(params IRegistration[] registrations) 
		{
			IRegistrar dIRegistrar = this;
			if (HasComponent<T>() == false)
			{
				dIRegistrar.Register(registrations);
			}
		}

		void IContainerRegistrar.AddChildContainer(IWindsorContainer windsorContainer) => this._WindsorContainer.AddChildContainer(windsorContainer);

		void IContainerRegistrar.RemoveChildContainer(IWindsorContainer windsorContainer) => this._WindsorContainer.RemoveChildContainer(windsorContainer);

		T IResolver.Resolve<T>() => this._WindsorContainer.Resolve<T>();

		public virtual void Dispose()
		{
			((IDisposable)this).Dispose();
		}

		public virtual void OnStartup(StartupEventArgs e)
		{
			((IStartupable)this).OnStartup(e);
		}
	}
}
