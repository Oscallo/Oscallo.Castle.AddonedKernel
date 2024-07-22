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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Oscallo.Castle.AddonedKernel.Activators
{
	/// <summary>
	/// Элемент строителя, отвечающий за храние информации об определенном типе
	/// </summary>
    [DebuggerDisplay("Name: {_AbstractionType.Name}, Count: {GetDebuggerCountOrNullString()}")]
    public class BuilderElement
    {
		/// <summary>
		/// Тип, на который реагирует строитель
		/// </summary>
        private Type? _AbstractionType { get; set; } = null;

		/// <summary>
		/// Список методов
		/// </summary>
        private List<CalleableMethodInfo>? _CalleableMethodInfos { get; set; } = null;


        internal Type? AbstractionType
        {
            get { return this._AbstractionType; }
        }

		/// <summary>
		/// Коллекция методов
		/// </summary>
        public ReadOnlyCollection<CalleableMethodInfo>? CalleableMethodInfos
        {
            get { return CastToReadOnlyCollection(this._CalleableMethodInfos); }
        }

        public BuilderElement(Type abstractionType, List<CalleableMethodInfo>? calleableMethodInfos)
        {
			this._AbstractionType = abstractionType;
			this._CalleableMethodInfos = calleableMethodInfos;
        }

        public BuilderElement(Type abstractionType) : this(abstractionType, null) { }

		/// <summary>
		/// Получение хранимого типа
		/// </summary>
		/// <returns>Тип, в отношении которого хранятся моды</returns>
		/// <exception cref="NullReferenceException"></exception>
        public Type GetIncludedType()
        {
            if (this._AbstractionType == null)
                throw new NullReferenceException();
            return this._AbstractionType;
        }

		/// <summary>
		/// Добавление нового метода
		/// </summary>
		/// <param name="method">Информация о методе</param>
        public void AddMethod(CalleableMethodInfo method)
        {
            if (this._CalleableMethodInfos == null)
				this._CalleableMethodInfos = new();
			this._CalleableMethodInfos.Add(method);
        }

		/// <summary>
		/// Преобразование в коллекцию только для чтения
		/// </summary>
		/// <param name="calleableMethodInfos">Список методов</param>
		/// <returns>Коллекция для чтения методов</returns>
        private ReadOnlyCollection<CalleableMethodInfo>? CastToReadOnlyCollection(List<CalleableMethodInfo>? calleableMethodInfos)
        {
            if (calleableMethodInfos == null)
                return null;
            else { return calleableMethodInfos.AsReadOnly(); }
        }

        public bool HavePreResolvedMethods => GetPreResolvedMethods(this._CalleableMethodInfos).Any();

        public bool HaveAfterResolvedMethods => GetAfterResolvedMethods(this._CalleableMethodInfos).Any();

		/// <summary>
		/// Получение методов, которые необходимо вызвать перед созданием
		/// </summary>
		/// <param name="calleableMethodInfos">Все методы</param>
		/// <returns>Коллекция иетодов</returns>
        private IEnumerable<CalleableMethodInfo> GetPreResolvedMethods(List<CalleableMethodInfo>? calleableMethodInfos) => GetResolvedMethods(calleableMethodInfos, CalleableMethodCallTypeEnum.PreResolve);

		/// <summary>
		/// Получение методов, которые необходимо вызвать после создания
		/// </summary>
		/// <param name="calleableMethodInfos">Все методы</param>
		/// <returns>Коллекция иетодов</returns>
		private IEnumerable<CalleableMethodInfo> GetAfterResolvedMethods(List<CalleableMethodInfo>? calleableMethodInfos) => GetResolvedMethods(calleableMethodInfos, CalleableMethodCallTypeEnum.AfterResolve);

		/// <summary>
		/// Получение все методов, в зависимости от состояния их вызова
		/// </summary>
		/// <param name="calleableMethodInfos">Все методы</param>
		/// <returns>Коллекция иетодов</returns>
		private IEnumerable<CalleableMethodInfo> GetResolvedMethods(List<CalleableMethodInfo>? calleableMethodInfos, CalleableMethodCallTypeEnum calleableMethodTypeEnum)
        {
            List<CalleableMethodInfo> resolvedMethodsList = new();

            if (calleableMethodInfos == null)
                return resolvedMethodsList;
            foreach (CalleableMethodInfo methodInfo in calleableMethodInfos)
            {
                if (methodInfo.MethodCallTypeEnum == calleableMethodTypeEnum)
                    resolvedMethodsList.Add(methodInfo);
            }
            return resolvedMethodsList;
        }

		/// <summary>
		/// Получение строки для отладчика
		/// </summary>
		/// <returns></returns>
        private string GetDebuggerCountOrNullString()
        {
            if (this._CalleableMethodInfos == null)
            {
                return "Null";
            }
            else
            {
                return this._CalleableMethodInfos.Count.ToString();
            }
        }
    }
}
