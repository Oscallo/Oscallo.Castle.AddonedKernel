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

using Castle.Core;
using Castle.MicroKernel.Facilities;
using System;

namespace Castle.AddonedKernel.Activators
{
    public class BuildeableActivatorFacility : AbstractFacility
    {
        private static Builder? _Builder;

        public static Builder? Builder
        {
            get { return _Builder; }
            set
            {
                if (_Builder == null)
                {
                    _Builder = value;
                }
            }
        }

        public BuildeableActivatorFacility() { }

        protected override void Init()
        {
            this.Kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
        }

        private void Kernel_ComponentModelCreated(ComponentModel model)
        {
            if (Builder != null)
            {
                foreach (Type? modelServiceType in model.Services)
                {
                    foreach (Type realizedType in Builder.RealizedTypes)
                    {
                        bool isRealized = realizedType.IsAssignableFrom(modelServiceType);

                        if (model.CustomComponentActivator == null)
                        {
                            model.CustomComponentActivator = typeof(BuildeableActivator);
                            return;
                        }
                    }
                }
            }
        }

        protected override void Dispose()
        {
            this.Kernel.ComponentModelCreated -= Kernel_ComponentModelCreated;
            base.Dispose();
        }
    }
}
