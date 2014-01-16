// <copyright file="ReflectionModelBinder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved. 
// </copyright>
//
// <summary>
//  Reflection Model Binder
// </summary>
//
// <history>
//  <change who="Anders" date="2011.09.14">Created</change>
// </history>
namespace Fanex.NexHR.ViewModels.ModelBinder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// Reflection Model Binder.
    /// </summary>
    /// <typeparam name="T">Generics type.</typeparam>
    public class ReflectionModelBinder<T> : CustomModelBinder where T : new()
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var viewModel = (T)Activator.CreateInstance(typeof(T));
            Map(viewModel, bindingContext);

            return viewModel;
        }

        /// <summary>
        /// Function Map.
        /// </summary>
        /// <param name="viewModel">An Object type.</param>
        /// <param name="bindingContext">ModelBinding Context.</param>
        /// <returns>Map results.</returns>
        public object Map(T viewModel, ModelBindingContext bindingContext)
        {
            // Not check property readonly
            PropertyInfo[] propertyList = viewModel.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyList)
            {
                if (propertyInfo.CanWrite)
                {
                    if (bindingContext.PropertyMetadata.ContainsKey(propertyInfo.Name))
                    {
                        ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + propertyInfo.Name);

                        if (result == null && bindingContext.FallbackToEmptyPrefix)
                        {
                            result = bindingContext.ValueProvider.GetValue(propertyInfo.Name);
                        }

                        // just allow bind value type
                        if (result != null)
                        {
                            propertyInfo.SetValue(viewModel, result.ConvertTo(propertyInfo.PropertyType), null);
                        }
                    }
                }
            }

            return null;
        }
    }
}
