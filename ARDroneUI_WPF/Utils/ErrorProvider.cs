/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Paul Stovell
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection;

namespace ARDrone.UI
{
    public class ErrorProvider : Decorator
    {
        private delegate void FoundBindingCallbackDelegate(FrameworkElement element, Binding binding, DependencyProperty dp);
        private FrameworkElement _firstInvalidElement;
        private Dictionary<DependencyObject, Style> _backupStyles = new Dictionary<DependencyObject, Style>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ErrorProvider()
        {
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ErrorProvider_DataContextChanged);
            this.Loaded += new RoutedEventHandler(ErrorProvider_Loaded);
        }

        /// <summary>
        /// Called when this component is loaded. We have a call to Validate here that way errors appear from the very 
        /// moment the page or form is visible.
        /// </summary>
        private void ErrorProvider_Loaded(object sender, RoutedEventArgs e)
        {
            Validate();
        }

        /// <summary>
        /// Called when our DataContext changes.
        /// </summary>
        private void ErrorProvider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged -= new PropertyChangedEventHandler(DataContext_PropertyChanged);
            }

            if (e.NewValue != null && e.NewValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged += new PropertyChangedEventHandler(DataContext_PropertyChanged);
            }

            Validate();
        }

        /// <summary>
        /// Validates all properties on the current data source.
        /// </summary>
        /// <returns>True if there are no errors displayed, otherwise false.</returns>
        /// <remarks>
        /// Note that only errors on properties that are displayed are included. Other errors, such as errors for properties that are not displayed, 
        /// will not be validated by this method.
        /// </remarks>
        public bool Validate()
        {
            bool isValid = true;
            _firstInvalidElement = null;

            if (this.DataContext is IDataErrorInfo)
            {
                List<Binding> allKnownBindings = ClearInternal();

                // Now show all errors
                foreach (Binding knownBinding in allKnownBindings)
                {
                    string errorMessage = ((IDataErrorInfo)this.DataContext)[knownBinding.Path.Path];
                    if (errorMessage != null && errorMessage.Length > 0)
                    {
                        isValid = false;

                        // Display the error on any elements bound to the property
                        FindBindingsRecursively(
                        this.Parent,
                        delegate(FrameworkElement element, Binding binding, DependencyProperty dp)
                        {
                            if (knownBinding.Path.Path == binding.Path.Path)
                            {

                                BindingExpression expression = element.GetBindingExpression(dp);
                                ValidationError error = new ValidationError(new ExceptionValidationRule(), expression, errorMessage, null);
                                System.Windows.Controls.Validation.MarkInvalid(expression, error);

                                if (_firstInvalidElement == null)
                                {
                                    _firstInvalidElement = element;
                                }
                                return;

                            }
                        });
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Returns the first element that this error provider has labelled as invalid. This method 
        /// is useful to set the users focus on the first visible error field on a page.
        /// </summary>
        /// <returns></returns>
        public FrameworkElement GetFirstInvalidElement()
        {
            return _firstInvalidElement;
        }

        /// <summary>
        /// Clears any error messages.
        /// </summary>
        public void Clear()
        {
            ClearInternal();
        }

        /// <summary>
        /// Clears any error messages and returns a list of all bindings on the current form/page. This is simply so 
        /// it can be reused by the Validate method.
        /// </summary>
        /// <returns>A list of all known bindings.</returns>
        private List<Binding> ClearInternal()
        {
            // Clear all errors
            List<Binding> bindings = new List<Binding>();
            FindBindingsRecursively(
                    this.Parent,
                    delegate(FrameworkElement element, Binding binding, DependencyProperty dp)
                    {
                        // Remember this bound element. We'll use this to display error messages for each property.
                        bindings.Add(binding);
                    });
            return bindings;
        }

        /// <summary>
        /// Called when the PropertyChanged event is raised from the object we are bound to - that is, our data context.
        /// </summary>
        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid")
            {
                return;
            }
            Validate();
        }

        /// <summary>
        /// Recursively goes through the control tree, looking for bindings on the current data context.
        /// </summary>
        /// <param name="element">The root element to start searching at.</param>
        /// <param name="callbackDelegate">A delegate called when a binding if found.</param>
        private void FindBindingsRecursively(DependencyObject element, FoundBindingCallbackDelegate callbackDelegate)
        {

            // See if we should display the errors on this element
            MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy);

            foreach (MemberInfo member in members)
            {
                DependencyProperty dp = null;

                // Check to see if the field or property we were given is a dependency property
                if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo field = (FieldInfo)member;
                    if (typeof(DependencyProperty).IsAssignableFrom(field.FieldType))
                    {
                        dp = (DependencyProperty)field.GetValue(element);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo prop = (PropertyInfo)member;
                    if (typeof(DependencyProperty).IsAssignableFrom(prop.PropertyType))
                    {
                        dp = (DependencyProperty)prop.GetValue(element, null);
                    }
                }

                if (dp != null)
                {
                    // Awesome, we have a dependency property. does it have a binding? If yes, is it bound to the property we're interested in?
                    Binding bb = BindingOperations.GetBinding(element, dp);
                    if (bb != null)
                    {
                        // This element has a DependencyProperty that we know of that is bound to the property we're interested in. 
                        // Now we just tell the callback and the caller will handle it.
                        if (element is FrameworkElement)
                        {
                            if (((FrameworkElement)element).DataContext == this.DataContext)
                            {
                                callbackDelegate((FrameworkElement)element, bb, dp);
                            }
                        }
                    }
                }
            }

            // Now, recurse through any child elements
            if (element is FrameworkElement || element is FrameworkContentElement)
            {
                foreach (object childElement in LogicalTreeHelper.GetChildren(element))
                {
                    if (childElement is DependencyObject)
                    {
                        FindBindingsRecursively((DependencyObject)childElement, callbackDelegate);
                    }
                }
            }
        }
    }
}