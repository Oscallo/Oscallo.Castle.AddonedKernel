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

using Castle.MicroKernel.Registration;
using Oscallo.Castle.AddonedKernel.Demo.MVVM;
using Oscallo.Castle.AddonedKernel.Demo.Services;
using Oscallo.Castle.AddonedKernel.IntegrableInterfaces;
using Oscallo.Castle.AddonedKernel.Integrators;
using System.Windows;

namespace Oscallo.Castle.AddonedKernel.Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IMainWindow
	{
		private readonly IEventeableLogger? _EventeableLogger;

		public MainWindow()
        {
            InitializeComponent();
        }

		public MainWindow(IEventeableLogger eventeableLogger)
		{
			InitializeComponent();

			this._EventeableLogger = eventeableLogger;

			this.logTb.Text = this._EventeableLogger.GetAllMessages();

			this._EventeableLogger.Notify += _EventeableLogger_Notify;
		}

		private void _EventeableLogger_Notify(string message)
		{
			this.logTb.Text += message;
		}

		void IIntegrator.Integrate(IRegistrar injector) 
		{
			injector.RegisterIfAbsent<IEventeableLogger>(Component.For<IEventeableLogger>().ImplementedBy<EventeableLogger>().LifeStyle.Singleton);
		}
	}
}
