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

using Castle.AddonedKernel.Activators;
using Castle.AddonedKernel.Demo.DI;
using Castle.AddonedKernel.Demo.IntegrableClasses;
using Castle.AddonedKernel.Demo.MVVM;
using Castle.AddonedKernel.Demo.Services;
using Castle.AddonedKernel.Integrators;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Castle.AddonedKernel.Demo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
    {
		private readonly IInjector _Injector;

		public App() 
		{
			WindsorContainer container = new();

			Injector.Register(container, Component.For<IWindsorContainer>().Instance(container).LifeStyle.Singleton);
			Injector.Register(container, Component.For<IEventeableLogger>().ImplementedBy<EventeableLogger>().LifeStyle.Singleton);
			Injector.Register(container, Component.For<IInjector>().ImplementedBy<Injector>().LifeStyle.Singleton);

			this._Injector = Injector.Resolve<IInjector>(container);

			Integrate(this._Injector);

			// Bug # 0001 https://github.com/users/Oscallo/projects/3?pane=issue&itemId=71677593
			IMainWindow mainWindow = this._Injector.Resolve<IMainWindow>();

			this.MainWindow = (Window)mainWindow;

			this.MainWindow.Show();

			Resolve(this._Injector);
		}

		private void Resolve(IResolver resolver) 
		{
			IUpperOrderClass upperOrderClass = resolver.Resolve<IUpperOrderClass>();
		}

		private void Integrate(IRegistrar injector)
		{
			BuildeableComponentActivatorFacility.Builder = GenerateBuilder(injector, BuildeableComponentActivatorFacility.Builder);

			injector.AddFacilityIfAbsent<BuildeableComponentActivatorFacility>();

			injector.RegisterIfAbsent<IMainWindow>(Component.For<IMainWindow>().ImplementedBy<MainWindow>().LifeStyle.Singleton);

			injector.RegisterIfAbsent<IUpperOrderClass>(Component.For<IUpperOrderClass>().ImplementedBy<UpperOrderClass>().LifeStyle.Singleton);
		}

		private Builder? GenerateBuilder(IRegistrar injector, Builder? builder)
		{
			List<CalleableMethodInfo> methodInfos = new();

			object[] parametersArray = new object[] { injector };

			MethodInfo methodInfo = typeof(IIntegrator).GetMethod(nameof(IIntegrator.Integrate));

			CalleableMethodInfo calleableMethodInfo = new() { MethodCallTypeEnum = CalleableMethodCallTypeEnum.PreResolve, MethodTypeEnum = CalleableMethodTypeEnum.Examplar, MethodInfo = methodInfo, Arguments = parametersArray, MethodArgumentsEnum = CalleableMethodArgumentsEnum.FromCalleabeMethod };

			methodInfos.Add(calleableMethodInfo);

			if (builder == null)
			{
				return new Builder(new BuilderElement(typeof(IIntegrator), methodInfos));
			}
			else
			{
				builder.Add(new BuilderElement(typeof(IIntegrator), methodInfos));
				return builder;
			}
		}
	}
}
