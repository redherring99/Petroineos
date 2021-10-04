using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DRNJ.Petro.UnitTest.Accessor
{
    /// <summary>
    /// DynamicAccessor
    ///
    /// Class to be utilised in Unit Tests to allow protected and private
    /// members and properties to be called from external tests
    ///
    /// Not perfect and based on someone else's hard word along with
    /// a bit of Jeffery magic
    ///
    /// Purists will argue that you should only test public methods etc
    /// I disagree - we're testing internal functionaliy and this is easier
    /// than writing facade derived classes
    /// 
    /// </summary>
    public class DynamicAccessor : DynamicObject
    {
        public PrivateObject privateObject;
        private object parentObject;

        public DynamicAccessor(object d)
        {
            this.privateObject = new PrivateObject(d);
        }

        public DynamicAccessor(object d, Type t)
        {
            this.parentObject = d;
            this.privateObject = new PrivateObject(d, new PrivateType(t));
        }


        /// <summary>
        /// The try invoke member.
        ///
        /// Doesn't work as is with Generic<T> methods
        ///
        /// so use a combination of
        /// https://stackoverflow.com/questions/5492373/get-generic-type-of-call-to-method-in-dynamic-object
        /// to get type params and
        ///  https://stackoverflow.com/questions/42006662/testing-private-static-generic-methods-in-c-sharp
        /// to invoke
        ///
        /// Hot Rod Programming (DJ)
        /// 
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                // https://stackoverflow.com/questions/5492373/get-generic-type-of-call-to-method-in-dynamic-object
                var csharpBinder = binder.GetType().GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
                var typeArgs = (csharpBinder.GetProperty("TypeArguments").GetValue(binder, null) as IList<Type>);

                //------------------------------------
                // if typeargs then a generic method |
                //------------------------------------
                if (typeArgs.Any())
                {
                    //----------------------------------------------------------------------------------------------------
                    // Use reflection to get to method                                                                   |
                    // https://stackoverflow.com/questions/42006662/testing-private-static-generic-methods-in-c-sharp    |
                    //----------------------------------------------------------------------------------------------------


                    MethodInfo fooMethod = this.parentObject.GetType().GetMethod(binder.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fooMethod == null)
                    {
                        result = null;
                        return false;
                    }
                    //-------------------------------------------
                    // Turn Into Generic and invoke with params |
                    //-------------------------------------------
                    MethodInfo genericFooMethod = fooMethod.MakeGenericMethod(typeArgs.ToArray());
                    result = genericFooMethod.Invoke(this.parentObject, args);
                }
                else
                {
                    //-----------------------------
                    // Non Generic so just invoke |
                    //-----------------------------
                    result = privateObject.Invoke(binder.Name, args);
                }

                return true;
            }
            catch (MissingMethodException)
            {
                result = null;
                return false;
            }
        }

        public void SetProperty(string propName, object o)
        {
            this.privateObject.SetProperty(propName, o);
        }

        public object GetProperty(string propName)
        {
            return this.privateObject.GetProperty(propName);
        }


        public object GetField(string fieldName)
        {
            return this.privateObject.GetField(fieldName);
        }

        public void SetField(string fieldName, object value)
        {
            this.privateObject.SetField(fieldName, value);
        }

    }
}
