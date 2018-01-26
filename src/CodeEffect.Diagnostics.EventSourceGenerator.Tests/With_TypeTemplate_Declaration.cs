using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FG.Diagnostics.AutoLogger.Generator.Utils;
using FG.Diagnostics.AutoLogger.Model;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Tests
{
    public class With_TypeTemplate_Declaration
    {
        [Test]
        public void Should_build_argument_from_property()
        {
            var typeTemplate = new TypeTemplate<Company>();

            typeTemplate.AddArgument(x => x.Id);

            var typeTemplateModel = typeTemplate.GetModel();

            typeTemplateModel.Arguments.Single().Name.Should().Be("Id");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("$this.Id");
        }

        [Test]
        public void Should_build_argument_from_sub_property()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument(x => x.Company.Id);

            var typeTemplateModel = typeTemplate.GetModel();

            typeTemplateModel.Arguments.Single().Name.Should().Be("CompanyId");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("$this.Company.Id");
        }

        [Test]
        public void Should_build_argument_from_property_with_null_coaless()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument(x => (x.Id ?? ""));

            var typeTemplateModel = typeTemplate.GetModel();

            typeTemplateModel.Arguments.Single().Assignment.Should().Be("Id");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("$this.Id ?? \"\"");
        }


        [Test]
        public void Should_build_argument_from_property_with_datetime_coaless()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument(x => (x.SomeDate ?? DateTime.MinValue));

            var typeTemplateModel = typeTemplate.GetModel();

            typeTemplateModel.Arguments.Single().Name.Should().Be("SomeDate");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("$this.SomeDate ?? DateTime.MinValue");
        }

        [Test]
        public void Should_build_argument_from_property_with_method_Call()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument(x => x.Company.AsJson());

            var typeTemplateModel = typeTemplate.GetModel();

            typeTemplateModel.Arguments.Single().Name.Should().Be("CompanyAsJson");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("$this.Company.AsJson()");
        }

        [Test]
        public void Should_build_argument_from_static_method_with_indexer_property_call()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument("correlationId", x => Context.Current == null ? "" : Context.Current["correlationId"]);

            var typeTemplateModel = typeTemplate.GetModel();

            Console.WriteLine(typeTemplateModel.Arguments.Single().Assignment);
            typeTemplateModel.Arguments.Single().Name.Should().Be("correlationId");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("Context.Current == null ? \"\" : Context.Current[\"correlationId\"]");
        }

        [Test]
        public void Should_build_argument_from_static_method_with_extensionMethod()
        {
            var typeTemplate = new TypeTemplate<Employee>();

            typeTemplate.AddArgument("correlationId", x => ContextExtensionMethods.GetCurrent("correlationId"));

            var typeTemplateModel = typeTemplate.GetModel();

            Console.WriteLine(typeTemplateModel.Arguments.Single().Assignment);
            typeTemplateModel.Arguments.Single().Name.Should().Be("correlationId");
            typeTemplateModel.Arguments.Single().Assignment.Should().Be("ContextExtensionMethods.GetCurrent(\"correlationId\")");
        }

        [Test]
        public void Should_build_full_template_from()
        {
            var typeTemplate = new TemplateEmployee();

            var typeTemplateModel = typeTemplate.GetTypeTemplateModel();
            var arguments = typeTemplateModel.Arguments.OrderBy(a => a.Name).ToArray();

            foreach (var argument in arguments)
            {
                Console.WriteLine($"{argument.Name}[{argument.Type}] {argument.Assignment}");
            }

            var i = 1;
            arguments[i = 0].Name.Should().Be("AnotherSwitch");
            arguments[i].Assignment.Should().Be("$this.AnotherSwitch ?? false");
            arguments[i].Type.Should().Be(typeof(bool).FullName);

            arguments[i = 1].Name.Should().Be("Company");
            arguments[i].Assignment.Should().Be("$this.Company.AsJson()");
            arguments[i].Type.Should().Be(typeof(string).FullName);

            arguments[i = 2].Name.Should().Be("CompanyId");
            arguments[i].Assignment.Should().Be("$this.Company.Id");
            arguments[i].Type.Should().Be(typeof(Guid).FullName);

            arguments[i = 3].Name.Should().Be("Id");
            arguments[i].Assignment.Should().Be("$this.Id");
            arguments[i].Type.Should().Be(typeof(string).FullName);

            arguments[i = 5].Name.Should().Be("SomeDate");
            arguments[i].Assignment.Should().Be("$this.SomeDate ?? DateTime.MinValue");
            arguments[i].Type.Should().Be(typeof(DateTime).FullName);

        }
    }
    
    public class TemplateEmployee : BaseTemplateExtension<Employee>
    {
        protected override void BuildArguments(TypeTemplate<Employee> config)
        {
            config
                .AddAllProperties().Except("DontLogMe", "Reports")
                .AddArgument(x => x.Company.Id)
                .AddArgument(x => x.Id)
                .AddArgument(x => x.SomeDate ?? DateTime.MinValue);
        }
    }

    public class Company
    {
        public Guid Id { get; set; }
        
    }

    public class Employee
    {
        public string Id { get; set; }
        public DateTime? SomeDate { get; set; }
        public Company Company { get; set; }

        public string DontLogMe { get; set; }
        public bool? AnotherSwitch { get; set; }

        public Company[] RelatedCompanies { get; set; }
        public string[] Reports { get; set; }
    }

    public static class ContextExtensionMethods
    {
        public static string GetCurrent(string key)
        {
            return Context.Current?[key];
        }
    }

    public class Context
    {       
        public static Context Current { get; }

        public string this[string key] => key;
    }
}