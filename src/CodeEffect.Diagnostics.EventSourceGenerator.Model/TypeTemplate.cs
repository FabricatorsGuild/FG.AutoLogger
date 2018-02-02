using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FG.Diagnostics.AutoLogger.Model
{
    public class TypeTemplate<TSource> : ITypeTemplate<TSource>, IIncludeAllProperties<TSource>
    {
        private string _name;
        private bool _includeAllProperties = false;
        private string[] _excludeProperties = new string[0];

        private readonly IDictionary<string, AssignmentOutput> _assignmentOutputs = new Dictionary<string, AssignmentOutput>();

        public IEnumerable<string> ExcludeProperties { get; set; }        

        public TypeTemplateModel GetModel()
        {
            DiscoverProperties();

            var arguments = new List<EventArgumentModel>();
            foreach (var assignmentOutput in _assignmentOutputs.Values)
            {
                if (assignmentOutput != null)
                {
                    arguments.Add(new EventArgumentModel(assignmentOutput.FullName, assignmentOutput.Type.FullName, assignmentOutput.Assignment));
                }
            }

            return new TypeTemplateModel()
            {
                Arguments = arguments.ToArray(),
                CLRType = typeof(TSource).FullName,
                Name = _name ?? typeof(TSource).Name
            };
        }

        private void DiscoverProperties()
        {
            // Add all excluded properties
            foreach (var excludeProperty in _excludeProperties)
            {
                if (!_assignmentOutputs.ContainsKey(excludeProperty))
                {
                    _assignmentOutputs.Add(excludeProperty, null);
                }
            }

            // Automatically include all properties not already defined or excluded
            var sourceType = typeof(TSource);
            if (_includeAllProperties)
            {
                var properties = sourceType.GetProperties();
                foreach (var property in properties)
                {
                    var assignmentOutput = Discover(property);
                    if (!_assignmentOutputs.ContainsKey(assignmentOutput.FullName))
                    {
                        _assignmentOutputs.Add(assignmentOutput.FullName, assignmentOutput);
                    }
                }
            }

        }


        public ITypeTemplate<TSource> AddArgument<TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is Expression expression)
            {
                var assignmentOutput = Build(expression);
                if (_assignmentOutputs.ContainsKey(assignmentOutput.FullName))
                {
                    _assignmentOutputs.Remove(assignmentOutput.FullName);
                }
                _assignmentOutputs.Add(assignmentOutput.FullName, assignmentOutput);
            }
            return this;
        }

        public ITypeTemplate<TSource> AddArgument<TProperty>(string argumentName, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is Expression expression)
            {
                var assignmentOutput = Build(expression);
                _assignmentOutputs.Add(argumentName, assignmentOutput);
                if (_assignmentOutputs.ContainsKey(assignmentOutput.FullName))
                {
                    _assignmentOutputs.Remove(assignmentOutput.FullName);
                }
                _assignmentOutputs.Add(assignmentOutput.FullName, assignmentOutput);
            }
            return this;
        }

        public IIncludeAllProperties<TSource> AddAllProperties()
        {
            _includeAllProperties = true;
            return this;
        }

        ITypeTemplate<TSource> IIncludeAllProperties<TSource>.Except(params string[] propertiesToExclude)
        {
            _excludeProperties = propertiesToExclude;
            return this;
        }


        public class AssignmentOutput
        {
            public AssignmentOutput(string name, string fullName, Type type, string assignment)
            {
                Name = name;
                FullName = fullName;
                Type = type;
                Assignment = assignment;
            }

            public string FullName { get; set; }
            public string Name { get; set; }
            public string Assignment { get; set; }
            public Type Type { get; set; }
        }        

        private AssignmentOutput Discover(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            var assignment = $"$this.{name}";

            var type = propertyInfo.PropertyType;
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                var defaultValue = "";
                if (nullableType == typeof(int))
                {
                    defaultValue = "0";
                }
                if (nullableType == typeof(long))
                {
                    defaultValue = "0";
                }
                if (nullableType == typeof(double))
                {
                    defaultValue = "0";
                }
                if (nullableType == typeof(float))
                {
                    defaultValue = "0";
                }
                if (nullableType == typeof(decimal))
                {
                    defaultValue = "0";
                }
                if (nullableType == typeof(bool))
                {
                    defaultValue = "false";
                }
                if (nullableType == typeof(DateTime))
                {
                    defaultValue = "DateTime.MinValue";
                }
                if (nullableType == typeof(Guid))
                {
                    defaultValue = "Guid.Empty";
                }
                assignment = $"{assignment} ?? {defaultValue}";
                type = nullableType;
            }

            if (type.IsClass && type != typeof(string))
            {
                assignment = $"{assignment}.AsJson()";
                type = typeof(string);
            }

            if (type.IsEnum)
            {
                assignment = $"{assignment}.ToString()";
                type = typeof(string);
            }

            return new AssignmentOutput(name, name, type, assignment);
        }

        private AssignmentOutput Build(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                var owner = memberExpression.Expression;
                AssignmentOutput ownerAssignment = null;
                if (owner == null)
                {
                    var memberDeclaringType = memberExpression.Member.DeclaringType;
                    var name = memberDeclaringType.Name;
                    ownerAssignment = new AssignmentOutput(name, name, memberDeclaringType, name);
                }
                else
                {
                    ownerAssignment = Build(memberExpression.Expression);
                }

                if (memberExpression.Member is PropertyInfo propertyInfo)
                {
                    var name = propertyInfo.Name;
                    var fullName = $"{ownerAssignment.FullName}{propertyInfo.Name}";
                    return new AssignmentOutput(name, fullName, propertyInfo.PropertyType,
                        $"{ownerAssignment.Assignment}.{propertyInfo.Name}");
                }
                if (memberExpression.Member is FieldInfo fieldInfo)
                {
                    var name = fieldInfo.Name;
                    var fullName = $"{ownerAssignment.FullName}{fieldInfo.Name}";
                    return new AssignmentOutput(name, fullName, fieldInfo.FieldType,
                        $"{ownerAssignment.Assignment}.{fieldInfo.Name}");
                }
            }
            if (expression is ParameterExpression parameterExpression)
            {
                return new AssignmentOutput("this", "", parameterExpression.Type, "$this");
            }
            if (expression is ConstantExpression constantExpression)
            {
                if (constantExpression.Value is string stringValue)
                {
                    return new AssignmentOutput("const", "", constantExpression.Type, $"\"{stringValue}\"");
                }
                if (constantExpression.Value == null)
                {
                    return new AssignmentOutput("const", "", constantExpression.Type, $"null");
                }
                return new AssignmentOutput("const", "", constantExpression.Type, $"\"{constantExpression.Value.ToString()}\"");
            }
            if (expression is MethodCallExpression methodCallExpression)
            {
                AssignmentOutput callerAssignment = null;
                var arguments = GetCsvList(methodCallExpression.Arguments.Select(a => Build(a).Assignment));

                // Non-static call
                if (methodCallExpression.Object != null)
                {
                    callerAssignment = Build(methodCallExpression.Object);
                }
                // Extension method call
                else if (methodCallExpression.Method.IsDefined(
                    typeof(System.Runtime.CompilerServices.ExtensionAttribute)))
                {
                    var argumentsList = new List<Expression>(methodCallExpression.Arguments);
                    var owner = argumentsList[0];
                    argumentsList.RemoveAt(0);

                    arguments = GetCsvList(argumentsList.Select(a => Build(a).Assignment));

                    callerAssignment = Build(owner);
                }
                // Static method call
                else
                {
                    var methodDeclaringType = methodCallExpression.Method.DeclaringType;
                    var name = methodDeclaringType?.Name ?? "";
                    callerAssignment = new AssignmentOutput(name, name, methodDeclaringType, name);
                }

                var methodName = methodCallExpression.Method.Name;
                var assignment = $"{callerAssignment.Assignment}.{methodCallExpression.Method.Name}({arguments})";
                var fullName = $"{callerAssignment.FullName}{methodCallExpression.Method.Name}";

                if (methodCallExpression.Method.IsSpecialName && methodName == "get_Item")
                {
                    assignment = $"{callerAssignment.Assignment}[{arguments}]";
                    fullName = $"{callerAssignment.FullName}{arguments}";
                }

                return new AssignmentOutput(methodName, fullName,
                    methodCallExpression.Method.ReturnType, assignment);
            }
            if (expression is BinaryExpression binaryExpression)
            {
                var left = Build(binaryExpression.Left);
                var type = left.Type;
                var op = "";
                var name = "";
                var fullName = "";
                var right = Build(binaryExpression.Right);
                switch (binaryExpression.NodeType)
                {
                    case (ExpressionType.Coalesce):
                        op = "??";
                        name = left.Name;
                        fullName = left.FullName;
                        type = Nullable.GetUnderlyingType(type) ?? type;
                        break;
                    case (ExpressionType.Equal):
                        op = "==";
                        name = left.Name;
                        fullName = "";
                        break;
                    case (ExpressionType.NotEqual):
                        op = "!=";
                        name = left.Name;
                        fullName = "";
                        break;

                    default:
                        throw new NotSupportedException($"Binary Expression of type {binaryExpression.NodeType} is not supported yet");
                }

                return new AssignmentOutput(name, fullName, type, $"{left.Assignment} {op} {right.Assignment}");
            }
            if (expression is ConditionalExpression conditionalExpression)
            {
                var test = Build(conditionalExpression.Test);
                var ifTrue = Build(conditionalExpression.IfTrue);
                var ifFalse = Build(conditionalExpression.IfFalse);

                var name = ifTrue.Name;
                var fullName = string.IsNullOrWhiteSpace(ifTrue.FullName) ? ifFalse.FullName : ifTrue.FullName;

                return new AssignmentOutput(name, fullName, ifTrue.Type, $"{test.Assignment} ? {ifTrue.Assignment} : {ifFalse.Assignment}");
            }

            throw new NotSupportedException($"Expression of type {expression.NodeType} is not supported yet");
        }

        public static string GetCsvList<T>(IEnumerable<T> values)
        {
            return GetCsvList(values, null);
        }
        public static string GetCsvList<T>(IEnumerable<T> values, Func<T, string> renderValue, string delimiter = ", ")
        {
            renderValue = renderValue ?? (v => v.ToString());
            var valuesLength = values.Count();
            if (valuesLength == 0) return "";
            if (valuesLength == 1) return renderValue(values.Single());
            return values.Aggregate("", (s, i) => $"{s}, {renderValue(i)}").Substring(2);
        }
    }

    public interface IIncludeAllProperties<TSource> : ITypeTemplate<TSource>
    {
        ITypeTemplate<TSource> Except(params string[] propertiesToExclude);
    }

    public interface ITypeTemplate<TSource>
    {
        TypeTemplateModel GetModel();

        ITypeTemplate<TSource> AddArgument<TProperty>(Expression<Func<TSource, TProperty>> propertyLambda);

        ITypeTemplate<TSource> AddArgument<TProperty>(string argumentName, Expression<Func<TSource, TProperty>> propertyLambda);
    }
}