namespace KerboKatz
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using UniLinq;

    public static class ReflectionHelper
    {
        public static T2 ReflectionExecute<T, T2>(this T instance, Expression<Func<T, T2>> exp)
        {
            return GetExpression(instance, exp).Compile().Invoke(instance);
        }

        public static void ReflectionExecute<T>(this T instance, Expression<Action<T>> exp)
        {
            GetExpression(instance, exp).Compile().Invoke(instance);
        }

        public static void ReflectionAssign<T, T2>(this T instance, Expression<Func<T, T2>> exp, T2 value)
        {
            if (exp.Body is MemberExpression me)
            {
                var member = GetMemberInfo(instance, me);
                if (member == null)
                {
                    throw new InvalidOperationException($"Member {me.Member.Name} not found 1");
                }
                if (member is FieldInfo fi)
                {
                    fi.SetValue(instance, value);
                    return;
                }

                if (member is PropertyInfo pi)
                {
                    pi.SetValue(instance, value, null);
                    return;
                }
            }
            else if (exp.Body is MethodCallExpression mce && mce.Method.Name == "get_Item" && mce.Arguments.Count == 1)
            {
                var m = mce.Object.Type.GetMethod("set_Item");
                var methodField = typeof(MethodCallExpression).GetField("method", BindingFlags.Instance | BindingFlags.NonPublic);
                methodField.SetValue(mce, m);
                //
                var argumentsField = mce.GetType().GetField("arguments", BindingFlags.Instance | BindingFlags.NonPublic);
                var arguments = new List<Expression>((IEnumerable<Expression>)argumentsField.GetValue(mce));
                arguments.Add(Expression.Constant(value, m.GetParameters()[1].ParameterType));
                argumentsField.SetValue(mce, arguments.AsReadOnly());
                var compiledExpression = Expression.Lambda(exp.Body, exp.Parameters.ToArray()).Compile();
                compiledExpression.DynamicInvoke(instance);
                return;
            }
            else
            {
                if (exp.Body is BinaryExpression be)
                {
                    switch (be.NodeType)
                    {
                        case ExpressionType.ArrayIndex:
                            ConvertExpression(instance, be.Left);
                            ConvertExpression(instance, be.Right);
                            var compiledArrayExpression = Expression.Lambda(be.Left, exp.Parameters.ToArray()).Compile();
                            var array = (Array)compiledArrayExpression.DynamicInvoke(instance);
                            var compiledIndexExpression = Expression.Lambda(be.Right, exp.Parameters.ToArray()).Compile();
                            var index = compiledIndexExpression.DynamicInvoke(instance);
                            if (index.GetType() == typeof(long))
                                array.SetValue(value, (long)index);
                            else
                                array.SetValue(value, (int)index);
                            return;
                    }
                }
            }
            throw new InvalidOperationException($"ExpressionType {exp.Body.GetType().FullName} not supported");
        }

        private static TExpression GetExpression<T, TExpression>(this T instance, TExpression exp)
            where TExpression : LambdaExpression
        {
            ConvertExpression(instance, exp.Body);
            return exp;
        }

        private static void ConvertExpression<T, TExpression>(T instance, TExpression exp)
        where TExpression : Expression
        {
            var mce = exp as MethodCallExpression;
            if (mce != null)
            {
                var methods = instance.GetType().GetMethods().Where(e =>
                {
                    if (e.Name != mce.Method.Name) return false;
                    var parameters = e.GetParameters();
                    if (parameters.Length != mce.Arguments.Count) return false;
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (!parameters[i].ParameterType.IsAssignableFrom(mce.Arguments[i].Type)) return false;
                    }

                    return true;
                })
                .ToArray();
                var method = methods.FirstOrDefault(e => e.DeclaringType == instance.GetType()) ?? methods.FirstOrDefault();
                if (method == null)
                {
                    throw new InvalidOperationException($"Method {mce.Method.Name} not found");
                }
                
                var methodField = typeof(MethodCallExpression).GetField("method", BindingFlags.Instance | BindingFlags.NonPublic, out var error);
                if (methodField == null)
                    throw new InvalidOperationException(error);

                methodField.SetValue(mce, method);

                return;
            }

            var me = exp as MemberExpression;
            if (me != null)
            {
                var member = GetMemberInfo(instance, me);
                if (member == null)
                {
                    throw new InvalidOperationException($"Member {me.Member.Name} not found 2");
                }
                var field = me.GetType().GetField("member", BindingFlags.Instance | BindingFlags.NonPublic, out var error);
                if (field == null)
                    throw new InvalidOperationException(error);
                field.SetValue(me, member);
            }

            var be = exp as BinaryExpression;
            if (be != null)
            {
                ConvertExpression(instance, be.Left);
                ConvertExpression(instance, be.Right);
                return;
            }
        }

        private static MemberInfo GetMemberInfo(object instance, MemberExpression me)
        {
            var members = instance.GetType().GetMember(me.Member.Name);
            var member = members.FirstOrDefault(e => e.DeclaringType == instance.GetType()) ?? members.FirstOrDefault();
            return member;
        }
    }
}