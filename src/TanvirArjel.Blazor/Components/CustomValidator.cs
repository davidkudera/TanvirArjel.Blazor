﻿// <copyright file="CustomValidator.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace TanvirArjel.Blazor.Components
{
    /// <summary>
    /// Contains methods for adding custom validation message to Blazor FormContext.
    /// </summary>
    public class CustomValidator : ComponentBase
    {
        private ValidationMessageStore _messageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        /// <summary>
        /// Add a generic error message to the current <see cref="EditContext"/>.
        /// </summary>
        /// <param name="errorMessage">The error message to be added.</param>
        public void AddError(string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string> { errorMessage } }
            };

            AddErrors(errors);
        }

        /// <summary>
        /// Add a generic error message to the current <see cref="EditContext"/> and display.
        /// </summary>
        /// <param name="errorMessage">The error message to be added and displayed.</param>
        public void AddAndDisplayError(string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string> { errorMessage } }
            };

            AddAndDiplayErrors(errors);
        }

        /// <summary>
        /// Add a key specified error message to the current <see cref="EditContext"/>.
        /// </summary>
        /// <param name="key">The key of the error message.</param>
        /// <param name="errorMessage">The message to be added.</param>
        public void AddError(string key, string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { key, new List<string> { errorMessage } }
            };

            AddErrors(errors);
        }

        /// <summary>
        /// Add a key specified error message to the current <see cref="EditContext"/> and display.
        /// </summary>
        /// <param name="key">The key of the error message.</param>
        /// <param name="errorMessage">The message to be added and displayed.</param>
        public void AddAndDisplayError(string key, string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { key, new List<string> { errorMessage } }
            };

            AddAndDiplayErrors(errors);
        }

        /// <summary>
        /// Add a collection of error messages to the current <see cref="EditContext"/>.
        /// </summary>
        /// <param name="errors">The erros to be added and displayed.</param>
        public void AddErrors(IEnumerable<string> errors)
        {
            Dictionary<string, List<string>> errorDict = errors.ToDictionary(err => Guid.NewGuid().ToString(), err => new List<string> { err });
            AddErrors(errorDict);
        }

        /// <summary>
        /// Add a collection of error messages to the current <see cref="EditContext"/> and display.
        /// </summary>
        /// <param name="errors">The erros to be added and displayed.</param>
        public void AddAndDisplayErrors(IEnumerable<string> errors)
        {
            Dictionary<string, List<string>> errorDict = errors.ToDictionary(err => Guid.NewGuid().ToString(), err => new List<string> { err });
            AddAndDiplayErrors(errorDict);
        }

        /// <summary>
        /// Add a key-value collection of error messages to the current <see cref="EditContext"/> and display.
        /// </summary>
        /// <param name="errors">The erros to be added and displayed.</param>
        public void AddAndDiplayErrors(IDictionary<string, List<string>> errors)
        {
            AddErrors(errors);
            CurrentEditContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// Add a key-value collection of error messages to the current <see cref="EditContext"/>.
        /// </summary>
        /// <param name="errors">The erros to be added.</param>
        public void AddErrors(IDictionary<string, List<string>> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return;
            }

            PropertyInfo[] propertyInfos = CurrentEditContext.Model.GetType().GetProperties();

            foreach (KeyValuePair<string, List<string>> err in errors)
            {
                if (propertyInfos.Any())
                {
                    bool isExistent = propertyInfos.Any(pi => pi.Name.Equals(err.Key, StringComparison.OrdinalIgnoreCase));

                    if (isExistent)
                    {
                        _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
                    }
                    else
                    {
                        _messageStore.Add(CurrentEditContext.Field(string.Empty), err.Value);
                    }
                }
                else
                {
                    _messageStore.Add(CurrentEditContext.Field(string.Empty), err.Value);
                }
            }
        }

        /// <summary>
        /// Display all the errors of current <see cref="EditContext"/>.
        /// </summary>
        public void DisplayErrors()
        {
            CurrentEditContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// Clear all the errors from the current <see cref="EditContext"/>.
        /// </summary>
        public void ClearErrors()
        {
            _messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// Called when the CustomValidator component is initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CustomValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(CustomValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            _messageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                _messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                _messageStore.Clear(e.FieldIdentifier);
        }
    }
}
