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
using System;

namespace Oscallo.Castle.AddonedKernel.Integrators
{
    public interface IResolver : IDisposable
    {
		/// <summary>
		/// Проверка компонента на наличие в списке зарегистрированных
		/// </summary>
		/// <typeparam name="T">Предположительно существующая абстракция</typeparam>
		/// <returns></returns>
		bool HasComponent<T>();

        bool HasFacility<TFacility>() where TFacility : IFacility, new();

		/// <summary>
		/// Получить абстрацию  
		/// </summary>
		/// <typeparam name="T">Абстрация</typeparam>
		/// <returns>Экземпляр</returns>
        T Resolve<T>();
    }
}
