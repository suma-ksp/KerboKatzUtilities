namespace KerboKatz
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;
    using UniLinq;

    public class ReflectionHelper<T1>
    {
        public readonly T1 instance;

        protected ReflectionHelper(T1 instance)
        {
            this.instance = instance;
        }
        public T GetMemberWithReflection<T>(Expression<Func<T1, T>> field)
        {
            var member = instance.GetType().GetField((field.Body as MemberExpression).Member.Name);
            if(member != null)
                return (T)member.GetValue(instance);
            var property = instance.GetType().GetProperty((field.Body as MemberExpression).Member.Name);
            return (T)property.GetValue(instance, new object[0]);
        }
        public void SetMemberWithReflection<T>(Expression<Func<T1, T>> field, T value)
        {
            var member = instance.GetType().GetField((field.Body as MemberExpression).Member.Name);
            if (member != null)
            {
                member.SetValue(instance, value);
                return;
            }

            var property = instance.GetType().GetProperty((field.Body as MemberExpression).Member.Name);
            property.SetValue(instance, value, new object[0]);
        }

        public T GetMethodResult<T>(Expression<Func<T1, T>> field)
        {
            var mce = field.Body as MethodCallExpression;
            var method = instance.GetType().GetMethod(mce.Method.Name);
            if (method == null)
                throw new InvalidOperationException($"Method {mce.Method.Name} not found on type " + instance.GetType().FullName);
            var arguments = new List<object>();
            foreach (var oneArgument in mce.Arguments)
            {
                var c = oneArgument as ConstantExpression;
                if (c == null)
                {
                    var m = oneArgument as MemberExpression;
                    if (m != null)
                        c = m.Expression as ConstantExpression;
                    if (m.Member is FieldInfo fi)
                        arguments.Add(fi.GetValue(c.Value));
                    else if (m.Member is PropertyInfo pi)
                        arguments.Add(pi.GetValue(c.Value, null));
                    else
                        throw new InvalidOperationException($"Unable to parse argument of type {oneArgument.GetType()} to ConstantExpression");
                }
                else
                    arguments.Add(c.Value);
            }
            return (T)method.Invoke(instance, arguments.ToArray());
        }

        public void ExecuteMethod(Expression<Action<T1>> field)
        {
            var mce = field.Body as MethodCallExpression;
            if(mce == null)
                throw new InvalidOperationException("Expression is not a MethodCallExpression");
            var method = instance.GetType().GetMethod(mce.Method.Name);
            if (method == null)
                throw new InvalidOperationException($"Method {mce.Method.Name} not found on type " + instance.GetType().FullName);
            var arguments = new List<object>();
            foreach (var oneArgument in mce.Arguments)
            {
                var c = oneArgument as ConstantExpression;
                if (c == null)
                {
                    var m = oneArgument as MemberExpression;
                    if (m != null)
                        c = m.Expression as ConstantExpression;
                    if (m.Member is FieldInfo fi)
                        arguments.Add(fi.GetValue(c.Value));
                    else if (m.Member is PropertyInfo pi)
                        arguments.Add(pi.GetValue(c.Value, null));
                    else
                        throw new InvalidOperationException($"Unable to parse argument of type {oneArgument.GetType()} to ConstantExpression");
                }
                else
                    arguments.Add(c.Value);
            }
            method.Invoke(instance, arguments.ToArray());
        }
    }
}
