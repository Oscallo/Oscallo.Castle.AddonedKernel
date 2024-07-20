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

using Castle.AddonedKernel.LifeCyrcle;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using System;

namespace Castle.AddonedKernel.Integrators
{
    public interface IRegistrar : ILifeCyrcleSupport, IDisposable
    {
        void Register(params IRegistration[] registrations);

        void RegisterIfAbsent<T>(params IRegistration[] registrations);

        void AddFacility<TFacility>() where TFacility : IFacility, new();

        bool HasComponent<T>();

        bool HasFacility<TFacility>() where TFacility : IFacility, new();

        void AddFacilityIfAbsent<TFacility>() where TFacility : IFacility, new();
    }
}
