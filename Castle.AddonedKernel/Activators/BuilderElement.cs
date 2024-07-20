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

namespace Castle.AddonedKernel.Activators
{
    [DebuggerDisplay("Name: {_AbstractionType.Name}, Count: {GetDebuggerCountOrNullString()}")]
    public class BuilderElement
    {
        private Type? _AbstractionType { get; set; } = null;

        private List<CalleableMethodInfo>? _CalleableMethodInfos { get; set; } = null;

        internal Type? AbstractionType
        {
            get { return this._AbstractionType; }
        }

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

        public Type ToType()
        {
            if (this._AbstractionType == null)
                throw new NullReferenceException();
            return this._AbstractionType;
        }

        public void AddMethod(CalleableMethodInfo method)
        {
            if (this._CalleableMethodInfos == null)
				this._CalleableMethodInfos = new();
			this._CalleableMethodInfos.Add(method);
        }

        private ReadOnlyCollection<CalleableMethodInfo>? CastToReadOnlyCollection(List<CalleableMethodInfo>? calleableMethodInfos)
        {
            if (calleableMethodInfos == null)
                return null;
            else { return calleableMethodInfos.AsReadOnly(); }
        }

        public bool HavePreResolvedMethods => GetPreResolvedMethods(this._CalleableMethodInfos).Any();

        public bool HaveAfterResolvedMethods => GetAfterResolvedMethods(this._CalleableMethodInfos).Any();

        private IEnumerable<CalleableMethodInfo> GetPreResolvedMethods(List<CalleableMethodInfo>? calleableMethodInfos) => GetResolvedMethods(calleableMethodInfos, CalleableMethodCallTypeEnum.PreResolve);

        private IEnumerable<CalleableMethodInfo> GetAfterResolvedMethods(List<CalleableMethodInfo>? calleableMethodInfos) => GetResolvedMethods(calleableMethodInfos, CalleableMethodCallTypeEnum.AfterResolve);

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
