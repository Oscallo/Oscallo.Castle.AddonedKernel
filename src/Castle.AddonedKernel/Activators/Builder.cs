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
using System.Linq;

namespace Castle.AddonedKernel.Activators
{
	/// <summary>
	/// Строитель, отвечающий за элементы активатора
	/// </summary>
    public sealed class Builder
    {
		/// <summary>
		/// Реализуемые элементы
		/// </summary>
		private List<BuilderElement> _RealizedElements { get; init; } = new();

		/// <summary>
		/// Список всех типов
		/// </summary>
		public ReadOnlyCollection<Type> RealizedTypes => CastToTypeOnlyCollection(this._RealizedElements);

		/// <summary>
		/// Элементы, которые будут обработаны перед созданием объекта
		/// </summary>
        public ReadOnlyCollection<BuilderElement> PreResolvedElements => GetPreResolvedElements(this._RealizedElements);

		/// <summary>
		/// Элементы, которые будут обработаны после создания объекта
		/// </summary>
		public ReadOnlyCollection<BuilderElement> AfterResolvedElements => GetAfterResolvedElements(this._RealizedElements);

        public Builder(BuilderElement builderElement)
        {
            Add(builderElement);
        }

        public Builder()
        {

        }

		/// <summary>
		/// Преобразование в коллекцию для чтения
		/// </summary>
		/// <param name="realizedBuilderElement">Список реализуемых элементов</param>
		/// <returns></returns>
        private ReadOnlyCollection<Type> CastToTypeOnlyCollection(List<BuilderElement> realizedBuilderElement)
        {
            List<Type> realizedTypes = new();

            foreach (BuilderElement element in realizedBuilderElement)
            {
                realizedTypes.Add(element.GetIncludedType());
            }

            return realizedTypes.AsReadOnly();
        }

		/// <summary>
		/// Добавить новый элемент
		/// </summary>
		/// <param name="builderElement">Новый обрабатываемый элемент</param>
        public void Add(BuilderElement builderElement)
        {
            if (this._RealizedElements.Where(x => x.AbstractionType == builderElement.AbstractionType).Any() == false)
				this._RealizedElements.Add(builderElement);
            else
            {
                BuilderElement existedBuilderElement = this._RealizedElements.Where(x => x.AbstractionType == builderElement.AbstractionType).First();
                IEnumerable<CalleableMethodInfo>? differentsInLists = GetDifferentsInCollections(builderElement.CalleableMethodInfos, existedBuilderElement.CalleableMethodInfos);
				if (differentsInLists == null || differentsInLists.Any() == false) { } 
                else
                {
                    foreach (CalleableMethodInfo calleableMethodInfo in differentsInLists)
                    {
                        existedBuilderElement.AddMethod(calleableMethodInfo);
                    }
                }
            }
        }

		/// <summary>
		/// Получить от в коллекциях
		/// </summary>
		/// <param name="addeableMethodInfos">Добавляемые методы</param>
		/// <param name="existeableMethodInfos">Уже добавленные методы</param>
		/// <returns>Отличия</returns>
        private IEnumerable<CalleableMethodInfo>? GetDifferentsInCollections(ReadOnlyCollection<CalleableMethodInfo>? addeableMethodInfos, ReadOnlyCollection<CalleableMethodInfo>? existeableMethodInfos)
        {
            if (addeableMethodInfos == null && existeableMethodInfos == null)
                return null;
            else
            {
                return addeableMethodInfos.Except(existeableMethodInfos).ToList();
            }
        }

		/// <summary>
		/// Получить колекцию методов, вызываемых перед созданием объекта
		/// </summary>
		/// <param name="realizedElements">Список реализуемых элементов</param>
		/// <returns>Коллекция элементов строителя</returns>
		private ReadOnlyCollection<BuilderElement> GetPreResolvedElements(List<BuilderElement> realizedElements)
        {
            IEnumerable<BuilderElement> preresolvedElements = this._RealizedElements.Where(x => x.HavePreResolvedMethods == true);
            if (preresolvedElements.Any() == false)
                return new ReadOnlyCollection<BuilderElement>(new List<BuilderElement>());
            else
            {
                return preresolvedElements.ToList().AsReadOnly();
            }
        }

		/// <summary>
		/// Получить колекцию методов, вызываемых после создания объекта
		/// </summary>
		/// <param name="realizedElements">Список реализуемых элементов</param>
		/// <returns>Коллекция элементов строителя</returns>
		private ReadOnlyCollection<BuilderElement> GetAfterResolvedElements(List<BuilderElement> realizedElements)
        {
            IEnumerable<BuilderElement> afterResolvedElements = this._RealizedElements.Where(x => x.HaveAfterResolvedMethods == true);
            if (afterResolvedElements.Any() == false)
                return new ReadOnlyCollection<BuilderElement>(new List<BuilderElement>());
            else
            {
                return afterResolvedElements.ToList().AsReadOnly();
            }
        }
    }
}
