using System;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;

namespace Kozachok.Utils.Validation
{
    public static class Validation
    {
        public static void WriteToConsole<TSource, TMemberType>(this TSource o, Expression<Func<TSource, TMemberType>> expression)
        {
            var body = (MemberExpression)expression.Body;
            var memberName = body.Member.Name;
            var compiledExpression = expression.Compile();
            var value = compiledExpression(o);

            Console.WriteLine($"{memberName}: {value}");
        }

        public static TSource IsNullEmptyOrWhitespace<TSource>(this TSource o, Expression<Func<TSource, string>> expression, Action action)
        {
            if (string.IsNullOrWhiteSpace(expression.Compile()(o)))
                action();

            return o;
        }

        public static TSource IsInvalidEmail<TSource>(this TSource o, Expression<Func<TSource, string>> expression, Action action)
        {
            if (!new EmailAddressAttribute().IsValid(expression.Compile()(o)))
                action();

            return o;
        }

        public static TSource IsInvalidGuid<TSource>(this TSource o, Expression<Func<TSource, Guid>> expression, Action action)
        {
            var value = expression.Compile()(o);

            if (value == Guid.Empty)
                action();

            return o;
        }

        public static TSource IsMatchMaxLength<TSource>(this TSource o, Expression<Func<TSource, string>> propertyExpression, Action action)
        {
            var value = propertyExpression.Compile()(o);

            var expression = (MemberExpression)propertyExpression.Body;
            var propertyInfo = (PropertyInfo)expression.Member;
            var attr = propertyInfo.GetCustomAttributes(typeof(MaxLengthAttribute), true).FirstOrDefault() as MaxLengthAttribute;

            if(attr != null && attr.Length > 0 && value.Length >= attr.Length)
                action();

            return o;
        }

        public static TSource IsLessThan<TSource>(this TSource o, Expression<Func<TSource, int>> expression, int targetValue, Action action)
        {
            var value = expression.Compile()(o);

            if (value < targetValue)
                action();

            return o;
        }

        public static TSource IsBiggerThan<TSource>(this TSource o, Expression<Func<TSource, int>> expression, int targetValue, Action action)
        {
            var value = expression.Compile()(o);

            if (value > targetValue)
                action();

            return o;
        }

        public static TSource Is<TSource>(this TSource o, Func<TSource, bool> condition, Action action)
        {
            if (condition(o))
                action();

            return o;
        }
    }
}
