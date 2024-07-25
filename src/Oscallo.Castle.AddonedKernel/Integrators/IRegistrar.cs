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

using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using System;

namespace Oscallo.Castle.AddonedKernel.Integrators
{
	/// <summary>
	/// Контракт, гарантирующий, что сущность будет иметь функцмонал регистрации зависимостей
	/// </summary>
    public interface IRegistrar : IDisposable
    {
		/// <summary>
		/// Регистрация 
		/// </summary>
		/// <param name="registrations">Аргументы</param>
        void Register(params IRegistration[] registrations);

		/// <summary>
		/// Регистрация, при отсутствии сущности
		/// </summary>
		/// <typeparam name="T">Регистрируемая абстракция</typeparam>
		/// <param name="registrations">Аргументы</param>
		void RegisterIfAbsent<T>(params IRegistration[] registrations);

        void AddFacility<TFacility>() where TFacility : IFacility, new();

		/// <summary>
		/// Проверка компонента на наличие в списке зарегистрированных
		/// </summary>
		/// <typeparam name="T">Регистрируемая абстракция</typeparam>
		/// <returns></returns>
		bool HasComponent<T>();

        bool HasFacility<TFacility>() where TFacility : IFacility, new();

        void AddFacilityIfAbsent<TFacility>() where TFacility : IFacility, new();
    }
}
