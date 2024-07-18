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
    public sealed class Builder
    {
        private List<BuilderElement> _RealizedElements { get; init; } = new();

        public ReadOnlyCollection<Type> RealizedTypes => CastToTypeOnlyCollection(this._RealizedElements);

        public ReadOnlyCollection<BuilderElement> PreResolvedElements => GetPreResolvedElements(this._RealizedElements);

        public ReadOnlyCollection<BuilderElement> AfterResolvedElements => GetAfterResolvedElements(this._RealizedElements);

        public Builder(BuilderElement builderElement)
        {
            Add(builderElement);
        }

        public Builder()
        {

        }

        private ReadOnlyCollection<Type> CastToTypeOnlyCollection(List<BuilderElement> realizedBuilderElement)
        {
            List<Type> realizedTypes = new();

            foreach (BuilderElement element in realizedBuilderElement)
            {
                realizedTypes.Add(element.ToType());
            }

            return realizedTypes.AsReadOnly();
        }

        public void Add(BuilderElement builderElement)
        {
            if (this._RealizedElements.Where(x => x.AbstractionType == builderElement.AbstractionType).Any() == false)
				this._RealizedElements.Add(builderElement);
            else
            {
                BuilderElement existedBuilderElement = this._RealizedElements.Where(x => x.AbstractionType == builderElement.AbstractionType).First();
                IEnumerable<CalleableMethodInfo>? differentsInLists = GetDifferentsInLists(builderElement.CalleableMethodInfos, existedBuilderElement.CalleableMethodInfos);
				if (differentsInLists == null || differentsInLists.Any() == false)
				{ 
				
				} 
                else
                {
                    foreach (CalleableMethodInfo calleableMethodInfo in differentsInLists)
                    {
                        existedBuilderElement.AddMethod(calleableMethodInfo);
                    }
                }
            }
        }

        private IEnumerable<CalleableMethodInfo>? GetDifferentsInLists(ReadOnlyCollection<CalleableMethodInfo>? addeableMethodInfos, ReadOnlyCollection<CalleableMethodInfo>? existeableMethodInfos)
        {
            if (addeableMethodInfos == null && existeableMethodInfos == null)
                return null;
            else
            {
                return addeableMethodInfos.Except(existeableMethodInfos).ToList();
            }
        }

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
