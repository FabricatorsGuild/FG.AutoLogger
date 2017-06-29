using System;

namespace FG.Diagnostics.AutoLogger.Model
{
    public abstract class BaseTemplateExtension<T> : BaseWithLogging, ITypeTemplateDefinition
    {
        private TypeTemplateModel _typeTemplateModel;

        protected abstract string GetDefinition();

        private TypeTemplateModel GetTypeTemplateModelInternal()
        {
            if (_typeTemplateModel == null)
            {
                var definition = GetDefinition();
                _typeTemplateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<TypeTemplateModel>(definition);
            }
            return _typeTemplateModel;
        }

        public bool IsTemplateFor(EventArgumentModel argument)
        {
            var templateTypeName = GetTypeTemplateModelInternal().Name;
            var templateTypeCLR = GetTypeTemplateModelInternal().CLRType;
            var argumentTypeName = argument.Type ?? argument.CLRType;
            var argumentTypeCLR = argument.CLRType ?? argument.Type;

            if (templateTypeName.Equals(argumentTypeName, StringComparison.InvariantCultureIgnoreCase) ||
                templateTypeCLR.Equals(argumentTypeCLR, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }            
            return false;
        }

        public bool IsInheritedTemplateFor(EventArgumentModel argument)
        {
            var templateTypeCLR = GetTypeTemplateModelInternal().CLRType ?? GetTypeTemplateModelInternal().Name;
            var argumentTypeCLR = argument.CLRType ?? argument.Type;

            try
            {
                var argumentType = Type.GetType(argumentTypeCLR);
                var templateType = Type.GetType(templateTypeCLR);
                if (templateType?.IsAssignableFrom(argumentType) ?? false)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Extension '{this.GetType().Name}' Failed to get type {argument.CLRType} - {ex.Message}");
            }
            return false;
        }

        public TypeTemplateModel GetTypeTemplateModel()
        {
            return GetTypeTemplateModelInternal();
        }

        public abstract string Module { get; }

    }
}